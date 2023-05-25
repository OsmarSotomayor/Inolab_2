using System;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Web.UI;
using System.Web;
using System.IO;
using static INOLAB_OC.DescargaFolio;
using SpreadsheetLight;
using System.Collections.Generic;
using ExcelDataReader;
using System.Data;
using System.Linq;

namespace INOLAB_OC
{
    public partial class SubiendoArchivos : System.Web.UI.Page
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

        public void ExcelFileReader(string path)
        {
            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            var tables = result.Tables.Cast<DataTable>();
            foreach (DataTable table in tables)
            {
                if (table.ToString() == "Sheet1")
                {
                    Response.Write("<script>alert('Si lee bien el archivo');</script>");
                    //Donde pondre los datos
                    //GridView1.DataSource = table;
                    //GridView1.DataBind();
                }
            }
        }

        protected void pruebaclic(object sender, EventArgs e)
        {
            //Consulta de los datos
            string path = @"C:\Users\" + Environment.UserName.ToString() + @"\Datos Folio\" + Session["folio_p"].ToString() + "completo.xlsx";
            try
            {
                ExcelFileReader(path);

            }
            catch (Exception es)
            {
                Response.Write("<script>alert('Error: No se encuentra el archivo correspondiente a este folio');</script>");
            }
        }
    }
}