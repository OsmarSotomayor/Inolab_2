using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;
using System.Configuration;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Collections.Generic;
using SpreadsheetLight;
using System.Windows;
using DocumentFormat.OpenXml.Drawing.Charts;
using INOLAB_OC;
using INOLAB_OC.Modelo;

public partial class ServiciosAsignados : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["idUsuario"] == null) { 
            Response.Redirect("./Sesion.aspx");
        }
        else {
            //En caso de que sean los jefes de area de los ingenieros tendran acceso al boton de calendario por el area a la que representan
            lbluser.Text = Session["nameUsuario"].ToString();
            if (Session["idUsuario"].ToString() == "54" || Session["idUsuario"].ToString() == "60" ||
                Session["idUsuario"].ToString() == "30")
            {
                cg.Visible = true;
            }
            else
            {
                cg.Visible = false;
            }
        }
    }

    //Conexion a la base de datos (para hacer prebas acceder a BrowserPruebas)
    SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ReportViewer1.ServerReport.ReportServerCredentials = new MyReportServerCredentials();
            // Set the processing mode for the ReportViewer to Remote
            ReportViewer1.ProcessingMode = ProcessingMode.Remote;

            ServerReport serverReport = ReportViewer1.ServerReport;

            // Set the report server URL and report path
            serverReport.ReportServerUrl = new Uri("http://INOLABSERVER01/Reportes_Inolab");
            serverReport.ReportPath = "/Servicio/Calendario-Servicio-Ing";

            // Create the sales order number report parameter
            ReportParameter salesOrderNumber = new ReportParameter();
            salesOrderNumber.Name = "ing";
            salesOrderNumber.Values.Add(Session["idUsuario"].ToString());

            // Set the report parameters for the report
            ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { salesOrderNumber });
            ReportViewer1.ShowParameterPrompts = false;
        }
    }

    [Serializable]

    public sealed class MyReportServerCredentials :
        IReportServerCredentials
    {//Inicializa el reporteador con las credenciales almacenadas en la configuración
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use the default Windows user.  Credentials will be
                // provided by the NetworkCredentials property.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                /* Read the user information from the Web.config file.  
                 By reading the information on demand instead of 
                 storing it, the credentials will not be stored in 
                 session, reducing the vulnerable surface area to the
                 Web.config file, which can be secured with an ACL.

                 User name */
                string userName =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerUser"];

                if (string.IsNullOrEmpty(userName))
                    throw new Exception(
                        "Missing user name from web.config file");

                // Password
                string password =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerPassword"];

                if (string.IsNullOrEmpty(password))
                    throw new Exception(
                        "Missing password from web.config file");

                // Domain
                string domain =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerDomain"];

                return new NetworkCredential(userName, password, domain);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie,
                    out string userName, out string password,
                    out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            // Not using form credentials
            return false;
        }
    }

    private void cargardatos()
    {
        //Carga los folios del ingeniero
        
            string query = "Select DISTINCT* from  v_fsr where idingeniero = " + Session["Idusuario"] + " order by folio desc";
            GridView1.DataSource = Conexion.getDataSet(query);
            GridView1.DataBind();
            contador.Text = GridView1.Rows.Count.ToString();
       
    }

    protected void GridView1_OnRowComand(object sender, GridViewCommandEventArgs e)
    {
        //Al darle clic al folio deseado este se almacena en la sesión y te redirige a la ventana de FSR
        try
        {

            int index = int.Parse(e.CommandArgument.ToString());
            GridViewRow row = GridView1.Rows[index];
            String text = "";
            String tipo = "";

            if (e.CommandName == "Select")
            {
                text = ((LinkButton)row.Cells[0].Controls[0]).Text;
                tipo = ((LinkButton)row.Cells[1].Controls[0]).Text;
                Session["folio_p"] = text;
                Session["not_ase"] = "";
                if (tipo.Equals("Asignado"))
                {
                    //Validacion de fecha 
                    String fech = "";
                    String Hoy = "";
                    fech = row.Cells[4].Text;
                    Hoy = DateTime.Now.ToString("dd/MM/yyyy");
                    if (Hoy == fech)
                    {
                        Response.Redirect("FSR.aspx", true);
                    }
                    else
                    {
                        //En caso de que la fecha de servicio sea distint a la fecha en la que se quiere abrir el folio, lo rechazara
                        Response.Write("<script>alert('Error: Fecha de servicio no coincide con el dia de hoy');</script>");
                    }
                }
                else
                {
                    Response.Redirect("FSR.aspx", true);
                }
            }
            if (e.CommandName == "Select2")
            {
                //Para el modo offline, genera un archivo con toda la informacion que ya hay del folio para que se llene en el aplicativo accediendo a esta 
                text = ((LinkButton)row.Cells[0].Controls[0]).Text;
                tipo = ((LinkButton)row.Cells[1].Controls[0]).Text;
                Session["folio_p"] = text;
                if (tipo.Equals("Asignado"))
                {
                    SLDocument sl = new SLDocument();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    //Columnas (Claves)
                    dt.Columns.Add("IdFSR", typeof(string));
                    dt.Columns.Add("Folio", typeof(string));
                    dt.Columns.Add("Cliente", typeof(string));
                    dt.Columns.Add("Departamento", typeof(string));
                    dt.Columns.Add("Direccion", typeof(string));
                    dt.Columns.Add("Telefono", typeof(string));
                    dt.Columns.Add("Localidad", typeof(string));
                    dt.Columns.Add("N_Reportado", typeof(string));
                    dt.Columns.Add("N_Responsable", typeof(string));
                    dt.Columns.Add("Mail", typeof(string));
                    dt.Columns.Add("TipoContrato", typeof(string));
                    dt.Columns.Add("TipoProblema", typeof(string));
                    dt.Columns.Add("TipoServicio", typeof(string));
                    dt.Columns.Add("servicio", typeof(string));
                    dt.Columns.Add("Ingeniero", typeof(string));
                    dt.Columns.Add("IdIngeniero", typeof(string));
                    dt.Columns.Add("mailIng", typeof(string));
                    dt.Columns.Add("F_SolicitudServicio", typeof(string));
                    dt.Columns.Add("FechaServicio", typeof(string));
                    dt.Columns.Add("Equipo", typeof(string));
                    dt.Columns.Add("Marca", typeof(string));
                    dt.Columns.Add("Modelo", typeof(string));
                    dt.Columns.Add("NoSerie", typeof(string));
                    dt.Columns.Add("IdEquipo_C", typeof(string));
                    dt.Columns.Add("Estatusid", typeof(string));
                    dt.Columns.Add("Estatus", typeof(string));
                    dt.Columns.Add("Observaciones", typeof(string));
                    dt.Columns.Add("NoLlamada", typeof(string));
                    dt.Columns.Add("Inicio_Servicio", typeof(string));
                    dt.Columns.Add("Fin_Servicio", typeof(string));
                    dt.Columns.Add("Dia", typeof(string));
                    dt.Columns.Add("FallaReportada", typeof(string));
                    dt.Columns.Add("HoraServicio", typeof(string));
                    dt.Columns.Add("Confirmacion", typeof(string));
                    dt.Columns.Add("Propuesta", typeof(string));
                    dt.Columns.Add("Actividad", typeof(string));
                    dt.Columns.Add("S_Confirmacion", typeof(string));
                    dt.Columns.Add("Asesor1", typeof(string));
                    dt.Columns.Add("Correoasesor1", typeof(string));
                    dt.Columns.Add("CooreoIng", typeof(string));
                    dt.Columns.Add("Proximo_Servicio", typeof(string));
                    dt.Columns.Add("idcontrato", typeof(string));
                    dt.Columns.Add("idservicio", typeof(string));
                    dt.Columns.Add("idproblema", typeof(string));
                    dt.Columns.Add("IdResp", typeof(string));
                    dt.Columns.Add("Responsable", typeof(string));
                    dt.Columns.Add("IdDocumenta", typeof(string));
                    dt.Columns.Add("Documentador", typeof(string));
                    dt.Columns.Add("Refaccion", typeof(string));
                    dt.Columns.Add("Ingeniero_A1", typeof(string));
                    dt.Columns.Add("IdIng_A1", typeof(string));
                    dt.Columns.Add("mailIng_A1", typeof(string));
                    dt.Columns.Add("Ingeniero_A2", typeof(string));
                    dt.Columns.Add("IdIng_A2", typeof(string));
                    dt.Columns.Add("mailIng_A2", typeof(string));
                    dt.Columns.Add("F_InicioServicio", typeof(string));
                    dt.Columns.Add("F_FinServicio", typeof(string));
                    dt.Columns.Add("IdT_Servicio", typeof(string));
                    dt.Columns.Add("OC", typeof(string));
                    dt.Columns.Add("ArchivoAdjunto", typeof(string));
                    dt.Columns.Add("DiaInicioServ", typeof(string));
                    dt.Columns.Add("DiaFinServ", typeof(string));
                    dt.Columns.Add("DiasServ", typeof(string));
                    dt.Columns.Add("NotAsesor", typeof(string));
                    dt.Columns.Add("Funcionando", typeof(string));
                    dt.Columns.Add("FallaEncontrada", typeof(string));
                    dt.Columns.Add("FechaFirmCliente", typeof(string));
                    dt.Columns.Add("NombreCliente", typeof(string));

                    //Registros (Valores)
                    List<string> Valores = new List<string>();
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from  v_fsr where Folio = " + Session["folio_p"], con);

                    //Para quitar los espaciados anteriores y posteriores
                    char[] charsToTrim = {' '};

                    SqlDataReader leer;
                    leer = cmd.ExecuteReader();
                    if (leer.Read())
                    {
                        Valores.Insert(0, leer["IdFSR"].ToString());
                        Valores.Insert(1, leer["Folio"].ToString());
                        Valores.Insert(2, leer["Cliente"].ToString());
                        Valores.Insert(3, leer["Departamento"].ToString());
                        Valores.Insert(4, leer["Direccion"].ToString());
                        Valores.Insert(5, leer["Telefono"].ToString());
                        Valores.Insert(6, leer["Localidad"].ToString());
                        Valores.Insert(7, leer["N_Reportado"].ToString().Trim(charsToTrim));
                        Valores.Insert(8, leer["N_Responsable"].ToString());
                        Valores.Insert(9, leer["Mail"].ToString());
                        Valores.Insert(10, leer["TipoContrato"].ToString());
                        Valores.Insert(11, leer["TipoProblema"].ToString());
                        Valores.Insert(12, leer["TipoServicio"].ToString());
                        Valores.Insert(13, leer["servicio"].ToString());
                        Valores.Insert(14, leer["Ingeniero"].ToString());
                        Valores.Insert(15, leer["IdIngeniero"].ToString());
                        Valores.Insert(16, leer["mailIng"].ToString());
                        Valores.Insert(17, Convert.ToDateTime(leer["F_SolicitudServicio"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        Valores.Insert(18, Convert.ToDateTime(leer["FechaServicio"].ToString()).ToString("yyyy-MM-dd"));
                        Valores.Insert(19, leer["Equipo"].ToString().Trim(charsToTrim));
                        Valores.Insert(20, leer["Marca"].ToString());
                        Valores.Insert(21, leer["Modelo"].ToString());
                        Valores.Insert(22, leer["NoSerie"].ToString());
                        Valores.Insert(23, leer["IdEquipo_C"].ToString());
                        Valores.Insert(24, leer["Estatusid"].ToString());
                        Valores.Insert(25, leer["Estatus"].ToString());
                        Valores.Insert(26, leer["Observaciones"].ToString());
                        Valores.Insert(27, leer["NoLlamada"].ToString());
                        Valores.Insert(28, "");
                        Valores.Insert(29, "");
                        Valores.Insert(30, leer["Dia"].ToString());
                        Valores.Insert(31, leer["FallaReportada"].ToString());
                        Valores.Insert(32, leer["HoraServicio"].ToString());
                        Valores.Insert(33, leer["Confirmacion"].ToString());
                        Valores.Insert(34, leer["Propuesta"].ToString());
                        Valores.Insert(35, leer["Actividad"].ToString());
                        Valores.Insert(36, leer["S_Confirmacion"].ToString());
                        Valores.Insert(37, leer["Asesor1"].ToString());
                        Valores.Insert(38, leer["Correoasesor1"].ToString());
                        Valores.Insert(39, leer["CooreoIng"].ToString());
                        Valores.Insert(40, "");
                        Valores.Insert(41, leer["idcontrato"].ToString());
                        Valores.Insert(42, leer["idservicio"].ToString());
                        Valores.Insert(43, leer["idproblema"].ToString());
                        Valores.Insert(44, leer["IdResp"].ToString());
                        Valores.Insert(45, leer["Responsable"].ToString());
                        Valores.Insert(46, leer["IdDocumenta"].ToString());
                        Valores.Insert(47, leer["Documentador"].ToString());
                        Valores.Insert(48, leer["Refaccion"].ToString());
                        Valores.Insert(49, leer["Ingeniero_A1"].ToString());
                        Valores.Insert(50, leer["IdIng_A1"].ToString());
                        Valores.Insert(51, leer["mailIng_A1"].ToString());
                        Valores.Insert(52, leer["Ingeniero_A2"].ToString());
                        Valores.Insert(53, leer["IdIng_A2"].ToString());
                        Valores.Insert(54, leer["mailIng_A2"].ToString());
                        Valores.Insert(55, leer["F_InicioServicio"].ToString());
                        Valores.Insert(56, leer["F_FinServicio"].ToString());
                        Valores.Insert(57, leer["IdT_Servicio"].ToString());
                        Valores.Insert(58, leer["OC"].ToString());
                        Valores.Insert(59, leer["ArchivoAdjunto"].ToString());
                        Valores.Insert(60, leer["DiaInicioServ"].ToString());
                        Valores.Insert(61, leer["DiaFinServ"].ToString());
                        Valores.Insert(62, leer["DiasServ"].ToString());
                        Valores.Insert(63, leer["NotAsesor"].ToString());
                        Valores.Insert(64, leer["Funcionando"].ToString());
                        Valores.Insert(65, leer["FallaEncontrada"].ToString());
                        Valores.Insert(66, "");
                        Valores.Insert(67, leer["NombreCliente"].ToString());
                    }
                    con.Close();

                    dt.Rows.Add(
                        Valores[0],
                        Valores[1],
                        Valores[2],
                        Valores[3],
                        Valores[4],
                        Valores[5],
                        Valores[6],
                        Valores[7],
                        Valores[8],
                        Valores[9],
                        Valores[10],
                        Valores[11],
                        Valores[12],
                        Valores[13],
                        Valores[14],
                        Valores[15],
                        Valores[16],
                        Valores[17],
                        Valores[18],
                        Valores[19],
                        Valores[20],
                        Valores[21],
                        Valores[22],
                        Valores[23],
                        Valores[24],
                        Valores[25],
                        Valores[26],
                        Valores[27],
                        Valores[28],
                        Valores[29],
                        Valores[30],
                        Valores[31],
                        Valores[32],
                        Valores[33],
                        Valores[34],
                        Valores[35],
                        Valores[36],
                        Valores[37],
                        Valores[38],
                        Valores[39],
                        Valores[40],
                        Valores[41],
                        Valores[42],
                        Valores[43],
                        Valores[44],
                        Valores[45],
                        Valores[46],
                        Valores[47],
                        Valores[48],
                        Valores[49],
                        Valores[50],
                        Valores[51],
                        Valores[52],
                        Valores[53],
                        Valores[54],
                        Valores[55],
                        Valores[56],
                        Valores[57],
                        Valores[58],
                        Valores[59],
                        Valores[60],
                        Valores[61],
                        Valores[62],
                        Valores[63],
                        Valores[64],
                        Valores[65],
                        Valores[66],
                        Valores[67]
                        );

                    sl.ImportDataTable(1, 1, dt, true);
                    string filepath = HttpRuntime.AppDomainAppPath + "Docs\\" + Session["folio_p"].ToString() + ".xlsx";

                    sl.SaveAs(filepath);
                    try
                    {
                        //Parte del codigo para pasar el archivo del servidor al equipo del ingeniero
                        using (WebClient client = new WebClient())
                        {
                            //Creacion del archivo de el folio para llenarlo modo offline
                            client.DownloadFile("http://23.102.185.149:8020/Docs/" + Session["folio_p"].ToString() + ".xlsx", 
                                @"C:\INOLAB\Datos Folio\" + Session["Usuario"].ToString() + "_" + Session["folio_p"].ToString() + ".xlsx");
                        }
                    }
                    catch (Exception es)
                    {
                        Console.Write(es.ToString());
                        Response.Write("<script>alert('Error al pasar el archivo al equipo del usuario');</script>");
                    }
                    Response.Write("<script>alert('Información cargada para modo Offline');</script>");
                }
                else
                {
                    Response.Write("<script>alert('Solo se pueden descargar folios con estatus de -Asignado-');</script>");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

    public void pagina()
    {
        DownloadFolio(Session["folio_p"].ToString());
        Response.Redirect("FSR.aspx");    
    }

    protected void DownloadFolio(string folio)
    {//Esta función intenta descargar el folio del reporteador, si falla, vuelve a inicializar el reporteador
        try
        {
            string filepath = HttpRuntime.AppDomainAppPath + "Docs\\" + folio + ".pdf";
            byte[] readText = File.ReadAllBytes(filepath);
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();
            string nombre = "Folio:" + folio + "_" + year + ".pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
            Response.BinaryWrite(readText);
            Response.Flush();
        }
        catch(Exception e)
        {
            recreatePDF(folio);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {//Cerrar sesión de usuario
        Session.Clear();
        Session.Abandon();
        Response.Redirect("./Sesion.aspx");
    }

    protected void btnCalendario_Click(object sender, EventArgs e)
    {//Esta función sirve para generar el calendario de servicios del ingeniero en formato pdf
        ServerReport serverReport = ReportViewer1.ServerReport;

        // Set the report server URL and report path
        serverReport.ReportServerUrl = new Uri("http://INOLABSERVER01/Reportes_Inolab");
        serverReport.ReportPath = "/Servicio/Calendario-Servicio-Ing";

        // Create the sales order number report parameter
        ReportParameter salesOrderNumber = new ReportParameter();
        salesOrderNumber.Name = "ing";
        salesOrderNumber.Values.Add(Session["idUsuario"].ToString());

        // Set the report parameters for the report
        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { salesOrderNumber });
        ReportViewer1.ShowParameterPrompts = false;

        string month = DateTime.Now.Month.ToString();
        string year = DateTime.Now.Year.ToString();
        string day = DateTime.Now.Day.ToString();
        string nombre = "Calendario_" + day + "-" + month + "-" + year + ".pdf";
        CreatePDF(nombre);
    }

    protected void recreatePDF(string folio)
    {//Esta función sirve para crear el Folio de un servicio finalizado.

        ServerReport serverReport = ReportViewer1.ServerReport;
        // Set the report server URL and report path
        serverReport.ReportServerUrl = new Uri("http://INOLABSERVER01/Reportes_Inolab");
        serverReport.ReportPath = "/OC/FSR Servicio";

        // Create the sales order number report parameter
        ReportParameter salesOrderNumber = new ReportParameter();
        salesOrderNumber.Name = "folio";
        salesOrderNumber.Values.Add(folio);

        // Set the report parameters for the report
        ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { salesOrderNumber });
        ReportViewer1.ShowParameterPrompts = false;

        string month = DateTime.Now.Month.ToString();
        string year = DateTime.Now.Year.ToString();
        string nombre = "Folio:" + folio + "_" + year + ".pdf";
        CreatePDF(nombre);
    }

    private void CreatePDF(string nombre)
    {//Genera el reporte y ejecuta la descarga del archivo en formato PDF
        // Variables  
        Warning[] warnings;
        string[] streamIds;
        string mimeType = string.Empty;
        string encoding = string.Empty;
        string extension = string.Empty;

        byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.  
        Response.Buffer = true;
        Response.Clear();
        Response.ContentType = mimeType;
        Response.AddHeader("content-disposition", "attachment; filename="+ nombre);
        Response.BinaryWrite(bytes); // create the file  
        Response.Flush(); // send it to the client to download  
    }

    string coman="";
    protected void ddlfiltro_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Filtro de folios dependiendo a su estado de FSR Estatus 
        if(ddlfiltro.Text=="Asignado")
        {
            coman = "select *from V_FSR where Estatus='Asignado' and IdIngeniero=" + Session["idusuario"] +" order by folio desc";
            sentencia();
        }
        if (ddlfiltro.Text == "En Proceso")
        {
            coman = "select *from V_FSR where Estatus='En Proceso' and IdIngeniero=" + Session["idusuario"]+" order by folio desc";
            sentencia();
        }
        if (ddlfiltro.Text == "Finalizado")
        {
            coman = "select *from v_fsr where estatus='Finalizado' and idingeniero=" + Session["idusuario"]+ " order by folio desc";
            sentencia();
        }
        if (ddlfiltro.Text == "Todos")
        {
            cargardatos();
        }
    }
    public void sentencia()
    {
        //Proceso de llenado del datagridview 
        GridView1.DataSource = Conexion.getDataSet(coman);
        GridView1.DataBind();
        contador.Text = GridView1.Rows.Count.ToString();
    }

    protected void btndescargafolio_Click(object sender, EventArgs e)
    {
        //pasa a la opcion de descargar el folio que se encuentre finalizado
        string _open = "window.open('DescargaFolio.aspx', '_newtab');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), _open, true);
    }

    protected void btnCalendario1_Click(object sender, EventArgs e)
    {
        //Ir a caldenario
        Response.Redirect("Calendario.aspx");
    }

    protected void manual_Click(object sender, EventArgs e)
    {
        //descargar manual de usuario
        Response.Redirect(@"\Docs\Manual de Usuario SWF v3.pdf");
    }

    protected void btninformacion_Click(object sender, EventArgs e)
    {
        string _open = "window.open('Informacion.aspx', '_newtab');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), _open, true);
    }
}