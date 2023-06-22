﻿using System;
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

namespace INOLAB_OC
{
   
    public partial class Informacion : System.Web.UI.Page
    {

        const string estatusDeFolioAsignado = "Asignado";
        const string estatusDeFolioEnProceso = "En Proceso";
        const string estatusDeFolioFinalizado = "Finalizado";
        const string todosLosFolios = "Todos";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }
            else
            {
                verificarSiUsuarioEsJefeDeSuArea();
                lbluser.Text = Session["nameUsuario"].ToString();
            }
        }
      

        public void verificarSiUsuarioEsJefeDeSuArea()
        {
            
            if (Session["idUsuario"].ToString() == "54") //Gustavo
            {
                lblcontador.Text = "Servicios Area Temperatura";
            }
            if (Session["idUsuario"].ToString() == "60") //Sergio
            {
                lblcontador.Text = "Servicios Area Fisicoquímicos";
            }
            if (Session["idUsuario"].ToString() == "30") //Armando
            {
                lblcontador.Text = "Servicios Area Analítica";
            }
        }

        // VALIDACION DE AREA PARA MOSTRAR FOLIOS DEPENDIENDO DEL AREA
        public void datosAnalitica()
        {
                //Carga los folios del ingeniero
                string query = "Select DISTINCT * from  v_fsr where areaservicio='Analitica' AND estatus='" + ddlfiltro.Text + "' order by folio desc";
                GridView1.DataSource = Conexion.getDataSet(query);
               
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString(); 
        }
        public void datosTemperatura()
        {
                //Carga los folios del ingeniero
                string query = "Select DISTINCT * from  v_fsr where areaservicio='Temperatura' order by folio desc";
                GridView1.DataSource = Conexion.getDataSet(query);
                
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString();
        }
        public void datosFisicoquimicos()
        {
                //Carga los folios del ingeniero
                string query = "Select DISTINCT * from  v_fsr where areaservicio='Fisicoquimico' order by folio desc";
                GridView1.DataSource = Conexion.getDataSet(query);
                
                GridView1.DataBind();
                contador.Text = GridView1.Rows.Count.ToString();
           
        }

      
        string consulta = "";

        protected void ddlfiltro_SelectedIndexChanged1(object sender, EventArgs e)
        {
            
            if (ddlfiltro.Text.Equals(estatusDeFolioAsignado))
            {
                consulta = "select *from V_FSR where Estatus='Asignado' and IdIngeniero=" + Session["idusuario"] + " order by folio desc";
                consultarFoliosDeServicio(consulta);
            }
            if (ddlfiltro.Text.Equals(estatusDeFolioEnProceso))
            {
                consulta = "select *from V_FSR where Estatus='En Proceso' and IdIngeniero=" + Session["idusuario"] + " order by folio desc";
                consultarFoliosDeServicio(consulta);
            }
            if (ddlfiltro.Text.Equals(estatusDeFolioFinalizado))
            {
                consulta = "select *from v_fsr where estatus='Finalizado' and idingeniero=" + Session["idusuario"] + " order by folio desc";
                consultarFoliosDeServicio(consulta);
            }
            if (ddlfiltro.Text.Equals(todosLosFolios))
            {
                consulta = "select * from v_fsr where  idingeniero = " + Session["idusuario"] + "order by folio desc;";
                consultarFoliosDeServicio(consulta);
            }
        }
        public void consultarFoliosDeServicio(string consulta)
        {
            GridView1.DataSource = Conexion.getDataSet(consulta);
            GridView1.DataBind();
            contador.Text = GridView1.Rows.Count.ToString();
        }
    }
}