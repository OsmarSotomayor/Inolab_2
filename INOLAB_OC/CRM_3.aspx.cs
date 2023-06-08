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
using INOLAB_OC.Modelo;
using System.IO.Packaging;

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

        

        //variable para saber quien es su gerente
        string gte;



        private void cargardatos(string classs)
        {
            //Carga los resgistros del ingeniero
            string query = "Select* from  funnel where clasificacion = '" + ddlClasificacion.Text + "' and asesor='" + lbluser.Text + "'";
            GridView1.DataSource = ConexionComercial.getDataSet(query);
            GridView1.DataBind();
           
        }
        private void Datos()
        {
           //Carga los resgistros del asesor    
           string query = "Select* from  funnel where asesor='" + lbluser.Text + "' and fechacierre between '" + txtfecha1.Text + "' and '" + txtfecha2.Text + "'";
           GridView1.DataSource = ConexionComercial.getDataSet(query);
           GridView1.DataBind();
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

            

            string cliente = txtcliente.Text;
            string clasifiacion = ddlClas_save.Text;
            string fechaCierre = Convert.ToDateTime(datepicker.Text).ToString("dd/MM/yyyy");
            string equipo = txtequipo.Text;
            string marca = txtmarca.Text;
            string modelo= txtmodelo.Text;
            string valor = txtvalor.Text;
            string estatus= txtestatus.Text;
            string asesor= lbluser.Text;
            string contacto = TXTcONTACTO.Text;
            string localidad = ddLocalidad.Text;
            string origen= ddOrigen.Text;
            string tipo = ddTipoVenta.Text;
            string get_ = gte;

            ConexionComercial.executeStp_Save_Funnel(cliente, clasifiacion, fechaCierre, equipo, marca, modelo, valor, estatus, asesor, contacto, localidad, origen,
                tipo, gte);

            Trace.Write(asesor);
            Response.Write("<script language=javascript>if(confirm('Registro Guardado Exitosamente')==true){ location.href='CRM_3.aspx'} else {location.href='CRM_3.aspx'}</script>");

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
            string query = "select * from funnel where noregistro = " + registro + " and asesor='" + lbluser.Text + "'";
            DataRow datosFunel = ConexionComercial.getDataRow(query);

            txtcliente.Text = datosFunel["Cliente"].ToString();
            ddlClas_save.Text = datosFunel["Clasificacion"].ToString();
            datepicker.Text = datosFunel["FechaCierre"].ToString();
            txtequipo.Text = datosFunel["Equipo"].ToString();
            txtmarca.Text = datosFunel["Marca"].ToString();
            txtmodelo.Text = datosFunel["Modelo"].ToString();
            txtvalor.Text = datosFunel["Valor"].ToString();
            txtestatus.Text = datosFunel["Estatus"].ToString();
            txtf_actualiza.Text = datosFunel["FechaActualizacion"].ToString();
            lblresistro.Text = datosFunel["NoRegistro"].ToString();
            TXTcONTACTO.Text = datosFunel["Contacto"].ToString();
            ddLocalidad.Text = datosFunel["Localidad"].ToString();
            ddOrigen.Text = datosFunel["Origen"].ToString();
            ddTipoVenta.Text = datosFunel["TipoVenta"].ToString();
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

            //cmd.Parameters["@registro"].Value = Convert.ToInt32(lbluser.Text);
            int registro =Convert.ToInt32(lblresistro.Text);
            string cliente = txtcliente.Text;
            string clasificacion = ddlClas_save.Text;
            DateTime date = Convert.ToDateTime(datepicker.Text);
            string equipo = txtequipo.Text;
            string marca = txtmarca.Text;
            string modelo= txtmodelo.Text;
            string valor= txtvalor.Text;
            string status = txtestatus.Text;
            string user = lbluser.Text;
            string contacto = TXTcONTACTO.Text;
            string localidad = ddLocalidad.Text;
            string origen = ddOrigen.Text;
            string tipoVenta = ddTipoVenta.Text;

            ConexionComercial.executeStp_Update_Funnel(registro, cliente, clasificacion, date, equipo, marca, 
                modelo, valor, status, user, contacto,localidad,origen,tipoVenta);

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
            string query = "Select* from  funnel where asesor='" + lbluser.Text + "'and clasificacion='" + ddlClasificacion.Text + "' and fechacierre between '" + txtfecha1.Text + "' and '" + txtfecha2.Text + "'";
            GridView1.DataSource = ConexionComercial.getDataSet(query);
            GridView1.DataBind(); 
        }

        public void FiltroFechaT(string fclas)
        {
            //Carga los resgistros del ingeniero
            string query = "Select* from  funnel where asesor='" + lbluser.Text + "'and fechacierre between '" + txtfecha1.Text + "' and '" + txtfecha2.Text + "'";
            GridView1.DataSource = ConexionComercial.getDataSet(query);
            GridView1.DataBind();
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