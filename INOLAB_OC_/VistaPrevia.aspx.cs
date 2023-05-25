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

public partial class VistaPrevia : Page
{
    protected void Page_Load(object sender, EventArgs e)
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

    //Conexion a base de datos (Para la base de datos de pruebas cambiar a BrowserPruebas y a Test)
    SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");
    SqlConnection con2 = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Inolab;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");
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

    protected void firmarbtn_Click(object sender, EventArgs e)
    {
        //Muestra la pantalla para firrmar el documento que le es requerido
        avisopriv.Style.Add("display", "block");
        headerid.Style.Add("display", "none");
        sectionreport.Style.Add("display", "none");
        footerid.Style.Add("display", "none");
    }

    protected void AvisoPriv(object sender, EventArgs e)
    {
        //Muestra la ventana de la firma del usuario
        avisopriv.Style.Add("display", "none");
        ulfol.Text = Session["folio_p"].ToString();
        ul_advert.Style.Add("display", "block");
    }

    protected void Uladver(object sender, EventArgs e)
    {
        //Muestra la ventana de la firma del usuario
        ul_advert.Style.Add("display", "none");
        firma.Style.Add("display", "block");
        string script = "startFirma();";
        ClientScript.RegisterStartupScript(GetType(), "Star", script, true);
    }

    protected void hidebutton_Click(object sender, EventArgs e)
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
        if(insertFirma(nombre, image))
        {
            updateFSR(nombre);
            ReportViewer1.ServerReport.Refresh();
        }
    }

    protected void updateFSR(string nombre)
    {
        //Actualiza el nombre del cliente y la fecha en la que el cliente realiza la firma 
        try
        {
            con.Open();
            SqlCommand comando = new SqlCommand(" UPDATE FSR SET NombreCliente='" + nombre + "', FechaFirmaCliente=" +
                "CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            comando.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
    }

    protected bool insertFirma(string nombre, string image)
    {
        //Actualiza la firma actual en el documento y le indica cual es la firma actual 
        try
        {
            string[] images = image.Split(',');
            string pattern = @"[^:\s*]\w+\/[\w-+\d.]+(?=[;| ])";
            string mimetype = "";
            string img1 = images[0];
            string img2 = images[1];
            Regex rx = new Regex(pattern);
            Match m = rx.Match(img1);
            if (m.Success)
                mimetype = m.Value;
            con.Open();
            SqlCommand firma = new SqlCommand("Insert into FirmaImg(ImageName,MimeType,ImageBits)" +
                " values(@nombre,@mime,@image);" +
                "SELECT CAST(scope_identity() AS int)", con);
            firma.Parameters.Add("@nombre", SqlDbType.VarChar);
            firma.Parameters.Add("@mime", SqlDbType.VarChar);
            firma.Parameters.Add("@image", SqlDbType.VarBinary);
            firma.Parameters["@nombre"].Value = nombre;
            firma.Parameters["@mime"].Value = mimetype;
            firma.Parameters["@image"].Value = Convert.FromBase64String(img2);
            int c = (int)firma.ExecuteScalar();
            if (c != 0)
            {
                SqlCommand datofirma = new SqlCommand("update FSR set IdFirmaImg=" + c + " where Folio=" + Session["folio_p"] + ";", con);
                datofirma.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            con.Close();
            return false;
        }
    }

    protected void finalizarbtn_Click(object sender, EventArgs e)
    {
        //Validacion de que se cuenta con las firmas de el ingeniero y del cliente
        int fir_us = 0;
        int fir_ing = 0;

        //Proceso para identificar si hay regristros de la firma del cliente en este folio 
        try
        {
            con.Open();
            SqlCommand firma = new SqlCommand("select IdFirmaImg from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object firmaus = firma.ExecuteScalar();
            fir_us = (int)firmaus;
            con.Close();
        }
        catch (Exception es)
        {
            Response.Write("<script>alert('Falta firma de cliente');</script>");
        }

        //Proceso para identificar si hay registros de la firma del ingeniero en este folio
        try
        {
            con.Open();
            SqlCommand firma2 = new SqlCommand("select IDFirmaIng from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object firmaing = firma2.ExecuteScalar();
            fir_ing = (int)firmaing;
            con.Close();
        }
        catch (Exception er)
        {
            Response.Write("<script>alert('Falta firma de Ingeniero');</script>");
        }

        if (fir_us == 0 || fir_ing == 0)
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

    private void ActStatus()
    {
        try
        {
            //Actualizacion a estado de cierre de actividad de SAP
            con2.Open();
            SqlCommand updatestatus2 = new SqlCommand("Update OCLG set OCLG.status = -3, OCLG.Closed = 'Y', OCLG.CloseDate =CAST('" +
                DateTime.Now.ToString("yyyy-MM-dd") + "' AS DATETIME) from OCLG INNER JOIN SCL5 ON OCLG.ClgCode=SCL5.ClgID where SCL5.U_FSR ='" + Session["folio_p"].ToString() + "'", con2);
            updatestatus2.ExecuteNonQuery();
            con2.Close();

            con2.Open();
            SqlCommand updatestatus3 = new SqlCommand("Update SCL5 set U_ESTATUS = 'Finalizado' where U_FSR ='" + Session["folio_p"].ToString() + "'", con2);
            updatestatus3.ExecuteNonQuery();
            con2.Close();


            //Buscar el callid
            con2.Open();
            string query = "Select SrvcCallId FROM SCL5 where U_FSR ='" + Session["folio_p"].ToString() + "'";
            SqlCommand command = new SqlCommand(query, con2);
            string resultado = Convert.ToString(command.ExecuteScalar());
            //Me da el Callid 
            con2.Close();

            con2.Open();
            string query2 = "Select count (DISTINCT U_ESTATUS) FROM SCL5 where SrvcCallId = " + resultado.ToString();
            SqlCommand command2 = new SqlCommand(query2, con2);
            string resultado2 = Convert.ToString(command2.ExecuteScalar());
            //resultado2 el numero de valores que hay en estatus (Para que se cierre la llamada debe de ser "-1")
            con2.Close();

            //Hacer un ciclo while para identificar nulos? (usar visorder para pasar por todos)
            con2.Open();
            string queryc= "Select count(*) FROM SCL5 where SrvcCallId = " + resultado.ToString();
            SqlCommand commandc = new SqlCommand(queryc, con2);
            string resultadoc = Convert.ToString(commandc.ExecuteScalar());
            con2.Close();

            bool nulo = false;

            for (int i = 1; i <= Convert.ToInt32(resultadoc); i++)
            {
                //Comprueba si hy m folios con distintos estatus en la tabla de la llamada en SAP
                con2.Open();
                string queryt = "Select U_ESTATUS FROM SCL5 where SrvcCallId = " + resultado.ToString() + 
                    "and VisOrder = " + i.ToString();
                SqlCommand commandt = new SqlCommand(queryt, con2);
                string resultadot = Convert.ToString(commandt.ExecuteScalar());
                if (resultadot != "Finalizado")
                {
                    nulo = true;
                }
                con2.Close();
            }

            if (resultado2 == "1" && nulo == false)
            {
                //En caso de que todos los registros correspondientes a la llamda esten con estatus de finalizado, finaliza la llamada
                con2.Open();
                SqlCommand updatestatus4 = new SqlCommand("Update OSCL set status = -1 where callID=" + resultado.ToString(), con2);
                updatestatus4.ExecuteNonQuery();
                con2.Close();
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
            con.Open();
            SqlCommand getemails = new SqlCommand("select * from MailNotification;", con);
            SqlDataReader sqldr = getemails.ExecuteReader();
           
            if (sqldr.HasRows)
            {
                List<String> mails = new List<string>();
                while (sqldr.Read())
                {
                    mails.Add(sqldr.GetValue(2).ToString());
                }
                bcc = String.Join(", ", mails);
            }
            else
            {
                bcc = "notificaciones@inolab.com";
            }

            con.Close();
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
        body = body.Replace("{slogan}", "data:image/png;base64," + GetBase64StringForImage(Server.MapPath("./Imagenes/slogan.png")));
       
        return body;
    }

    protected static string GetBase64StringForImage(string imgPath)
    {
        //Convierte la imagen en base64
        byte[] imageBytes = File.ReadAllBytes(imgPath);
        string base64String = Convert.ToBase64String(imageBytes);
        return base64String;
    }

    protected void firmaing_Click(object sender, EventArgs e)
    {
        Response.Redirect("FirmarFolio.aspx");
    }

    private void SendMail2(string mail)
    {
        //Envía el correo electrónico con la información del FSR y adjunto el archivo
        try
        {
            //Asunto de el correo
            string subject = "Notificación de observaciones Folio: " + Session["folio_p"].ToString();
            
            //Datos referentes a el envio del correo, anexandome a una copia oculta para monitorear que se este realizando el correcto envio de los correos
            MailAddress from = new MailAddress("notificaciones@inolab.com");
            MailAddress to = new MailAddress(mail);
            MailMessage message2 = new MailMessage(from, to);
            message2.Bcc.Add("luisrosales@inolab.com");
            message2.Body = PopulateBody2(Session["folio_p"].ToString());
            message2.IsBodyHtml = true;
            message2.Subject = subject;

            //Datos referentes a el transmisor del correo para su correcto envio
            SmtpClient client = new SmtpClient();
            client.Port = 1025;
            client.Host = "smtp.inolab.com";
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
            client.Send(message2);
            message2.Dispose();
            client.Dispose();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

    private string PopulateBody2(string folio)
    {
        //Realiza el replace en HTML para la creación del correo
        string body = string.Empty;

        using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index_not_ase.html")))
        {
            body = reader.ReadToEnd();
            reader.Dispose();
        }

        //Consulta de todos los datos que se necesitan para llenar la informacion que tendra el correo
        con.Open();
        string query = "Select top (1) Observaciones FROM FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand obs = new SqlCommand(query, con);
        string observ = Convert.ToString(obs.ExecuteScalar());
        con.Close();

        con2.Open();
        string query2 = "Select top (1) SrvcCallId FROM SCL5 where U_FSR=" + Session["folio_p"].ToString();
        SqlCommand call = new SqlCommand(query2, con2);
        string llamada = Convert.ToString(call.ExecuteScalar());
        con2.Close();

        con.Open();
        string query3 = "Select top (1) Cliente FROM FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand clie = new SqlCommand(query3, con);
        string cliente = Convert.ToString(clie.ExecuteScalar());
        con.Close();

        con.Open();
        string query4 = "Select top (1) Equipo FROM FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand equi = new SqlCommand(query4, con);
        string equipo = Convert.ToString(equi.ExecuteScalar());
        con.Close();

        con.Open();
        string query5= "Select top (1) TipoServicio FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand serv = new SqlCommand(query5, con);
        string servicio = Convert.ToString(serv.ExecuteScalar());
        con.Close();

        con.Open();
        string query6 = "Select top (1) Ingeniero FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand ing = new SqlCommand(query6, con);
        string ingeniero = Convert.ToString(ing.ExecuteScalar());
        con.Close();

        con.Open();
        string query7 = "Select top (1) Actividad FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand act = new SqlCommand(query7, con);
        string actividad = Convert.ToString(act.ExecuteScalar());
        con.Close();

        con.Open();
        string query8 = "Select top (1) OC FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand orden = new SqlCommand(query8, con);
        string OrdenVenta = Convert.ToString(orden.ExecuteScalar());
        con.Close();

        //Incersion de datos para el HTML
        body = body.Replace("{folio}", folio);
        body = body.Replace("{slogan}", "data:image/png;base64," + GetBase64StringForImage(Server.MapPath("./Imagenes/slogan.png")));
        body = body.Replace("{observaciones}", observ);
        body = body.Replace("{n_llamada}", llamada);
        body = body.Replace("{act_iv}", actividad);
        body = body.Replace("{OrdenVenta}", OrdenVenta);
        body = body.Replace("{cliente}", cliente);
        body = body.Replace("{equipo}", equipo);
        body = body.Replace("{servicio}", servicio);
        body = body.Replace("{ingeniero}", ingeniero);
        return body;
    }

    private void SendMail3(string filepath)
    {
        //Envía el correo electrónico con la información del FSR y adjunto el archivo
        try
        {
            //Asunto de el correo
            string subject = "Servicio Terminado. Folio: " + Session["folio_p"].ToString();

            //Datos referentes a el envio del correo, anexandome a una copia oculta para monitorear que se este realizando el correcto envio de los correos
            MailAddress from = new MailAddress("notificaciones@inolab.com");
            MailAddress to = new MailAddress("facturacion@inolab.com");
            MailMessage message3 = new MailMessage(from, to);
            message3.Bcc.Add("luisrosales@inolab.com");
            //message3.Bcc.Add("luisrosales@inolab.com" + "ivivar@inolab.com");
            message3.Body = PopulateBody3(Session["folio_p"].ToString());
            message3.IsBodyHtml = true;
            message3.Subject = subject;

            //Se inserta el folio correspondiente para mandarlo como archivo dentro del correo
            Attachment attach = new Attachment(filepath);
            message3.Attachments.Add(attach);

            //Datos referentes a el transmisor del correo para su correcto envio
            SmtpClient client = new SmtpClient();
            client.Port = 1025;
            client.Host = "smtp.inolab.com";
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
            client.Send(message3);
            message3.Dispose();
            client.Dispose();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

    private string PopulateBody3(string folio)
    {
        //Realiza el replace en HTML para la creación del correo
        string body = string.Empty;

        using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index_not_fact.html")))
        {
            body = reader.ReadToEnd();
            reader.Dispose();
        }

        //Consulta de todos los datos que se necesitan para llenar la informacion que tendra el correo
        con2.Open();
        string query2 = "Select top (1) SrvcCallId FROM SCL5 where U_FSR=" + Session["folio_p"].ToString();
        SqlCommand call = new SqlCommand(query2, con2);
        string llamada = Convert.ToString(call.ExecuteScalar());
        con2.Close();

        con.Open();
        string query3 = "Select top (1) Cliente FROM FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand clie = new SqlCommand(query3, con);
        string cliente = Convert.ToString(clie.ExecuteScalar());
        con.Close();

        con.Open();
        string query7 = "Select top (1) Actividad FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand act = new SqlCommand(query7, con);
        string actividad = Convert.ToString(act.ExecuteScalar());
        con.Close();

        con.Open();
        string query8 = "Select top (1) OC FROM V_FSR where Folio=" + Session["folio_p"].ToString();
        SqlCommand orden = new SqlCommand(query8, con);
        string OrdenVenta = Convert.ToString(orden.ExecuteScalar());
        con.Close();

        //Incersion de los datos que requiere el HTML 
        body = body.Replace("{folio}", folio);
        body = body.Replace("{slogan}", "data:image/png;base64," + GetBase64StringForImage(Server.MapPath("./Imagenes/slogan.png")));
        body = body.Replace("{n_llamada}", llamada);
        body = body.Replace("{act_iv}", actividad);
        body = body.Replace("{OrdenVenta}", OrdenVenta);
        body = body.Replace("{cliente}", cliente);
        return body;
    }

    protected void Finalizar_Click(object sender, EventArgs e)
    {
        //Se hace la finalizacion del folio con la fecha que haya seleccoinado el ingeniero
        if (datepicker.Text.ToString() == "")
        {
            Response.Write("<script>alert('Favor de seleccionar alguna fecha para la finalización del folio');</script>");
        }
        else
        {
            //Juntar las fechas
            string fecha = Convert.ToDateTime(datepicker.Text.ToString()).ToString("yyyy-MM-dd");
            int hora = Convert.ToInt32(horafinal.SelectedItem.ToString());
            int minuto = Convert.ToInt32(minfinal.SelectedItem.ToString());

            string fechacompleta = fecha + " " + hora.ToString() + ":" + minuto.ToString();
            DateTime DateObject = DateTime.Parse(fechacompleta);

            DateTime fecha_sol = ValEstatus();

            DateTime DateObject2 = DateTime.Parse(fecha_sol.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            
            //Comparacion de fecha con fecha de inicio de folio
            int result = DateTime.Compare(DateObject2, DateObject);
            if (result > 0)
            {
                Response.Write("<script>alert('La fecha de fin de folio no puede ser anterior a la fecha de inicio del folio');</script>");
            }
            else
            {
                //update a base de datos y redireccionamiento
                con.Open();
            SqlCommand comando = new SqlCommand(" UPDATE FSR SET  WebFechaFin='" + DateObject.ToString("yyyy-MM-dd HH:mm:ss.fff")
                + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            comando.ExecuteNonQuery();
            con.Close();

                //Despues por medio de un boton redireccoinar a carga fin
                Response.Redirect("CargaFin.aspx");
            }

        }
    }
    private DateTime ValEstatus()
    {
        //Comprueba la fecha en la que se inicio el folio y no deja que la fech de finweb sea antes que la de inicioWeb
        try
        {
            con.Open();
            SqlCommand comando2 = new SqlCommand("select WebFechaIni from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object fchsol = comando2.ExecuteScalar();
            DateTime fecha_sol = (DateTime)fchsol;
            con.Close();

            return fecha_sol;
        }
        catch (Exception et)
        {
            con.Close();
            con.Open();
            SqlCommand comando2 = new SqlCommand("select Inicio_Servicio from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object fchsol = comando2.ExecuteScalar();
            DateTime fecha_sol = (DateTime)fchsol;
            con.Close();

            con.Open();
            SqlCommand comando = new SqlCommand(" UPDATE FSR SET WebFechaIni=" +
                "CAST('" + fecha_sol.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            comando.ExecuteNonQuery();
            con.Close();

            return fecha_sol;
        }

    }
}