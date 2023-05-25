using System;
using System.Net;
using Microsoft.Reporting.WebForms;

namespace INOLAB_OC
{
    public partial class Reporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ReportViewer1.Width = 1300;
                ReportViewer1.Height = 700;

                ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                IReportServerCredentials irsc = new CustomReportCredentials("developer", "In0l4b2022#");
                ReportViewer1.ServerReport.ReportServerCredentials = irsc;
                ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://INOLABSERVER01/Reportes_Inolab");
                ReportViewer1.ServerReport.ReportPath = "/OC/ReporteGeneralOC";
                ReportViewer1.ServerReport.Refresh();
            }
        }

        public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
        {
            private string _UserName;
            private string _PassWord;

            public CustomReportCredentials(string UserName, string PassWord)
            {
                _UserName = UserName;
                _PassWord = PassWord;
            }

            public System.Security.Principal.WindowsIdentity ImpersonationUser
            {
                get { return null; }
            }

            public ICredentials NetworkCredentials
            {
                get { return new NetworkCredential(_UserName, _PassWord); }
            }

            public bool GetFormsCredentials(out Cookie authCookie, out string user,
             out string password, out string authority)
            {
                authCookie = null;
                user = password = authority = null;
                return false;
            }
        }
    }
}