using System;
using System.Data.SqlClient;
using System.Data;
using INOLAB_OC.Modelo;
using System.Diagnostics;

namespace INOLAB_OC
{
    public partial class Sesion : System.Web.UI.Page
    {
        const string areaVentas = "2";
        const string areaServiciosIngenieria = "6";
        const string directorArtemio = "7";
        protected void Page_Load(object sender, EventArgs e)
        {
          string IP = Request.ServerVariables["REMOTE_ADDR"];
          lblip.Text = IP.ToString();   
        }

      
        public void Log()
        {
            Conexion.executeStoreProcedureLogWeb(txtUsuario.Text, lblip.Text);
        }
        public void ingresarAlAreaCorrespondiente()
        {
            try
            {
                //Fechas para el calendario
                Session["fecha1"] = "";
                Session["fecha2"] = "";

                Log();
                
                DataRow dataUser = Conexion.getDataRow("select  Usuario, Password_,Nombre, Apellidos,idarea,idrol,IdUsuario from Usuarios where usuario='" + txtUsuario.Text + "' and password_='" + txtPass.Text + "'");
                
                if ((txtUsuario.Text == dataUser["Usuario"].ToString()) || (txtPass.Text == dataUser["Password_"].ToString()))
                {
                    Response.Write("<script>alert('Ingreso de " + dataUser["Nombre"].ToString() + " " + dataUser["Apellidos"].ToString() + "');</script>");
                }
                string nombreDeUsuario = dataUser["Nombre"].ToString(), idArea = dataUser["IdArea"].ToString(), idRollUser = dataUser["IdRol"].ToString(), nombreDeUsuarioAbreviado = dataUser["Usuario"].ToString();
                
                Session["valor"] = nombreDeUsuario;
                Session["idar"] = idArea;
                Session["idr"] = idRollUser;
                Session["Usuario"] = nombreDeUsuarioAbreviado;


                if (idArea.Equals(areaServiciosIngenieria))
                {
                    Session["idUsuario"] = dataUser["idUsuario"].ToString();
                    Session["nameUsuario"] = dataUser["Nombre"].ToString();
                    Response.Redirect("../Vista/Ingenieros/ServiciosAsignados.aspx");
                    
                }
                else
                {
                    
                    //Para el caso de que sea Liz la que ingrese, se le mostraran los calendarios de servicios que hay de todos los ingenieros correspondiendo a su area
                    Session["idUsuario"] = dataUser["idUsuario"].ToString();
                    Session["nameUsuario"] = dataUser["Nombre"].ToString();

                    if (Session["idUsuario"].ToString() == "8")
                    {
                        Response.Redirect("../Vista/Ingenieros/CalSel.aspx");
                    }
                    if (idArea == areaVentas)
                    {
                        if(Session["idUsuario"].ToString() == directorArtemio)
                        {
                            Response.Redirect("../Vista/Ventas/CRM_4.aspx");
                        }
                        Response.Redirect("../Vista/Ventas/CRM_1.aspx");
          
                    }

                }
            }
            catch
            {
                Response.Write("<script>alert('Usuario y/o Contraseña Incorrectos');</script>");
            }
        }

        protected void Inicio_De_Sesion_Click(object sender, EventArgs e)
        {
            ingresarAlAreaCorrespondiente();
        }


    }
}