using System;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Web.UI;
using System.Web;
using System.IO;
using static INOLAB_OC.DescargaFolio;
using INOLAB_OC.Modelo;
using System.Data;
using System.Linq.Expressions;

public partial class FSR : Page
{
    const string FINALIZADO = "3";
    const string ASIGNADO = "1";
    const string PROCESO = "2";
    const string SIN_SERVICIO_INICIADO ="";

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
            titulo.Text = "Información de Servicio con numero de folio N°. " +  Session["folio_p"].ToString();
            Nombre_de_usuario.Text = Session["nameUsuario"].ToString();
        }
        consultaDatosFolioServicio();

        if (Estatus_de_folio_servicio.Text.Equals(FINALIZADO))
        {
            Btn_Estatus_Servicio.Text = "Guardar Cambios";
            Btn_actualizar_fechas.Visible = true;
            Btn_agregar_acciones.Visible = true;
        }
        else
        {
            Btn_actualizar_fechas.Visible = false;
            Btn_agregar_acciones.Visible = false;
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

    public void consultaDatosFolioServicio()
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
            Estatus_de_folio_servicio.Text= informacionServicio["Estatusid"].ToString();
        

        //Si ya se inicio el servicio, el boton en ves de decor Iniciar, dira Continuar
       
        
        string inicioServicio = Conexion.getText("select top 1 Inicio_Servicio from FSR where Folio = " + Session["folio_p"].ToString() + " and Inicio_Servicio is not null;");

        if (inicioServicio != SIN_SERVICIO_INICIADO)
        {
            Btn_Estatus_Servicio.Text = "Continuar Servicio";
        }
        else if(inicioServicio.Equals(SIN_SERVICIO_INICIADO) || inicioServicio == null )
        {
            Btn_Estatus_Servicio.Text = "Iniciar Servicio";
        }
        
    }

    protected void Actualizar_Datos_Servicio_Click(object sender, EventArgs e)
    {
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

        verificarEstatusDeFolio(Estatus_de_folio_servicio.Text);
       
    }

    private void verificarEstatusDeFolio(string estatusDeFolio)
    {
        switch (estatusDeFolio)
        {
            case ASIGNADO:
                //Se abre ventana emergente para colocar 
                floatsection.Style.Add("display", "block");
                headerone.Style.Add("filter", "blur(9px)");
                cuerpo.Style.Add("display", "none");
                reportdiv.Style.Add("display", "none");
                break;

            case PROCESO:
                //Si esta en proceso puede ir a ver la parte de acciones realizadas (modificada)
                recreatePdfParaServicioFinalizado(Session["folio_p"].ToString());
                Response.Redirect("./DetalleFSR.aspx");
                break;
            case FINALIZADO:
                recreatePdfParaServicioFinalizado(Session["folio_p"].ToString());
                Response.Redirect("./FSR.aspx");
                break;

        }
            
    }
    protected void btndescarga_Click(object sender, EventArgs e)
    {
        //recreatePDF(Session["folio_p"].ToString());
    }

    protected void recreatePdfParaServicioFinalizado(string folio)
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

        crearReportePdf(nombre);
    }

    private void crearReportePdf(string nombre)
    {
     // Variables  
        Warning[] warnings;
        string[] streamIds;
        string mimeType = string.Empty;
        string encoding = string.Empty;
        string extension = string.Empty;

        //Setup the report viewer object and get the array of bytes
        byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        if (Estatus_de_folio_servicio.Text == "3")
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

    protected void Iniciar_Folio_De_Servicio_Click(object sender, EventArgs e)
    {
        //Proceso para iniciar el folio con la fechaWeb que seleccione el ingeniero de servicio
        if (Fecha_inicio_servicio.Text.ToString().Equals(""))
        {
            Response.Write("<script>alert('Favor de seleccionar alguna fecha para la el inicio del folio');</script>");
        }
        else
        {
           
            DateTime fechaYhoraDeInicioDeServicio = generarFechaYHoraDeInicioDeServicio();
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
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', WebFechaIni='" + fechaYhoraDeInicioDeServicio.ToString("yyyy - MM - dd HH: mm:ss.fff")
                    + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            try
            {
                
                string consulta = "Select ClgID FROM SCL5 where U_FSR = " + Session["folio_p"];
                int idFolioActividadSap = ConexionInolab.getScalar(consulta);
                ConexionInolab.executeQuery(" UPDATE OCLG SET tel = '" + Session["folio_p"] + " En Proceso' where ClgCode=" + idFolioActividadSap.ToString() + ";");
               
            }
            catch (Exception es)
            {
                Console.Write(es.ToString());
            }
            Response.Redirect("./DetalleFSR.aspx");
            
        }
    }

    public DateTime generarFechaYHoraDeInicioDeServicio()
    {
        string fechaInicioServicio = Convert.ToDateTime(Fecha_inicio_servicio.Text.ToString()).ToString("yyyy-MM-dd");
        int horaInicioServicio = Convert.ToInt32(horainicial.SelectedItem.ToString());
        int minutoInicioServicio = Convert.ToInt32(mininicial.SelectedItem.ToString());

        string fechaYhoraInicioServicio = fechaInicioServicio + " " + horaInicioServicio.ToString() + ":" + minutoInicioServicio.ToString();
        DateTime inicioDeServicio = DateTime.Parse(fechaYhoraInicioServicio);
        return inicioDeServicio;
    }

    protected void Actualizar_fechaYhora_Servicio_Click(object sender, EventArgs e)
    {
        DateTime fechaYhoraInicioDeFolio = verificarFechaYHoraDeInicioDeFolio();
        DateTime fechaYhoraCierreFolio = verificarFechaYHoraDeCierreDeFolio();

        datepicker2.Text = fechaYhoraInicioDeFolio.ToString("yyyy-MM-dd");
        datepicker3.Text = fechaYhoraCierreFolio.ToString("yyyy-MM-dd");

        Hora_inicio_folio.SelectedValue = fechaYhoraInicioDeFolio.ToString("HH");
        Minuto_inicio_folio.SelectedValue = fechaYhoraInicioDeFolio.ToString("mm");

        Hora_fin_folio.SelectedValue = fechaYhoraCierreFolio.ToString("HH");
        Minuto_fin_folio.SelectedValue = fechaYhoraCierreFolio.ToString("mm");

        //Se abre ventana emergente para colocar 
        Actfechas.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        cuerpo.Style.Add("display", "none");
        reportdiv.Style.Add("display", "none");
    }

    protected void Finalizar_Click(object sender, EventArgs e)
    {
    
        string fechaYhoraInicio = datepicker2.Text.ToString() + " " + Hora_inicio_folio.SelectedValue.ToString() + ":" + Minuto_inicio_folio.SelectedValue.ToString();
        DateTime fechaYhoraInicioFolioServicio = DateTime.Parse(fechaYhoraInicio);

        string fechaYhoraFin = datepicker3.Text.ToString() + " " + Hora_fin_folio.SelectedValue.ToString() + ":" + Minuto_fin_folio.SelectedValue.ToString();
        DateTime fechaYhoraFinFolioServicio = DateTime.Parse(fechaYhoraFin);

        Conexion.executeQuery(" UPDATE FSR SET WebFechaIni ='" + fechaYhoraInicioFolioServicio.ToString("yyyy - MM - dd HH: mm:ss.fff") +
            "' ,WebFechaFin='" + fechaYhoraFinFolioServicio.ToString("yyyy - MM - dd HH: mm:ss.fff") + "' where Folio=" + Session["folio_p"] +
            " and Id_Ingeniero =" + Session["idUsuario"] + ";");


        Actfechas.Style.Add("display", "none");
        headerone.Style.Add("filter", "blur(0px)");
        cuerpo.Style.Add("display", "block");
        reportdiv.Style.Add("display", "block");
    }


    private DateTime verificarFechaYHoraDeInicioDeFolio()
    {
        //Verifica si hay fecha de inicio de folio web o si hay fecha de inicio
        try
        {
            
            object fechaYHoraDeInicioColumna_WebFechaIni = Conexion.getObject("select WebFechaIni from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaYhoraDeInicioDeFolioServicio = (DateTime)fechaYHoraDeInicioColumna_WebFechaIni;

            lbl_fechaYhora_inicio_servicio.Text = fechaYhoraDeInicioDeFolioServicio.ToString();
            return fechaYhoraDeInicioDeFolioServicio;
        }
        catch (Exception es)
        {
            
            object fechaYHoraDeInicio = Conexion.getObject("select Inicio_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaYHoraDeInicioDeServicio = (DateTime)fechaYHoraDeInicio;

            lbl_fechaYhora_inicio_servicio.Text = fechaYHoraDeInicioDeServicio.ToString();
            return fechaYHoraDeInicioDeServicio;
        }
    }

    private DateTime verificarFechaYHoraDeCierreDeFolio()
    {
        
        try
        {
            object fechaYHoraFinColumna_WebFechaFin = Conexion.getObject("select WebFechaFin from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaYHoraFinDeFolioDeServicio = (DateTime)fechaYHoraFinColumna_WebFechaFin;

            Lbl_fin_de_servicio.Text = fechaYHoraFinDeFolioDeServicio.ToString();
            return fechaYHoraFinDeFolioDeServicio;
        }
        catch (Exception es)
        {
            object fechaYHoraFinColumna_Fin_Servicio = Conexion.getObject("select Fin_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            DateTime fechaYHoraFinDeFolioDeServicio = (DateTime)fechaYHoraFinColumna_Fin_Servicio;

            Lbl_fin_de_servicio.Text = fechaYHoraFinDeFolioDeServicio.ToString();
            return fechaYHoraFinDeFolioDeServicio;
        }
    }

    protected void Actualizar_Acciones_Click(object sender, EventArgs e)
    {
        Response.Redirect("./DetalleFSR.aspx");
    }

    protected void Adjuntar_Archivos_Click(object sender, EventArgs e)
    {
        Response.Redirect("./Adjuntar.aspx");
    }

    
}