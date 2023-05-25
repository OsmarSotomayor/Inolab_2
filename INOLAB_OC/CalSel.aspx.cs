using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INOLAB_OC
{
    public partial class CalSel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }
            else
            {

            }
        }

        protected void ddlfiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Dependiendo al elemento que se seleccione, hara referencia al id del area que aparece en la base de datos de usuarios
            if (ddlfiltro.SelectedIndex == 1)
            {
                Session["Area"] = "1";
            }
            if (ddlfiltro.SelectedIndex == 2)
            {
                Session["Area"] = "2";
            }
            if (ddlfiltro.SelectedIndex == 3)
            {
                Session["Area"] = "3";
            }
        }
        protected void btnSesion_Click(object sender, EventArgs e)
        {
            if (ddlfiltro.SelectedIndex == 0)
            {
                Response.Write("<script>alert('Por favor, seleccionar un Area');</script>");
            }
            else
            {
                //se mostrara el calendario correspondiente
                Response.Redirect("Calendario_Areas.aspx");
            }
        }
    }
}