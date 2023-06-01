using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;
using System.Configuration;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Collections.Generic;
using SpreadsheetLight;
using System.Windows;


namespace INOLAB_OC
{
    public partial class CRM_3 : System.Web.UI.Page
    {
        //comentario test
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }

            lbluser.Text = Session["nameUsuario"].ToString();
            lbliduser.Text = Session["idUsuario"].ToString();


            if( (Session["idUsuario"].ToString()=="2") || (Session["idUsuario"].ToString() == "44") || (Session["idUsuario"].ToString() == "3") || (Session["idUsuario"].ToString() == "15" || (Session["idUsuario"].ToString() == "131") || (Session["idUsuario"].ToString() == "16")))
            {
                gte = "Paola";
            }
            if((Session["idUsuario"].ToString() == "1") || (Session["idUsuario"].ToString() == "13") || (Session["idUsuario"].ToString() == "123") || (Session["idUsuario"].ToString() == "124") || (Session["idUsuario"].ToString() == "84") || (Session["idUsuario"].ToString() == "98") || (Session["idUsuario"].ToString() == "126"))
            {
                gte = "Karla";
            }

            if ((Session["idUsuario"].ToString() == "119") || (Session["idUsuario"].ToString() == "122") || (Session["idUsuario"].ToString() == "6"))
            {
                gte = "Rodolfo";
            }
        }

        //Conexion a la base de datos (para hacer prebas acceder a BrowserPruebas)
        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Comercial;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

        //variable para saber quien es su gerente
        string gte;



        private void cargardatos(string classs)
        {
            //Carga los resgistros del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  funnel where clasificacion = '"+ddlClasificacion.Text+"' and asesor='"+lbluser.Text+"'", con);
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
        private void Datos()
        {
            //Carga los resgistros del asesor
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  funnel where asesor='"+lbluser.Text+"' and fechacierre between '"+txtfecha1.Text+"' and '"+txtfecha2.Text+"'", con);
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

        // definine la clasificacion para la consulta sql
        string clas;
        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnfiltrar.Visible = true;
            lblfecha1.Visible = true;
            lblfecha2.Visible = true;
            btnfiltrar.Visible = true;
            txtfecha1.Visible = true;
            txtfecha2.Visible = true;

            if(ddlClasificacion.Text=="Lead")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Oportunidad")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Proyecto")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Forecast")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Orden Compra")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Prospecto")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Todo")
            {
                txtfecha1.Text = "2023-01-01";
                txtfecha2.Text = "2090-12-31";
                Datos();
                txtfecha1.Text = null;
                txtfecha2.Text = null;
            }
            if (ddlClasificacion.Text == "No Relacionado")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }
            if (ddlClasificacion.Text == "Perdido")
            {
                txtfecha1.Text = null;
                txtfecha2.Text = null;
                clas = ddlClasificacion.Text;
                cargardatos(clas);
            }


            lblcontador.Text = GridView1.Rows.Count.ToString();

   
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardaFunnel();
        }

        public void GuardaFunnel()
        {
            if (txtcliente.Text == "")
            {
                Response.Write("<script>alert('Captura el Cliente.');</script>");
                return;
            }
            if (ddlClas_save.Text=="")
            {
                Response.Write("<script>alert('Selecciona la clasificación del Registro.');</script>");
                return;
            }
            if(datepicker.Text=="")
            {
                Response.Write("<script>alert('Captura la Fecha de Cierre del registro.');</script>");
                return;
            }
            if (ddLocalidad.Text == "")
            {
                Response.Write("<script>alert('Captura la Localidad del registro.');</script>");
                return;
            }
            if (ddTipoVenta.Text == "")
            {
                Response.Write("<script>alert('Captura el Tipo de Venta.');</script>");
                return;
            }

            SqlCommand cmd = new SqlCommand("stp_save_funnel", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@cliente", SqlDbType.VarChar);
            cmd.Parameters.Add("@clasif", SqlDbType.VarChar);
            cmd.Parameters.Add("@f_cierre", SqlDbType.Date);
            cmd.Parameters.Add("@equipo", SqlDbType.VarChar);
            cmd.Parameters.Add("@marca", SqlDbType.VarChar);
            cmd.Parameters.Add("@modelo", SqlDbType.VarChar);
            cmd.Parameters.Add("@valor", SqlDbType.Decimal);
            cmd.Parameters.Add("@estatus", SqlDbType.VarChar);
            cmd.Parameters.Add("@asesor", SqlDbType.VarChar);
            cmd.Parameters.Add("@contacto", SqlDbType.VarChar);
            cmd.Parameters.Add("@localidad", SqlDbType.VarChar);
            cmd.Parameters.Add("@origen", SqlDbType.VarChar);
            cmd.Parameters.Add("@tipo", SqlDbType.VarChar);
            cmd.Parameters.Add("@gte", SqlDbType.VarChar);

            cmd.Parameters["@cliente"].Value = txtcliente.Text;
            cmd.Parameters["@clasif"].Value = ddlClas_save.Text;
            cmd.Parameters["@f_cierre"].Value = Convert.ToDateTime(datepicker.Text).ToString("dd/MM/yyyy");
            cmd.Parameters["@equipo"].Value = txtequipo.Text;
            cmd.Parameters["@marca"].Value = txtmarca.Text;
            cmd.Parameters["@modelo"].Value = txtmodelo.Text;
            cmd.Parameters["@valor"].Value = txtvalor.Text;
            cmd.Parameters["@estatus"].Value = txtestatus.Text;
            cmd.Parameters["@asesor"].Value = lbluser.Text;
            cmd.Parameters["@contacto"].Value = TXTcONTACTO.Text;
            cmd.Parameters["@localidad"].Value = ddLocalidad.Text;
            cmd.Parameters["@origen"].Value = ddOrigen.Text;
            cmd.Parameters["@tipo"].Value = ddTipoVenta.Text;
            cmd.Parameters["@gte"].Value = gte;

            Response.Write("<script language=javascript>if(confirm('Registro Guardado Exitosamente')==true){ location.href='CRM_3.aspx'} else {location.href='CRM_3.aspx'}</script>");


            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();

        }
        // parametro para la consulta en BD 
        int registro;
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            registro = Convert.ToInt32(GridView1.SelectedRow.Cells[1].Text);
            //lbluser.Text= GridView1.SelectedRow.Cells[1].Text;
            //registro =Convert.ToInt32(lbluser.Text);
     

            leer();
            btnGuardar.Visible = false;
            btnactualiza.Visible = true;

        }
        public void Limpiar()
        {
            txtcliente.Text = null;
            ddlClas_save.Text = null;
            datepicker.Text = null;
            txtequipo.Text = null;
            txtmarca.Text = null;
            txtmodelo.Text = null;
            txtvalor.Text = "0";
            txtestatus.Text = null;
            txtf_actualiza.Text = null;
            TXTcONTACTO.Text = null;
            ddLocalidad.Text = null;
            ddOrigen.Text = null;
            ddTipoVenta.Text = null;


            btnGuardar.Visible = true;
            btnactualiza.Visible = false;
        }
        public void leer()
        {
            //consulta para traer los datos del grid a los textbox y editar
            SqlCommand cmd = new SqlCommand("select * from funnel where noregistro = " + registro+" and asesor='"+lbluser.Text+"'", con);
            con.Open();

            SqlDataReader leer;
            leer = cmd.ExecuteReader();
            if (leer.Read())
            {
                txtcliente.Text = leer["Cliente"].ToString();
                ddlClas_save.Text = leer["Clasificacion"].ToString();
                datepicker.Text= leer["FechaCierre"].ToString();
                txtequipo.Text= leer["Equipo"].ToString();
                txtmarca.Text= leer["Marca"].ToString();
                txtmodelo.Text= leer["Modelo"].ToString();
                txtvalor.Text= leer["Valor"].ToString();
                txtestatus.Text= leer["Estatus"].ToString();
                txtf_actualiza.Text = leer["FechaActualizacion"].ToString();
                lblresistro.Text = leer["NoRegistro"].ToString();
                TXTcONTACTO.Text = leer["Contacto"].ToString();
                ddLocalidad.Text = leer["Localidad"].ToString();
                ddOrigen.Text = leer["Origen"].ToString();
                ddTipoVenta.Text = leer["TipoVenta"].ToString();


            }
            con.Close();


        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.EditIndex = -1;
            GridView1.PageIndex = e.NewPageIndex;
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        protected void btnactualiza_Click(object sender, EventArgs e)
        {
            ActualizaFunnel();
            
        }

        public void ActualizaFunnel()
        {
            if (datepicker.Text == "")
            {
                Response.Write("<script>alert('Captura la Fecha de Cierre del registro.');</script>");
                return;
            }
            SqlCommand cmd = new SqlCommand("stp_update_funnel", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@registro", SqlDbType.Int);
            cmd.Parameters.Add("@cliente", SqlDbType.VarChar);
            cmd.Parameters.Add("@clasif", SqlDbType.VarChar);
            cmd.Parameters.Add("@f_cierre", SqlDbType.Date);
            cmd.Parameters.Add("@equipo", SqlDbType.VarChar);
            cmd.Parameters.Add("@marca", SqlDbType.VarChar);
            cmd.Parameters.Add("@modelo", SqlDbType.VarChar);
            cmd.Parameters.Add("@valor", SqlDbType.Decimal);
            cmd.Parameters.Add("@estatus", SqlDbType.VarChar);
            cmd.Parameters.Add("@asesor", SqlDbType.VarChar);
            cmd.Parameters.Add("@contacto", SqlDbType.VarChar);
            cmd.Parameters.Add("@localidad", SqlDbType.VarChar);
            cmd.Parameters.Add("@origen", SqlDbType.VarChar);
            cmd.Parameters.Add("@tipo", SqlDbType.VarChar);
            


            //cmd.Parameters["@registro"].Value = Convert.ToInt32(lbluser.Text);
            cmd.Parameters["@registro"].Value =Convert.ToInt32(lblresistro.Text);
            cmd.Parameters["@cliente"].Value = txtcliente.Text;
            cmd.Parameters["@clasif"].Value = ddlClas_save.Text;
            cmd.Parameters["@f_cierre"].Value = Convert.ToDateTime(datepicker.Text);
            cmd.Parameters["@equipo"].Value = txtequipo.Text;
            cmd.Parameters["@marca"].Value = txtmarca.Text;
            cmd.Parameters["@modelo"].Value = txtmodelo.Text;
            cmd.Parameters["@valor"].Value = txtvalor.Text;
            cmd.Parameters["@estatus"].Value = txtestatus.Text;
            cmd.Parameters["@asesor"].Value = lbluser.Text;
            cmd.Parameters["@contacto"].Value = TXTcONTACTO.Text;
            cmd.Parameters["@localidad"].Value = ddLocalidad.Text;
            cmd.Parameters["@origen"].Value = ddOrigen.Text;
            cmd.Parameters["@tipo"].Value = ddTipoVenta.Text;


            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();

            Response.Write("<script language=javascript>if(confirm('Registro Actualizado Exitosamente')==true){ location.href='CRM_3.aspx'} else {location.href='CRM_3.aspx'}</script>");
            Limpiar();
        }


        //REDIRECCIONA A PLAN DE TRABAJO
        protected void btnPlan_Click(object sender, EventArgs e)
        {
            Response.Redirect("CRM_2.aspx");
        }

       

        //REDIRECCIONA A DESCARGA DE COTIZACIONES
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("http://inolabserver01/Reportes_Inolab/Pages/ReportViewer.aspx?%2fComercial%2fCOTIZACION-EQUIPO&rs:Command=Render");
        }

        protected void Btn_MenuPrincipal_Click(object sender, EventArgs e)
        {
            Response.Redirect("CRM_1.aspx");
        }

        //FILTRADO DE FECHA POR CLASIFICACION
        public void FiltroFecha(string fclas)
        {
            //Carga los resgistros del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  funnel where asesor='" + lbluser.Text + "'and clasificacion='"+ddlClasificacion.Text+"' and fechacierre between '"+txtfecha1.Text+"' and '"+txtfecha2.Text+"'", con);
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

        public void FiltroFechaT(string fclas)
        {
            //Carga los resgistros del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select* from  funnel where asesor='" + lbluser.Text + "'and fechacierre between '" + txtfecha1.Text + "' and '" + txtfecha2.Text + "'", con);
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

        protected void btnfiltrar_Click(object sender, EventArgs e)
        {
            if(ddlClasificacion.Text=="Todo")
            {
                FiltroFechaT(clas);
            }
            else
            {
                FiltroFecha(clas);
            }
            
            lblcontador.Text = GridView1.Rows.Count.ToString();
        }
    }
}