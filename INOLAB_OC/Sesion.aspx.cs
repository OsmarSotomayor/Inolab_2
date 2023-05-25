using System;
using System.Data.SqlClient;
using System.Data;


namespace INOLAB_OC
{
    public partial class Sesion : System.Web.UI.Page
    {
        //Conexion a la base de datos (para hacer prebas acceder a BrowserPruebas)
        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");
        protected void Page_Load(object sender, EventArgs e)
        {
            string IP = Request.ServerVariables["REMOTE_ADDR"];
            lblip.Text = IP.ToString();
        }

        protected void btnSesion_Click(object sender, EventArgs e)
        {
            //Al presionar el boton de ingresar
            Ingresar();
        }

        public void Ingresar()
        {
            try
            {
                //Fechas para el calendario
                Session["fecha1"] = "";
                Session["fecha2"] = "";

                Log();
                //Se hace la consulta de los datos del usuario que esta ingresando 
                SqlCommand comando = new SqlCommand("select  Usuario, Password_,Nombre, Apellidos,idarea,idrol,IdUsuario from Usuarios where usuario='" + txtUsuario.Text + "' and password_='" + txtPass.Text + "'", con);
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(ds, "tablausuario");

                DataRow dr;
                dr = ds.Tables["tablausuario"].Rows[0];
                if ((txtUsuario.Text == dr["Usuario"].ToString()) || (txtPass.Text == dr["Password_"].ToString()))
                {
                    Response.Write("<script>alert('Ingreso de " + dr["Nombre"].ToString() + " " + dr["Apellidos"].ToString() + "');</script>");
                }
                string valor = dr["Nombre"].ToString(), idar = dr["IdArea"].ToString(), idr = dr["IdRol"].ToString();
                //Asignacion de variables globales que s epueden utilizar en otr spaginas dentro de una misma sesion
                Session["valor"] = valor;
                Session["idar"] = idar;
                Session["idr"] = idr;

                Session["Usuario"] = dr["Usuario"].ToString();


                if (idar == "6")
                {
                    //Accede a servicios asignados mientras sea idar = 6
                    Session["idUsuario"] = dr["idUsuario"].ToString();
                    Session["nameUsuario"] = dr["Nombre"].ToString();
                    Response.Redirect("ServiciosAsignados.aspx");
                }
                else
                {
                    //Para el caso de que sea Liz la que ingrese, se le mostraran los calendarios de servicios que hay de todos los ingenieros correspondiendo a su area
                    Session["idUsuario"] = dr["idUsuario"].ToString();
                    Session["nameUsuario"] = dr["Nombre"].ToString();
                    if (Session["idUsuario"].ToString() == "8")
                    {
                        //Form de seleccion de calendario
                        Response.Redirect("CalSel.aspx");

                    }
                    //Usado para las paginas que dependan al departamento que vengan
                    //if (idr == "2" && idar=="2")
                    if (idar == "2")
                    {
                        if(Session["idUsuario"].ToString() == "7")
                        {
                            Response.Redirect("CRM_4.aspx");
                        }
                        //Response.Redirect("Servicio_OC1.aspx");
                        Response.Redirect("CRM_1.aspx");
                    }

                }
            }
            catch
            {
                Response.Write("<script>alert('Usuario y/o Contraseña Incorrectos');</script>");
            }
        }

        public void Log()
        {
            SqlCommand cmd1 = new SqlCommand("LogWeb", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Add("@usuario", SqlDbType.VarChar);
            cmd1.Parameters.Add("@ip", SqlDbType.VarChar);

            cmd1.Parameters["@usuario"].Value = txtUsuario.Text;
            cmd1.Parameters["@ip"].Value = lblip.Text;

            con.Open();
            cmd1.ExecuteNonQuery();
        }
    }
}