using System;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Web.UI;
using System.Web;
using System.IO;
using static INOLAB_OC.DescargaFolio;

public partial class FSR : Page
{

    protected void Page_Init(object sender, EventArgs e)
    {
        //test dos
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["idUsuario"] == null)
        {
            Response.Redirect("./Sesion.aspx");
        }
        else
        {
            //Codigo para que aparezca el titulo, sin error de repeticion del numero del folio
            titulo.Text = "Información de FSR N°. " +  Session["folio_p"].ToString();
            lbluser.Text = Session["nameUsuario"].ToString();
        }
        consultadatos();

        if (labelestado.Text == "3")
        {
            //Guarda los cambios que se hayan hecho en caso de estar el folio finalizado
            Button1.Text = "Guardar Cambios";
            btnfechas.Visible = true;
            btnacciones.Visible = true;
        }
        else
        {
            btnfechas.Visible = false;
            btnacciones.Visible = false;
        }
        if (idservicio.SelectedValue.ToString() =="4" || idservicio.SelectedValue.ToString()== "8" || 
            idservicio.SelectedValue.ToString() == "9")
        {
            //En ciertos tipos de servicio, no se podra hacer el cambio de su estado debido a que no deben de poder cambiarlo libremente los ingenieros
            idservicio.Enabled = false;
        }
        else
        {
            idservicio.Enabled = true;
        }
        int usuario;
        usuario =Convert.ToInt32(Session["idUsuario"]);

        if(usuario==71)
        {
            btnadjuntar.Visible = true;
        }
        else
        {
            btnadjuntar.Visible = false;
        }

    }

    //Conexion a la base de datos (para hacer prebas acceder a BrowserPruebas y Test)
    SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");
    SqlConnection con2 = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Inolab;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

    public void consultadatos()
    {
        //Consulta los datos que se encuentran en la vista de el folio
        SqlCommand cmd=new SqlCommand("select * from v_fsr where idingeniero = " + Session["Idusuario"] + " and Folio = " + Session["folio_p"] + "; ", con);
        con.Open();

        SqlDataReader leer;
        leer = cmd.ExecuteReader();
        if (leer.Read())
        {
            //Inserta los datos de la vista coorepondientemente al campo que se le es asignado dentro del .aspx
            txtfolio.Text=leer["Cliente"].ToString();
            txttelfax.Text=leer["Telefono"].ToString();
            txtdireccion.Text= leer["Direccion"].ToString();
            txtlocalidad.Text=leer["Localidad"].ToString();
            txtdepto.Text=leer["Departamento"].ToString();
            txtnresponsable.Text=leer["N_Responsable"].ToString();
            txtreportadopor.Text=leer["N_Reportado"].ToString();
            txtemail.Text = leer["Mail"].ToString();
            txtdescripcion.Text=leer["Equipo"].ToString();
            txtmarca.Text=leer["Marca"].ToString();
            txtmodelo.Text=leer["Modelo"].ToString();
            txtnoserie.Text=leer["NoSerie"].ToString();
            txtid.Text=leer["IdEquipo_C"].ToString();
            
            idservicio.SelectedValue = leer["idservicio"].ToString();
            idcontrato.SelectedValue = leer["idcontrato"].ToString();
            idproblema.SelectedValue = leer["idproblema"].ToString();

            datepicker.Text = leer["FechaServicio"].ToString();
            DropDownList7.SelectedValue = leer["HoraServicio"].ToString();
            cmding.Text = leer["IdIngeniero"].ToString();
            DropDownList8.Text = leer["Estatusid"].ToString();
            labelestado.Text= leer["Estatusid"].ToString();
        }
        con.Close();

        //Si ya se inicio el servicio, el boton en ves de decor Iniciar, dira Continuar
        con.Open();
        SqlCommand verificarhora = new SqlCommand("select top 1 Inicio_Servicio from FSR where Folio = " + Session["folio_p"].ToString() + " and Inicio_Servicio is not null;", con);
        Object result = verificarhora.ExecuteScalar();
        
        if (result != null)
        {
            Button1.Text = "Continuar Servicio";
        }
        else
        {
            Button1.Text = "Iniciar Servicio";
        }
        con.Close();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {//Obtiene la información de los campos MARCA, MODELO, NOSERIE, EQUIPO y IDEQUIPO para actualizarla en la base de datos 
        try
            {
                string marca = marcaHF.Value;
                string modelo = modeloHF.Value;
                string noserie = noserieHF.Value;
                string descripcion = descripcionHF.Value;
                string id = idHF.Value;

                string icontrato = tcontratoHF.Value;
                string iproblema = tproblemaHF.Value;
                string iservicio = tservicioHF.Value;

                //Transformacion de los valores asignados a enteros para su actualizacion en la base de datos
                int idcontrato = int.Parse(icontrato);
                int idproblema = int.Parse(iproblema);
                int idservicio = int.Parse(iservicio);


                string direccion = direccionHF.Value;
                string cliente = clienteHF.Value;
                string depto = deptoHF.Value;
                string localidad = localidadHF.Value;
                string telefono = TelefonoHF.Value;
                string responsable = responsableHF.Value;
                string reportado = reportadoHF.Value;
                string email = emailHF.Value;

                //se hace la actualizacion de los datos dentro de los campos asignados
                con.Open();
                SqlCommand comando = new SqlCommand(" UPDATE FSR SET Marca='" + marca + "', Modelo='" + modelo + "', NoSerie='" + noserie + 
                    "', Equipo='" + descripcion + "', IdEquipo_C='" + id + "',Cliente='" + cliente + "',Telefono='" + telefono + 
                    "',N_Responsable='" + responsable + "',N_Reportado='" + reportado + "',Mail='" + email + "',Direccion='" + direccion + 
                    "',Localidad='" + localidad + "',Depto='" + depto + "',IdT_Problema='" + idproblema + "',IdT_Contrato='" + idcontrato + 
                    "', IdT_Servicio='" + idservicio + "'" + " where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + 
                    Session["idUsuario"] + ";", con);
                comando.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                con.Close();
            }

        //Si el folio esta finalizado, sale a /FSR
        if (labelestado.Text == "3") {
            recreatePDF(Session["folio_p"].ToString());
            Response.Redirect("./FSR.aspx");

        }
        //Si esta en proceso puede ir a ver la parte de acciones realizadas (modificada)
        else if (labelestado.Text == "2")
        {
            recreatePDF(Session["folio_p"].ToString());
            Response.Redirect("./DetalleFSR.aspx");
        }
        //Si esta asignado, ve la parte de acciones realizadas
        else if (labelestado.Text == "1")
        {
            //Se abre ventana emergente para colocar 
            floatsection.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            cuerpo.Style.Add("display", "none");
            reportdiv.Style.Add("display", "none");
        }
    }

    protected void btndescarga_Click(object sender, EventArgs e)
    {
        //recreatePDF(Session["folio_p"].ToString());
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

        //Setup the report viewer object and get the array of bytes
        byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        if (labelestado.Text == "3")
        {
            //aqui iria el codigo para guardar en //Docs debido a que es cuando se renderiza el pdf 
            string nombre_archivo = Session["folio_p"].ToString();
            string fecha_archivo = DateTime.Now.ToString("dd-MM-yyyy HH_mm");
            //El nombre del archivo al ser una actualizacion, llevara la fecha y hora a al que se subio la actualizacion
            string filepath = HttpRuntime.AppDomainAppPath + "Docs\\" + nombre_archivo + " " + fecha_archivo + ".pdf";

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                Console.Write(fs.Name);
                fs.Dispose();
            }
        }
    }

    protected void Iniciar_Click(object sender, EventArgs e)
    {
        //Proceso para iniciar el folio con la fechaWeb que seleccione el ingeniero de servicio
        if (datepicker1.Text.ToString() == "")
        {
            Response.Write("<script>alert('Favor de seleccionar alguna fecha para la el inicio del folio');</script>");
        }
        else
        {
            //Juntar las fechas
            string fecha = Convert.ToDateTime(datepicker1.Text.ToString()).ToString("yyyy-MM-dd");
            int hora = Convert.ToInt32(horainicial.SelectedItem.ToString());
            int minuto = Convert.ToInt32(mininicial.SelectedItem.ToString());

            string fechacompleta = fecha + " " + hora.ToString() + ":" + minuto.ToString();
            DateTime DateObject = DateTime.Parse(fechacompleta);

            //Checa que sea la fecha del servicio
            con.Open();
            SqlCommand comando2 = new SqlCommand("select FechaServicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object fchsol = comando2.ExecuteScalar();
            DateTime fecha_sol = (DateTime)fchsol;           
            con.Close();
            DateTime DateObject2 = DateTime.Parse(fecha_sol.ToString("yyyy-MM-dd"));

            //Validacion de si fecha web es despues de fecha servicio
            //int result = DateTime.Compare(DateObject2, DateObject);
            //if (result >0)
            //{
            //    Response.Write("<script>alert('La fecha de inicio de Folio no puede ser anterior a la fecha de solicitud de servicio');</script>");
            //}
            //else
            //{
                //Fecha se solicitud anterior o igual a la fecha de inicio

                //update a base de datos y redireccionamiento
                con.Open();
                SqlCommand comando = new SqlCommand(" UPDATE FSR SET idStatus = '2' ,Inicio_Servicio='" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', WebFechaIni='" + DateObject.ToString("yyyy - MM - dd HH: mm:ss.fff")
                    + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
                comando.ExecuteNonQuery();
                con.Close();
            try
            {
                //Conseguir la clave CLGcode de el folio
                con2.Open();
                string consulta = "Select ClgID FROM SCL5 where U_FSR = " + Session["folio_p"];
                SqlCommand clgcode = new SqlCommand(consulta, con2);
                int clg = (int)clgcode.ExecuteScalar();
                con2.Close();

                //Se hace el update de la concatenacion de Folio y Estatus
                con2.Open();
                SqlCommand sap = new SqlCommand(" UPDATE OCLG SET tel = '" + Session["folio_p"] + " En Proceso' where ClgCode=" + clg.ToString() + ";", con2);
                sap.ExecuteNonQuery();
                con2.Close();
            }
            catch (Exception es)
            {
                Console.Write(es.ToString());
            }
            Response.Redirect("./DetalleFSR.aspx");
            //}
        }
    }

    protected void btnfechas_Click(object sender, EventArgs e)
    {
        //Se hace la busqueda de las fechas web actuales
        DateTime fecha_actual1 = verfechas();
        DateTime fecha_actual2 = verfechas2();

        datepicker2.Text = fecha_actual1.ToString("yyyy-MM-dd");
        datepicker3.Text = fecha_actual2.ToString("yyyy-MM-dd");

        horafinal.SelectedValue = fecha_actual1.ToString("HH");
        minfinal.SelectedValue = fecha_actual1.ToString("mm");

        horafs.SelectedValue = fecha_actual2.ToString("HH");
        horais.SelectedValue = fecha_actual2.ToString("mm");

        //Se abre ventana emergente para colocar 
        Actfechas.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        cuerpo.Style.Add("display", "none");
        reportdiv.Style.Add("display", "none");
    }

    protected void Finalizar_Click(object sender, EventArgs e)
    {
        //Guarda la fecha que se le asigna a la parte de fecha inicial web
        string fechaini = datepicker2.Text.ToString() + " " + horafinal.SelectedValue.ToString() + ":" + minfinal.SelectedValue.ToString();
        DateTime DateObject = DateTime.Parse(fechaini);

        string fechafin = datepicker3.Text.ToString() + " " + horafs.SelectedValue.ToString() + ":" + horais.SelectedValue.ToString();
        DateTime DateObject1 = DateTime.Parse(fechafin);

        con.Open();
        SqlCommand comando = new SqlCommand(" UPDATE FSR SET WebFechaIni ='" + DateObject.ToString("yyyy - MM - dd HH: mm:ss.fff") + 
            "' ,WebFechaFin='" + DateObject1.ToString("yyyy - MM - dd HH: mm:ss.fff") + "' where Folio=" + Session["folio_p"] + 
            " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
        comando.ExecuteNonQuery();
        con.Close();

        Actfechas.Style.Add("display", "none");
        headerone.Style.Add("filter", "blur(0px)");
        cuerpo.Style.Add("display", "block");
        reportdiv.Style.Add("display", "block");
    }


    private DateTime verfechas()
    {
        //Verifica si hay fecha de inicio de folio web o si hay fecha de inicio
        try
        {
            con.Open();
            SqlCommand act1 = new SqlCommand("select WebFechaIni from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object factual1 = act1.ExecuteScalar();
            DateTime fecha_actual1 = (DateTime)factual1;
            con.Close();
            actual1.Text = fecha_actual1.ToString();

            return fecha_actual1;
        }
        catch (Exception es)
        {
            con.Close();
            con.Open();
            SqlCommand act1 = new SqlCommand("select Inicio_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object factual1 = act1.ExecuteScalar();
            DateTime fecha_actual1 = (DateTime)factual1;
            con.Close();
            actual1.Text = fecha_actual1.ToString();

            return fecha_actual1;
        }
    }

    private DateTime verfechas2()
    {
        //Verifica si hay fecha de finalizacion de folio web o si hay fecha de finalizacion 
        try
        {
            con.Open();
            SqlCommand act2 = new SqlCommand("select WebFechaFin from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object factual2 = act2.ExecuteScalar();
            DateTime fecha_actual2 = (DateTime)factual2;
            con.Close();
            actual2.Text = fecha_actual2.ToString();

            return fecha_actual2;
        }
        catch (Exception es)
        {
            con.Close();
            con.Open();
            SqlCommand act2 = new SqlCommand("select Fin_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object factual2 = act2.ExecuteScalar();
            DateTime fecha_actual2 = (DateTime)factual2;
            con.Close();
            actual2.Text = fecha_actual2.ToString();

            return fecha_actual2;
        }
    }

    protected void btnacciones_Click(object sender, EventArgs e)
    {
        //Va a la ventana de detalleFSR
        Response.Redirect("./DetalleFSR.aspx");
    }

    protected void btnadjuntar_Click(object sender, EventArgs e)
    {
        Response.Redirect("./Adjuntar.aspx");
    }

    protected void datepicker_TextChanged(object sender, EventArgs e)
    {

    }
}