using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;

namespace INOLAB_OC
{
    public partial class Adjuntar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
            {
                Response.Redirect("./Sesion.aspx");
            }

            Carpeta();

        }

        public void Carpeta()
        {
            String ruta = "";

            //          RUTA DE LA CARPETA

            ruta = Server.MapPath("~/Imagenes/" + Session["folio_p"].ToString());

            //ruta = Server.MapPath(@"\\INOLABSERVER01\Folios\" + Session["folio_p"].ToString());

            //ruta = Server.MapPath(@"\\INOLABSERVER01\Folios\" + Session["folio_p"].ToString());

            // CREA EL DIRECTORIO DEL FOLIO EN LA RUTA 
            Directory.CreateDirectory(ruta);

            //      ENCABEZADO DEL TEXTO
            lblcontador.Text = "Carpeta de Archivos para el FSR   " + Session["folio_p"].ToString();


        }

        protected void btnguardar_Click(object sender, EventArgs e)
        {
            try
            {
                //      LECTURA DE ARCHIVOS A GUARDAR
                for(int i=0; i < Context.Request.Files.Count; i++ )
                {
                    HttpPostedFile file = Context.Request.Files[i];
                    string name = file.FileName;
                    string ruta2;

                    //   RUTA DEL FOLIO SELECCIONADO PARA GUARDAR ARCHIVOS

                    ruta2 = Server.MapPath("~/Imagenes/" + Session["folio_p"].ToString() + "/");

                    //ruta2 = Server.MapPath(@"\\INOLABSERVER01\Folios\" + Session["folio_p"].ToString() + "/");

                    file.SaveAs(ruta2 + name);
                }

                lblmensaje.Visible = true;
                Response.Write("<script>alert('Archivos agredados correctamente');</script>");
                lblmensaje.Text = "Archivos agredados correctamente";


            }
            catch(Exception ex)
            {
                lblmensaje.Visible = true;
                Response.Write("<script>alert('Error al cargar la información');</script>");
                lblmensaje.Text = "Error en adjuntar archivos...";
                

            }                
 
        }
    }
}