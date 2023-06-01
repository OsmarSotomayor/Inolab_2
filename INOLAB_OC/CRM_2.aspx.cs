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
namespace INOLAB_OC
{
    public partial class CRM_2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }

            lbluser.Text = Session["nameUsuario"].ToString();
            lbliduser.Text = Session["idUsuario"].ToString();
            ReportViewer1.ServerReport.Refresh();


        }

        //REPORTEADOR
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
                serverReport.ReportPath = "/Comercial/Calendario-LlamadasXAsesor";

                // Create the sales order number report parameter
                ReportParameter asesor_v = new ReportParameter();
                asesor_v.Name = "asesor";
                asesor_v.Values.Add(Session["nameUsuario"].ToString());
                //asesor_v.Values.Add(Session["folio_p"].ToString());

                // Set the report parameters for the report
                ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { asesor_v });
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


        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Comercial;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

        // definine la clasificacion para la consulta sql

        protected void ddlTipoRegistro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoRegistro.Text == "Llamada")
            {
                lblFecha.Text = "Fecha de Llamada";
                datepicker.Enabled = true;
            }
            else
            {
                lblFecha.Text = "Fecha de Visita";
                datepicker.Enabled = true;

            }
        }

        // BOTON GUARDA UN NUEVO REGISTRO
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardaLlamada();
        }
        // FUNCION PARA GUARDAR NUEVO REGISTRO
        public void GuardaLlamada()
        {
            if (ddlTipoRegistro.Text == "")
            {
                Response.Write("<script>alert('Selecciona si el Registro es Llamada o Visita.');</script>");
                return;
            }
            if (datepicker.Text == "")
            {
                Response.Write("<script>alert('El Campo Fecha llamada/Visita no puede estar vacío.');</script>");
                return;
            }


            DateTime validafecha = DateTime.Parse(datepicker.Text);     //fecha a validar
            DateTime hoy = DateTime.Today;                              // fecha al dia de hoy

            if (validafecha < hoy)
            {
                Response.Write("<script>alert('El Campo Fecha Llamada/Visita no puede ser menor al día de Hoy.');</script>");
                return;
            }
            SqlCommand cmd = new SqlCommand("strp_save_plan", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@tiporegistro", SqlDbType.VarChar);
            cmd.Parameters.Add("@cliente", SqlDbType.VarChar);
            cmd.Parameters.Add("@fllamada", SqlDbType.Date);
            cmd.Parameters.Add("@comentario", SqlDbType.VarChar);
            cmd.Parameters.Add("@asesor", SqlDbType.VarChar);
            cmd.Parameters.Add("@objetivo", SqlDbType.VarChar);
            cmd.Parameters.Add("@hora", SqlDbType.VarChar);

            cmd.Parameters["@tiporegistro"].Value = ddlTipoRegistro.Text;
            cmd.Parameters["@cliente"].Value = txtcliente.Text;
            cmd.Parameters["@fllamada"].Value = Convert.ToDateTime(datepicker.Text);
            cmd.Parameters["@comentario"].Value = txtcomentario.Text;
            cmd.Parameters["@asesor"].Value = lbluser.Text;
            cmd.Parameters["@objetivo"].Value = txtobjetivo.Text;
            cmd.Parameters["@hora"].Value = ddlhora.Text;

            Response.Write("<script language=javascript>if(confirm('Registro Guardado Exitosamente')==true){ location.href='CRM_2.aspx'} else {location.href='CRM_2.aspx'}</script>");


            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();
        }

        //REDIRECCIONA A REGISTRO DE FUNNEL
        protected void btnRegFunnel_Click(object sender, EventArgs e)
        {
            Response.Redirect("CRM_3.aspx");
        }
        //REDIRECCIONA A GRAFICA FUNNEL
        protected void btnInforme_A_Click(object sender, EventArgs e)
        {
            Response.Redirect("CRM_1.aspx");
        }
        // ABRE EL REPORTEADOR DE COTIZACIONES
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("http://inolabserver01/Reportes_Inolab/Pages/ReportViewer.aspx?%2fComercial%2fCOTIZACION-EQUIPO&rs:Command=Render");
        }

        protected void Btn_MenuPrincipal(object sender, EventArgs e)
        {
            Response.Redirect("CRM_1.aspx");
        }

        //VALIDACION DE FECHA PARA NO GUARDAR UN DIA ANTERIOR A HOY
        public void ValidaFecha()
        {
            DateTime validafecha = DateTime.Parse(datepicker.Text);     //fecha a validar
            DateTime hoy = DateTime.Today;                              // fecha al dia de hoy

            if (validafecha < hoy)
            {
                Response.Write("<script>alert('La Fecha en el Registro de Llamada/Visita no puede ser menor al día de Hoy');</script>");
                return;
            }
        }
        //MUESTRA TODOS LOS DATOS X ASESOR
        private void Datos()
        {
            //Carga los resgistros del ASESOR
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  Llamada_Vista where asesor='" + lbluser.Text + "' and FechaLlamada between DATEADD(wk,DATEDIFF(wk,0,getdate()),0) and dateadd(wk,datediff(wk,0,getdate()),4)", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet objdataset = new DataSet();
                adapter.Fill(objdataset);
                // lblcontador.Text = GridView1.Rows.Count.ToString();
                GridView1.DataSource = objdataset;
                GridView1.DataBind();


            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }
        }

        int registro;
        //consulta para traer los datos del grid a los textbox y editar
        public void leer()
        {

            SqlCommand cmd = new SqlCommand("select * from llamada_vista where idllamada = " + registro + " and asesor='" + lbluser.Text+"'", con);
            con.Open();

            SqlDataReader leer;
            leer = cmd.ExecuteReader();
            if (leer.Read())
            {
                txtcliente.Text = leer["Cliente"].ToString();
                ddlTipoRegistro.Text = leer["Tipo"].ToString();
                datepicker.Text = leer["Fechallamada"].ToString();
                txtobjetivo.Text = leer["Objetivo"].ToString();
                txtcomentario.Text = leer["Comentario"].ToString();
                lblREGISTRO.Text=leer["IdLlamada"].ToString();
            }
            con.Close();


        }
        //LIMPIA LOS REGISTROS
        public void Clean()
        {
            ddlTipoRegistro.Text = "";
            txtcliente.Text = null;
            datepicker.Text = null;
            txtcomentario.Text = null;
            btnUpdate.Visible = false;
            btnGuardar.Visible = true;
        }


        //SELECCIONA EL REGISTRIO DEL GRID PARA LLENARLOS EN LOS TEXTBOX
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            registro = Convert.ToInt32(GridView1.SelectedRow.Cells[1].Text);
            btnUpdate.Visible = true;
            btnGuardar.Visible = false;
            leer();
        }
        //CARGA DATOS DEACUERDO EL TIPO DE REGISTRO
        private void cargardatos(string classs)
        {
            //Carga los resgistros del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  Llamada_vista where FechaLlamada between DATEADD(wk,DATEDIFF(wk,0,getdate()),0) and dateadd(wk,datediff(wk,0,getdate()),4) and tipo ='" + ddlTipofiltro.Text + "' and asesor='" + lbluser.Text + "'", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet objdataset = new DataSet();
                adapter.Fill(objdataset);
                // lblcontador.Text = GridView1.Rows.Count.ToString();
                GridView1.DataSource = objdataset;
                GridView1.DataBind();


            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }
        }
        string clas;
        protected void ddlTipofiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipofiltro.Text == "Llamada")
            {
                clas = ddlTipofiltro.Text;
                cargardatos(clas);
            }
            if (ddlTipofiltro.Text == "Visita")
            {
                clas = ddlTipofiltro.Text;
                cargardatos(clas);
            }
            if (ddlTipofiltro.Text == "Todo")
            {
                clas = ddlTipofiltro.Text;
                Datos();
            }
            lblcontador.Text = GridView1.Rows.Count.ToString();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Update();
        }

        public void Update()
        {
            if (ddlTipoRegistro.Text == "")
            {
                Response.Write("<script>alert('Selecciona si el Registro es Llamada o Visita.');</script>");
                return;
            }
            if (datepicker.Text == "")
            {
                Response.Write("<script>alert('El Campo Fecha llamada/Visita no puede estar vacío.');</script>");
                return;
            }


            DateTime validafecha = DateTime.Parse(datepicker.Text);     //fecha a validar
            DateTime hoy = DateTime.Today;                              // fecha al dia de hoy

            if (validafecha < hoy)
            {
                Response.Write("<script>alert('El Campo Fecha Llamada/Visita no puede ser menor al día de Hoy.');</script>");
                return;
            }
            SqlCommand cmd = new SqlCommand("stp_update_plan", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@registro", SqlDbType.Int);
            cmd.Parameters.Add("@fecha", SqlDbType.Date);
            cmd.Parameters.Add("@cliente", SqlDbType.VarChar);
            cmd.Parameters.Add("@comen", SqlDbType.VarChar);
            cmd.Parameters.Add("@tipo", SqlDbType.VarChar);
            cmd.Parameters.Add("@objetivo", SqlDbType.VarChar);

            cmd.Parameters["@registro"].Value = Convert.ToInt32(lblREGISTRO.Text);
            cmd.Parameters["@fecha"].Value = Convert.ToDateTime(datepicker.Text);
            cmd.Parameters["@cliente"].Value = txtcliente.Text;
            cmd.Parameters["@comen"].Value = txtcomentario.Text;
            cmd.Parameters["@tipo"].Value = ddlTipoRegistro.Text;
            cmd.Parameters["@objetivo"].Value = txtobjetivo.Text;

            Response.Write("<script language=javascript>if(confirm('Registro Actualizado Exitosamente')==true){ location.href='CRM_2.aspx'} else {location.href='CRM_2.aspx'}</script>");


            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();

        }

        protected void btnClean_Click(object sender, EventArgs e)
        {
            btnUpdate.Visible = false;
            Clean();


        }
    }
}