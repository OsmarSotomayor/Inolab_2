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
    //test
    public partial class Informacion : System.Web.UI.Page
    {
        string area;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }
            else
            {
                //En caso de que sean los jefes de area de los ingenieros tendran acceso al boton de seguimiento por el area a la que representan _
                lbluser.Text = Session["nameUsuario"].ToString();

                if (Session["idUsuario"].ToString() == "54") //Gustavo
                {
                    // cg.Visible = true;
                    lblcontador.Text = "Servicios Area Temperatura";
                }
                if (Session["idUsuario"].ToString() == "60") //Sergio
                {
                    // cg.Visible = true;
                    lblcontador.Text = "Servicios Area Fisicoquímicos";
                }
                if (Session["idUsuario"].ToString() == "30") //Armando
                {
                    // cg.Visible = true;
                    lblcontador.Text = "Servicios Area Analítica";
                }
            }
        }
        //Conexion a la base de datos (para hacer prebas acceder a BrowserPruebas)
        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");


        public void CargaDatos()
        {
            if (Session["idUsuario"].ToString() == "54")
            {
                D_Analitica();
            }
            if (Session["idUsuario"].ToString() == "60")
            {
                D_Fisicoquimicos();
            }
            if (Session["idUsuario"].ToString() == "30")
            {
                D_Temperatura();
            }
        }

        // VALIDACION DE AREA PARA MOSTRAR FSR
        public void D_Analitica()
        {           
            //Carga los folios del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select DISTINCT * from  v_fsr where areaservicio='Analitica' AND estatus='"+ddlfiltro.Text+"' order by folio desc", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet objdataset = new DataSet();
                adapter.Fill(objdataset);

                GridView1.DataSource = objdataset;
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString();
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }


        }
        public void D_Temperatura()
        {
            //Carga los folios del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select DISTINCT * from  v_fsr where areaservicio='Temperatura' order by folio desc", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet objdataset = new DataSet();
                adapter.Fill(objdataset);

                GridView1.DataSource = objdataset;
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString();
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }


        }
        public void D_Fisicoquimicos()
        {


            //Carga los folios del ingeniero
            try
            {
                SqlCommand cmd = new SqlCommand("Select DISTINCT * from  v_fsr where areaservicio='Fisicoquimico' order by folio desc", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet objdataset = new DataSet();
                adapter.Fill(objdataset);

                GridView1.DataSource = objdataset;
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString();
            }
            catch (Exception e)
            {
                Response.Write(e.ToString());
            }


        }

        protected void ddlfiltro_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        string comando = "";

        // filtro de estatus de los FSR
        protected void ddlfiltro_SelectedIndexChanged1(object sender, EventArgs e)
        {
            //Filtro de folios dependiendo a su estado de FSR Estatus 
            if (ddlfiltro.Text == "Asignado")
            {
                D_Analitica();

                comando = "select *from V_FSR where Estatus='Asignado' and IdIngeniero=" + Session["idusuario"] + " order by folio desc";
                sentencia();
            }
            if (ddlfiltro.Text == "En Proceso")
            {
                //comando = "select *from V_FSR where Estatus='En Proceso' and IdIngeniero=" + Session["idusuario"] + " order by folio desc";
                //sentencia();
            }
            if (ddlfiltro.Text == "Finalizado")
            {
                //comando = "select *from v_fsr where estatus='Finalizado' and idingeniero=" + Session["idusuario"] + " order by folio desc";
                //sentencia();
            }
            if (ddlfiltro.Text == "Todos")
            {
               // cargardatos();
            }
        }
        public void sentencia()
        {
            //Proceso de llenado del datagridview 
            SqlCommand cmd = new SqlCommand(comando, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.SelectCommand = cmd;
            con.Open();
            DataSet objdataset = new DataSet();
            adapter.Fill(objdataset);
            con.Close();

            GridView1.DataSource = objdataset;
            GridView1.DataBind();
            contador.Text = GridView1.Rows.Count.ToString();
        }
    }
}