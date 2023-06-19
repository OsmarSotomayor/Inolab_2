using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;
using INOLAB_OC.Modelo;

public partial class VistaPrevia : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        cargarDatosInicialesDeUsuario();
    }

   private void cargarDatosInicialesDeUsuario()
    {
        if (Session["idUsuario"] == null)
        {
            Response.Redirect("./Sesion.aspx");
        }
        else
        {
            lbluser.Text = Session["nameUsuario"].ToString();
        }
    }

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
            serverReport.ReportPath = "/OC/FSR Servicio v2";

            // Create the sales order number report parameter
            ReportParameter salesOrderNumber = new ReportParameter();
            salesOrderNumber.Name = "folio";
            salesOrderNumber.Values.Add(Session["folio_p"].ToString());

            // Set the report parameters for the report
            ReportViewer1.ServerReport.SetParameters( new ReportParameter[] { salesOrderNumber });
            ReportViewer1.ShowParameterPrompts = false;
        }
    }

    [Serializable]
    public sealed class MyReportServerCredentials :
        IReportServerCredentials
    {//Inicializa el Reporteador
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
                    throw new Exception("Missing user name from web.config file");

                // Password
                string password =
                    ConfigurationManager.AppSettings
                        ["MyReportViewerPassword"];

                if (string.IsNullOrEmpty(password))
                    throw new Exception("Missing password from web.config file");

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

    protected void Mostrar_pantalla_para_firma_de_documento_Click(object sender, EventArgs e)
    {
        avisopriv.Style.Add("display", "block");
        headerid.Style.Add("display", "none");
        sectionreport.Style.Add("display", "none");
        footerid.Style.Add("display", "none");
    }

    protected void Btn_Aceptar_aviso_de_privacidad(object sender, EventArgs e)
    {
        avisopriv.Style.Add("display", "none");
        ulfol.Text = Session["folio_p"].ToString();
        ul_advert.Style.Add("display", "block");
    }

    protected void Mostrar_ventana_de_firma_de_usuario(object sender, EventArgs e)
    {
        ul_advert.Style.Add("display", "none");
        firma.Style.Add("display", "block");
        string script = "startFirma();";
        ClientScript.RegisterStartupScript(GetType(), "Star", script, true);
    }

    protected void Btn_guardar_firmar_Click(object sender, EventArgs e)
    {
        string image = hidValue.Value;
        string nombre = textboxnombre.Text;
        firma.Style.Add("display", "none");
        headerid.Style.Add("display", "block");
        sectionreport.Style.Add("display", "block");
        footerid.Style.Add("display", "flex");
        if (nombre.Length < 1)
        {
            nombre = "N/A";
        }
        if(insertarFirma(nombre, image))
        {
            actualizarNombreDeClienteYFechaEnQueFirma(nombre);
            ReportViewer1.ServerReport.Refresh();
        }
    }

    protected void actualizarNombreDeClienteYFechaEnQueFirma(string nombre)
    { 
        string query = " UPDATE FSR SET NombreCliente='" + nombre + "', FechaFirmaCliente=" +
                "CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";";
        Conexion.executeQuery(query);
        
    }

    protected bool insertarFirma(string nombreDeImagen, string imagen)
    {
        try
        {
            string[] images = imagen.Split(',');
            string pattern = @"[^:\s*]\w+\/[\w-+\d.]+(?=[;| ])";
            string tipoDeImagen = "";
            string img1 = images[0];
            string img2 = images[1];
            Regex rx = new Regex(pattern);
            Match m = rx.Match(img1);
            if (m.Success)
                tipoDeImagen = m.Value;
           
            int idFirmaImagen = Conexion.insertarFirmaImagen(nombreDeImagen, tipoDeImagen, img2);
            if (idFirmaImagen != 0)
            {
                Conexion.executeQuery("update FSR set IdFirmaImg=" + idFirmaImagen + " where Folio=" + Session["folio_p"] + ";");
                return true;
            }
            else
            {
               
                return false;
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            return false;
        }
    }

    protected void Finalizar_reporte_Click(object sender, EventArgs e)
    {
        int firmaUsuario = 0;
        int firmaCliente = 0;

        firmaUsuario = verificarSiSeAgregoFirmaDeCliente();
        firmaCliente = verificarSiSeAgregoFirmaDeIngeniero();

        if (firmaUsuario == 0 || firmaCliente == 0)
        {

        }
        else
        {
            //Agregar codigo de nueva ventana emergente donde se seleccionaran las fechas que iran al folio
            floatsection.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            sectionreport.Style.Add("display", "none");
            reportdiv.Style.Add("display", "none");
            footerid.Style.Add("display", "none");
        }


    }

    private int verificarSiSeAgregoFirmaDeCliente()
    {
        int firmaCliente = -1;
        try
        {
            firmaCliente = Conexion.getScalar("select IdFirmaImg from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            if (firmaCliente == -1)
            {
                Response.Write("<script>alert('Falta firma de cliente');</script>");
            }
        }
        catch (Exception es)
        {
            Response.Write("<script>alert('Falta firma de cliente');</script>");
        }
        return firmaCliente;
    }

    private int verificarSiSeAgregoFirmaDeIngeniero()
    {
        int firmaIngeniero =-1;
        try
        {
            firmaIngeniero = Conexion.getScalar("select IDFirmaIng from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            if (firmaIngeniero == -1 )
            {
                Response.Write("<script>alert('Falta firma de Ingeniero');</script>");
            }
        }
        catch (Exception er)
        {
            Response.Write("<script>alert('Falta firma de Ingeniero');</script>");
        }
        return firmaIngeniero;
    }
    private void ActStatus()
    {
        try
        {
            //Actualizacion a estado de cierre de actividad de SAP
            string actualizarEstatus = "Update OCLG set OCLG.status = -3, OCLG.Closed = 'Y', OCLG.CloseDate =CAST('" +
                DateTime.Now.ToString("yyyy-MM-dd") + "' AS DATETIME) from OCLG INNER JOIN SCL5 ON OCLG.ClgCode=SCL5.ClgID where SCL5.U_FSR ='" + Session["folio_p"].ToString() + "'";
            ConexionInolab.executeQuery(actualizarEstatus);

            string queryUpdateStatusSCL5 = "Update SCL5 set U_ESTATUS = 'Finalizado' where U_FSR ='" + Session["folio_p"].ToString() + "'";
            ConexionInolab.executeQuery(queryUpdateStatusSCL5);
           
            //Buscar el callid
            string querySrvcCallId = "Select SrvcCallId FROM SCL5 where U_FSR ='" + Session["folio_p"].ToString() + "'";
            string resultado =ConexionInolab.getText(querySrvcCallId);

            //Me da el Callid 
            string querySCL5 = "Select count (DISTINCT U_ESTATUS) FROM SCL5 where SrvcCallId = " + resultado.ToString();
            string resultado2 = ConexionInolab.getText(querySCL5);

            //resultado2 el numero de valores que hay en estatus (Para que se cierre la llamada debe de ser "-1")


            //Hacer un ciclo while para identificar nulos? (usar visorder para pasar por todos)
            string queryCountSCL5 = "Select count(*) FROM SCL5 where SrvcCallId = " + resultado.ToString();
            string resultadoc = ConexionInolab.getText(queryCountSCL5);
            

            bool nulo = false;

            for (int i = 1; i <= Convert.ToInt32(resultadoc); i++)
            {
                //Comprueba si hy m folios con distintos estatus en la tabla de la llamada en SAP
                string queryt = "Select U_ESTATUS FROM SCL5 where SrvcCallId = " + resultado.ToString() + 
                    "and VisOrder = " + i.ToString();
                string resultadot = ConexionInolab.getText(queryt);

                if (resultadot != "Finalizado")
                {
                    nulo = true;
                }
                
            }

            if (resultado2 == "1" && nulo == false)
            {
                //En caso de que todos los registros correspondientes a la llamda esten con estatus de finalizado, finaliza la llamada
                ConexionInolab.executeQuery("Update OSCL set status = -1 where callID=" + resultado.ToString());
                
            }
        }
        catch (Exception er)
        {
            Response.Write("<script>alert('Fallo en subir a sap ');</script>");
        }

    }
    
    private void SendMail(string filepath, string mail)
    {//Envía el correo electrónico con la información del FSR y adjunto el archivo
        try
        {
            string to,bcc, from, subject;
            Console.Write(mail);
            to = "";
            SqlDataReader sqlDataReader = Conexion.getSqlDataReader("select * from MailNotification;");
           
            if (sqlDataReader.HasRows)
            {
                List<String> mails = new List<string>();
                while (sqlDataReader.Read())
                {
                    mails.Add(sqlDataReader.GetValue(2).ToString());
                }
                bcc = String.Join(", ", mails);
            }
            else
            {
                bcc = "notificaciones@inolab.com";
            }
            Conexion.cerrarConexion();
            from = "notificaciones@inolab.com";
            subject = "FSR folio " + Session["folio_p"];
            MailMessage message = new MailMessage();
            message.Bcc.Add(bcc);
            message.From = new MailAddress(from);
            message.Body = PopulateBody(Session["folio_p"].ToString(),"cliente");
            message.IsBodyHtml = true;
            message.Subject = subject;

            Attachment attach = new Attachment(filepath);
            message.Attachments.Add(attach);

            SmtpClient client = new SmtpClient();
            client.Port = 1025;
            client.Host = "smtp.inolab.com";
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
            client.Send(message);
            message.Dispose();
            client.Dispose();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

    private string CreatePDF(string fileName)
    {
        //Realiza la creación del PDF 
        // Variables  
        Warning[] warnings;
        string[] streamIds;
        string mimeType = string.Empty;
        string encoding = string.Empty;
        string extension = string.Empty;

        //Setup the report viewer object and get the array of bytes 
        byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.  
        string filepath =  HttpRuntime.AppDomainAppPath + "Docs\\" + fileName + ".pdf";
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        using (FileStream fs = new FileStream(filepath, FileMode.Create))
        {
            fs.Write(bytes, 0, bytes.Length);
            Console.Write(fs.Name);
            fs.Dispose();
        }
        return filepath;
    }

    private string PopulateBody(string folio, string cliente)
    {
        //Realiza el replace en HTML para la creación del correo
        string body = string.Empty;
        
        using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index2.html")))
        {
            body = reader.ReadToEnd();
            reader.Dispose();
        }
        //Incrsion de datos para el HTML
        body = body.Replace("{folio}", folio);
        body = body.Replace("{cliente}", cliente);
        body = body.Replace("{slogan}", "data:image/png;base64," + convertirImagenAStringBase64(Server.MapPath("./Imagenes/slogan.png")));
       
        return body;
    }

    protected static string convertirImagenAStringBase64(string imgPath)
    {
        byte[] imageBytes = File.ReadAllBytes(imgPath);
        string base64String = Convert.ToBase64String(imageBytes);
        return base64String;
    }

    protected void firmaing_Click(object sender, EventArgs e)
    {
        Response.Redirect("FirmarFolio.aspx");
    }



    protected void Finalizar_folio_Click(object sender, EventArgs e)
    {
        if (datepicker.Text.ToString() == "")
        {
            Response.Write("<script>alert('Favor de seleccionar alguna fecha para la finalización del folio');</script>");
        }
        else
        {
            DateTime fechaYhoraFinDeFolio = getFechaYhoraDeFinDeFolio();
            DateTime fechaYhoraInicioServicio = getFechaYhoraInicioDeServicio();
            verificarQueFechaInicioDeFolioSeaMenorAfechaFinDeServicio(fechaYhoraInicioServicio, fechaYhoraFinDeFolio);

        }
    }

    public DateTime getFechaYhoraDeFinDeFolio()
    {
        string fechaDeFolio = Convert.ToDateTime(datepicker.Text.ToString()).ToString("yyyy-MM-dd");
        int hora = Convert.ToInt32(horafinal.SelectedItem.ToString());
        int minuto = Convert.ToInt32(minfinal.SelectedItem.ToString());

        string fechaCompletaYhoraDeCierrDeFolio = fechaDeFolio + " " + hora.ToString() + ":" + minuto.ToString();
        DateTime fechaCierreFolio;
        return  fechaCierreFolio = DateTime.Parse(fechaCompletaYhoraDeCierrDeFolio);
    }

    public DateTime getFechaYhoraInicioDeServicio()
    {
        DateTime fechaInicioDeServicio = verificarQueFechaInicioFolioSeaMenorAfechaWeb();
        DateTime fechaYhoraInicioServicio;
        return fechaYhoraInicioServicio = DateTime.Parse(fechaInicioDeServicio.ToString("yyyy-MM-dd HH:mm:ss.fff"));
    }

    private DateTime verificarQueFechaInicioFolioSeaMenorAfechaWeb()
    {
        try
        {
            string query = "select WebFechaIni from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";";
            DateTime fechaSolucion = Conexion.getDateTime(query);
            return fechaSolucion;
        }
        catch (Exception et)
        {

            string query = "select Inicio_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";";
            DateTime fecha_sol = Conexion.getDateTime(query);

            Conexion.executeQuery(" UPDATE FSR SET WebFechaIni=" +
                "CAST('" + fecha_sol.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            return fecha_sol;
        }

    }
   
    public void verificarQueFechaInicioDeFolioSeaMenorAfechaFinDeServicio(DateTime fechaInicioServicio, DateTime fechaFinServicio)
    {
        int resultadoComparacion = DateTime.Compare(fechaInicioServicio, fechaFinServicio);
        if (resultadoComparacion > 0)
        {
            Response.Write("<script>alert('La fecha de fin de folio no puede ser anterior a la fecha de inicio del folio');</script>");
        }
        else
        {
            Conexion.executeQuery(" UPDATE FSR SET  WebFechaFin='" + fechaFinServicio.ToString("yyyy-MM-dd HH:mm:ss.fff")
            + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            Response.Redirect("CargaFin.aspx");
        }
    }

   
}