using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;
using System.Configuration;
using System.Web.UI;
using INOLAB_OC.Modelo;

namespace INOLAB_OC
{
    public partial class DescargaFolio : System.Web.UI.Page
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
            cargardatos();
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

                     User name*/
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
                string query = "Select *from  v_fsr where idingeniero=" + Session["Idusuario"] + " and estatusid=3 order by folio desc";
                GridView1.DataSource = Conexion.getDataSet(query);
                GridView1.DataBind();
           
        }

        protected void GridView1_OnRowComand(object sender, GridViewCommandEventArgs e)
        {//Al darle clic al folio deseado este se almacena en la sesión y te redirige a la ventana de FSR
            try
            {
                String text = "";
                String tipo = "";
                if (e.CommandName == "Select")
                {
                    int index = int.Parse(e.CommandArgument.ToString());
                    GridViewRow row = GridView1.Rows[index];
                    text = ((LinkButton)row.Cells[0].Controls[0]).Text;
                    tipo = row.Cells[1].Text;
                }

                Session["folio_p"] = text;

                if (tipo.Equals("Finalizado"))
                {
                    //llama a la funcion de descargar folio en caso de que haya uno existente
                    //DownloadFolio(Session["folio_p"].ToString());
                    //como no queremos que nos tome el existente, siempre vamos a crear un folio en caso de que se encuentre finalizado pero queramos hacer cambios
                    recreatePDF(Session["folio_p"].ToString());
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        /*Funcion para mostrar un folio ya creado y en caso de que no este, llama a recrear folio 
         * protected void DownloadFolio(string folio)
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
            catch (Exception e)
            {
                recreatePDF(folio);
            }
        }*/

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

            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.  
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + nombre);
            Response.BinaryWrite(bytes); // create the file  
            Response.Flush(); // send it to the client to download
        }
    }
}