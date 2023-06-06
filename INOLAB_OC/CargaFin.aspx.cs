﻿using INOLAB_OC.Modelo;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

namespace INOLAB_OC
{
    public partial class CargaFin : Page
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
      
        int cargai =0;
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
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { salesOrderNumber });
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

        protected void updateFSR(string nombre)
        {//Actualiza el nombre del cliente y la fecha en la que el cliente realiza la firma 
              Conexion.executeQuery(" UPDATE FSR SET NombreCliente='" + nombre + "', FechaFirmaCliente=" +
                    "CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");       
        }

        private void ActStatus()
        {
            try
            {
                //Actualizacion a estado de cierre de actividad de SAP    
                string updatestatus2 = "Update OCLG set OCLG.status = -3, OCLG.Closed = 'Y', OCLG.CloseDate =CAST('" +
                    DateTime.Now.ToString("yyyy-MM-dd") + "' AS DATETIME) from OCLG INNER JOIN SCL5 ON OCLG.ClgCode=SCL5.ClgID where SCL5.U_FSR ='" + Session["folio_p"].ToString() + "'";           
                ConexionInolab.executeQuery(updatestatus2);
                            
                string updatestatus3 = "Update SCL5 set U_ESTATUS = 'Finalizado' where U_FSR ='" + Session["folio_p"].ToString() + "'";
                ConexionInolab.executeQuery(updatestatus3);               

                //Buscar el callid    
                string callId = "Select SrvcCallId FROM SCL5 where U_FSR ='" + Session["folio_p"].ToString() + "'";               
                string resultado = ConexionInolab.getText(callId);
                                          
                string resultado2 = ConexionInolab.getText("Select count (DISTINCT U_ESTATUS) FROM SCL5 where SrvcCallId = " + resultado.ToString());
                //resultado2 el numero de valores que hay en estatus (Para que se cierre la llamada debe de ser "-1")
                
                //Hacer un ciclo while para identificar nulos? (usar visorder para pasar por todos)                               
                string resultadoc = ConexionInolab.getText("Select count(*) FROM SCL5 where SrvcCallId = " + resultado.ToString());
                
                bool nulo = false;

                //Proceso de chequeo de todos los estatus asignados a los folios dentro de la llamada para poder cerrarla o dejarla abierta
                for (int i = 1; i <= Convert.ToInt32(resultadoc); i++)
                {
                    string queryt = "Select U_ESTATUS FROM SCL5 where SrvcCallId = " + resultado.ToString() +
                        "and VisOrder = " + i.ToString();                   
                    string resultadot = ConexionInolab.getText(queryt);

                    if (resultadot != "Finalizado")
                    {
                        nulo = true;
                    }
                }

                //Despues del chequeo, si nulo == false, se cerrara la llamada debido a que todos los folios estan finalizados
                if (resultado2 == "1" && nulo == false)
                {
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
            //Envio del correo al cliente con su folio en estatus de Finalizado (hecho por paquito-cabeza [Incomprensible para mis conocimientos de C# :c])
            try
            {
                string to, bcc, from, subject;
                Console.Write(mail);
                to = "";           
                SqlDataReader sqldr = Conexion.getSqlDataReader("select * from MailNotification;");

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

                
                from = "notificaciones@inolab.com";
                subject = "FSR folio " + Session["folio_p"];
                MailMessage message = new MailMessage();
                message.Bcc.Add(bcc);
                message.From = new MailAddress(from);
                message.Body = PopulateBody(Session["folio_p"].ToString(), "cliente");
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
        {//Realiza la creación del PDF 
         // Variables  
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            //Setup the report viewer object and get the array of bytes 
            byte[] bytes = ReportViewer1.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.  
            string filepath = HttpRuntime.AppDomainAppPath + "Docs\\" + fileName + ".pdf";
            //Si existe este documento en el apartado de Docs, lo sustituye con el nuevo que se esta subiendo
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            //Se crea el PDF del folio para guardarlo en esa localizacion
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                Console.Write(fs.Name);
                fs.Dispose();
            }
            return filepath;
        }

        private string PopulateBody(string folio, string cliente)
        {//Realiza el replace en HTML para la creación del correo
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index2.html")))
            {
                body = reader.ReadToEnd();
                reader.Dispose();
            }
            //Parametros que requiere el body para representar de buena forma el HTML
            body = body.Replace("{folio}", folio);
            body = body.Replace("{cliente}", cliente);
            body = body.Replace("{slogan}", "data:image/png;base64," + GetBase64StringForImage(Server.MapPath("./Imagenes/slogan.png")));

            return body;
        }

        protected static string GetBase64StringForImage(string imgPath)
        {//Convierte la imagen en base64
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        private void SendMail2(string mail)
        {//Envía el correo electrónico con la información del FSR y adjunto el archivo
            try
            {
                //Asunto con el que se enviara el correo
                string subject = "Notificación de observaciones Folio: " + Session["folio_p"].ToString();

                //Datos y cuerpo del correo de a quien ira dirigido y de parte de quien 
                MailAddress from = new MailAddress("notificaciones@inolab.com");
                MailAddress to = new MailAddress(mail);
                MailMessage message2 = new MailMessage(from, to);
                message2.Bcc.Add("luisrosales@inolab.com");
                message2.Body = PopulateBody2(Session["folio_p"].ToString());
                message2.IsBodyHtml = true;
                message2.Subject = subject;

                //Datos par dar permiso a enviar el mensaje con una cuenta con la que se cuente
                SmtpClient client = new SmtpClient();
                client.Port = 1025;
                client.Host = "smtp.inolab.com";
                client.EnableSsl = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
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
        {//Realiza el replace en HTML para la creación del correo
            //Cuerpo del correo para el/la ases@r de ventas con la informacion para la accion correspondiente
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index_not_ase.html")))
            {
                body = reader.ReadToEnd();
                reader.Dispose();
            }
      
            //Consulta de los datos a la base de datos para colocarlos en el HTML
            string query = "Select top (1) Observaciones FROM FSR where Folio=" + Session["folio_p"].ToString();
            string observ = Conexion.getText(query);
          
            string llamada = "Interna";
            try
            {
             
                string query2 = "Select top (1) SrvcCallId FROM SCL5 where U_FSR=" + Session["folio_p"].ToString();
                llamada = ConexionInolab.getText(query2);
             
            }
            catch (Exception es)
            {
                Console.Write(es.ToString());
            }

            
            string query3 = "Select top (1) Cliente FROM FSR where Folio=" + Session["folio_p"].ToString();        
            string cliente = Conexion.getText(query3);
                     
            string query4 = "Select top (1) Equipo FROM FSR where Folio=" + Session["folio_p"].ToString();        
            string equipo = Conexion.getText(query4);
                     
            string query5 = "Select top (1) TipoServicio FROM V_FSR where Folio=" + Session["folio_p"].ToString();
            string servicio = Conexion.getText(query5);
                     
            string query6 = "Select top (1) Ingeniero FROM V_FSR where Folio=" + Session["folio_p"].ToString();         
            string ingeniero = Conexion.getText(query6);
                       
            string query7 = "Select top (1) Actividad FROM V_FSR where Folio=" + Session["folio_p"].ToString();           
            string actividad = Conexion.getText(query7);
                    
            string query8 = "Select top (1) OC FROM V_FSR where Folio=" + Session["folio_p"].ToString();          
            string OrdenVenta = Conexion.getText(query8);
          

            //Insercion de datos al HTML
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
        {//Envía el correo electrónico con la información del FSR y adjunto el archivo
            //Cuerpo del correo que se le enviara a facturacion, en caso de que se requiera facturar (Servicios Puntuales nada más)
            try
            {
                //Asunto del correo que se enviara
                string subject = "Servicio Terminado. Folio: " + Session["folio_p"].ToString();

                //Datos de a quein ira y de quien va dirigido el correo (me anexo como copia oculta para monitorear que se realice el proceso de forma correcta)
                MailAddress from = new MailAddress("notificaciones@inolab.com");
                MailAddress to = new MailAddress("facturacion@inolab.com");
                MailMessage message3 = new MailMessage(from, to);
                message3.Bcc.Add("luisrosales@inolab.com");
                
                //Intento de colocar a dos personas en copia oculta (No funciona)
                //message3.Bcc.Add("luisrosales@inolab.com" + "ivivar@inolab.com");

                message3.Body = PopulateBody3(Session["folio_p"].ToString());
                message3.IsBodyHtml = true;
                message3.Subject = subject;

                //Anexo del folio el cual sera enviado junto con el mensaje
                Attachment attach = new Attachment(filepath);
                message3.Attachments.Add(attach);

                //Datos correspondientes al emisor para el envio del mensaje 
                SmtpClient client = new SmtpClient();
                client.Port = 1025;
                client.Host = "smtp.inolab.com";
                client.EnableSsl = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
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
        {//Realiza el replace en HTML para la creación del correo
            //Cuerpo de el correo para facturacion
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("./HTML/index_not_fact.html")))
            {
                body = reader.ReadToEnd();
                reader.Dispose();
            }

            //En caso de ser llamada interna, no tendra datos en SAP (Este campo queda como "Interno")
            string llamada = "Interna";
            try
            {
                string query2 = "Select top (1) SrvcCallId FROM SCL5 where U_FSR=" + Session["folio_p"].ToString();              
                llamada = ConexionInolab.getText(query2);
             
            }
            catch (Exception es)
            {
                Console.Write(es.ToString());        
            }
            
            //Obtencion de datos para el correo
           
            string query3 = "Select top (1) Cliente FROM FSR where Folio=" + Session["folio_p"].ToString();
            string cliente = Conexion.getText(query3);
      
            string query7 = "Select top (1) Actividad FROM V_FSR where Folio=" + Session["folio_p"].ToString();
            string actividad = Conexion.getText(query7);
            

            
            string query8 = "Select top (1) OC FROM V_FSR where Folio=" + Session["folio_p"].ToString();
            string OrdenVenta = Conexion.getText(query8);
            

            //Insercion de datos para el HTML
            body = body.Replace("{folio}", folio);
            body = body.Replace("{slogan}", "data:image/png;base64," + GetBase64StringForImage(Server.MapPath("./Imagenes/slogan.png")));
            body = body.Replace("{n_llamada}", llamada);
            body = body.Replace("{act_iv}", actividad);
            body = body.Replace("{OrdenVenta}", OrdenVenta);
            body = body.Replace("{cliente}", cliente);
            return body;
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
            //Realiza el update de los datos en FSR y sollicita la creación del PDF para mandarlo por correo electrónico 
            try
            {
                //Obtiene el Mail de el cliente
          
                SqlDataReader sqldr = Conexion.getSqlDataReader("select Mail from FSR where Folio = " + Session["folio_p"].ToString() + " and IdFirmaImg is not null;");
                if (sqldr.Read())
                {
                    string mail = sqldr.GetValue(0).ToString();
                    
                    
                    //Actualizacion de estatus de el FSR con los datos correspondientes
                   
                    Conexion.updateHorasDeServicio(Session["folio_p"], Session["idUsuario"]);

                    //Proceso de SAP
                    try
                    {
                        //Conseguir la clave CLGcode de el folio                     
                        string clgcode = "Select ClgID FROM SCL5 where U_FSR = " + Session["folio_p"];                       
                        int clg = ConexionInolab.getScalar(clgcode);            

                        //Se hace el update de la concatenacion de Folio y Estatus
                        ConexionInolab.executeQuery(" UPDATE OCLG SET tel = '" + Session["folio_p"] + " Finalizado' where ClgCode=" + clg.ToString() + ";");
                        
                    }
                    catch (Exception es)
                    {
                        Console.Write(es.ToString());
                    }

                    //Actualizacion del estatus del folio a Finalizado
                    string queryUpdateStatus = "UPDATE FSR SET IdStatus = 3 WHERE Folio = " + Session["folio_p"].ToString() + " and IdStatus = 2;";
                    Conexion.executeQuery(queryUpdateStatus);
                    

                    //Se actualiza el estado en SAP de la llamada
                    ActStatus();

                    ReportViewer1.ServerReport.Refresh();
                    string path = CreatePDF(Session["folio_p"].ToString());
                    //Notificacion al Asesor
                    if (Session["not_ase"].ToString() == "Si")
                    {
                        string query = "Select top (1) Correoasesor1 FROM V_FSR where Folio=" + Session["folio_p"].ToString();
                        string asesorm = Conexion.getText(query);

                        SendMail2(asesorm);
                    }

                    //Notificacion a Facturacion
                    //Codigo para verificar el tipo de contrato:

                   
                    string query2 = "Select top (1) IdT_Contrato FROM FSR where Folio=" + Session["folio_p"].ToString();
                    string TCon = Conexion.getText(query2);
                    

                    //Codigo para en caso de ser Servicio puntual mandar correo al asesor
                    if (TCon == "7")
                    {
                        SendMail3(path);
                    }
                    SendMail(path, mail);
                    Response.Redirect("ServiciosAsignados.aspx");
                }
                else
                {
                    Response.Redirect("VistaPrevia.aspx");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());

            }
        }
    }
}