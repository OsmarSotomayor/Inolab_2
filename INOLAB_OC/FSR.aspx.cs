using System;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Web.UI;
using System.Web;
using System.IO;
using static INOLAB_OC.DescargaFolio;
using INOLAB_OC.Modelo;
using System.Data;

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

    public void consultadatos()
    {
        

           string query = "select * from v_fsr where idingeniero = " + Session["Idusuario"] + " and Folio = " + Session["folio_p"] + "; ";
           DataRow informacionServicio = Conexion.getDataRow(query);
        
            //Inserta los datos de la vista coorepondientemente al campo que se le es asignado dentro del .aspx
            txtfolio.Text= informacionServicio["Cliente"].ToString();
            txttelfax.Text= informacionServicio["Telefono"].ToString();
            txtdireccion.Text= informacionServicio["Direccion"].ToString();
            txtlocalidad.Text= informacionServicio["Localidad"].ToString();
            txtdepto.Text= informacionServicio["Departamento"].ToString();
            txtnresponsable.Text= informacionServicio["N_Responsable"].ToString();
            txtreportadopor.Text= informacionServicio["N_Reportado"].ToString();
            txtemail.Text = informacionServicio["Mail"].ToString();
            txtdescripcion.Text= informacionServicio["Equipo"].ToString();
            txtmarca.Text= informacionServicio["Marca"].ToString();
            txtmodelo.Text= informacionServicio["Modelo"].ToString();
            txtnoserie.Text= informacionServicio["NoSerie"].ToString();
            txtid.Text= informacionServicio["IdEquipo_C"].ToString();
            
            idservicio.SelectedValue = informacionServicio["idservicio"].ToString();
            idcontrato.SelectedValue = informacionServicio["idcontrato"].ToString();
            idproblema.SelectedValue = informacionServicio["idproblema"].ToString();

            datepicker.Text = informacionServicio["FechaServicio"].ToString();
            DropDownList7.SelectedValue = informacionServicio["HoraServicio"].ToString();
            cmding.Text = informacionServicio["IdIngeniero"].ToString();
            DropDownList8.Text = informacionServicio["Estatusid"].ToString();
            labelestado.Text= informacionServicio["Estatusid"].ToString();
        

        //Si ya se inicio el servicio, el boton en ves de decor Iniciar, dira Continuar
       
        
        int result = Conexion.getScalar("select top 1 Inicio_Servicio from FSR where Folio = " + Session["folio_p"].ToString() + " and Inicio_Servicio is not null;");
        
        if (result != -1)
        {
            Button1.Text = "Continuar Servicio";
        }
        else
        {
            Button1.Text = "Iniciar Servicio";
        }
        
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
                
                Conexion.executeQuery(" UPDATE FSR SET Marca='" + marca + "', Modelo='" + modelo + "', NoSerie='" + noserie +
                    "', Equipo='" + descripcion + "', IdEquipo_C='" + id + "',Cliente='" + cliente + "',Telefono='" + telefono +
                    "',N_Responsable='" + responsable + "',N_Reportado='" + reportado + "',Mail='" + email + "',Direccion='" + direccion +
                    "',Localidad='" + localidad + "',Depto='" + depto + "',IdT_Problema='" + idproblema + "',IdT_Contrato='" + idcontrato +
                    "', IdT_Servicio='" + idservicio + "'" + " where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" +
                    Session["idUsuario"] + ";");
               
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                
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
            
            object fechaServicio = Conexion.getObject("select FechaServicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            DateTime fecha_sol = (DateTime)fechaServicio;           
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

                //update a base de datos y redireccionamient
                Conexion.executeQuery(" UPDATE FSR SET idStatus = '2' ,Inicio_Servicio='" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', WebFechaIni='" + DateObject.ToString("yyyy - MM - dd HH: mm:ss.fff")
                    + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            try
            {
                //Conseguir la clave CLGcode de el folio
                string consulta = "Select ClgID FROM SCL5 where U_FSR = " + Session["folio_p"];
                int clg = ConexionInolab.getScalar(consulta);

                //Se hace el update de la concatenacion de Folio y Estatus
                ConexionInolab.executeQuery(" UPDATE OCLG SET tel = '" + Session["folio_p"] + " En Proceso' where ClgCode=" + clg.ToString() + ";");
               
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

        Conexion.executeQuery(" UPDATE FSR SET WebFechaIni ='" + DateObject.ToString("yyyy - MM - dd HH: mm:ss.fff") +
            "' ,WebFechaFin='" + DateObject1.ToString("yyyy - MM - dd HH: mm:ss.fff") + "' where Folio=" + Session["folio_p"] +
            " and Id_Ingeniero =" + Session["idUsuario"] + ";");


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
            
            object fechaActual1 = Conexion.getObject("select WebFechaIni from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaActualFormatoDateTime = (DateTime)fechaActual1;
            
            actual1.Text = fechaActualFormatoDateTime.ToString();
            return fechaActualFormatoDateTime;
        }
        catch (Exception es)
        {
            
            object fechaActual = Conexion.getObject("select Inicio_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaActualFormatoDateTime = (DateTime)fechaActual;
            
            actual1.Text = fechaActualFormatoDateTime.ToString();
            return fechaActualFormatoDateTime;
        }
    }

    private DateTime verfechas2()
    {
        //Verifica si hay fecha de finalizacion de folio web o si hay fecha de finalizacion 
        try
        {
            
            object fechaFinServicio = Conexion.getObject("select WebFechaFin from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaFinServicioFormatoDateTime = (DateTime)fechaFinServicio;

            actual2.Text = fechaFinServicioFormatoDateTime.ToString();
            return fechaFinServicioFormatoDateTime;
        }
        catch (Exception es)
        {
            
            object fechaFinServicio = Conexion.getObject("select Fin_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaFinServicioFormatoDateTime = (DateTime)fechaFinServicio;
         
            actual2.Text = fechaFinServicio.ToString();
            return fechaFinServicioFormatoDateTime;
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