using System;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace INOLAB_OC
{
    public partial class ConfirmacionServicio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string valor = Request.QueryString["vlrtknfkgt"];
            if (Request.Params["vlrtknfkgt"] != null)
            {
                lbluser.Text = Request.Params["vlrtknfkgt"];
            }
            consulta();
            sentencia();
            if (respuesta == Convert.ToString('Y'))
            {
                Response.Redirect("Principal.aspx");
            }
            if (string.IsNullOrEmpty(propuesta_s))
                {}
            else
            {
                Response.Redirect("Principal.aspx");
            }
        }

        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");
        string respuesta;
        string propuesta_s;
        string cliente;
        string equipo;
        DateTime fecha_s;
        string hora;
        string ing;
        string status;
        string mailasesor, asesor;
        string llamada, folio;
        string actividad, tservicio;

        protected void btbproponer_Click(object sender, EventArgs e)
        {
            datepicker.Visible = true;
            lblnuevafecha.Visible = true;
            btnEnviarRespuesta.Visible = true;
            lblhora.Visible = true;
            dplhora.Visible = true;
        }
        public void consulta()
        {
            DateTime fecha;
            SqlCommand cmd = new SqlCommand("Select *from v_FSR where nollamada=" + lbluser.Text, con);
            con.Open();
            SqlDataReader leer;
            leer = cmd.ExecuteReader();
            if (leer.Read())
            {
                fecha = Convert.ToDateTime(leer["FechaServicio"]);
                txtfechafsr.Text = fecha.ToShortDateString();
                lbluser.Text = Convert.ToString(leer["NoLlamada"]);
                respuesta = Convert.ToString(leer["Confirmacion"]);
                propuesta_s = Convert.ToString(leer["Propuesta"]);
                cliente = Convert.ToString(leer["Cliente"]);
                equipo = Convert.ToString(leer["Equipo"]);
                fecha_s = Convert.ToDateTime(leer["FechaServicio"]);
                hora = Convert.ToString(leer["HoraServicio"]) + " HRS.";
                ing = Convert.ToString(leer["Ingeniero"]);
                status = Convert.ToString(leer["S_Confirmacion"]);
                mailasesor = Convert.ToString(leer["Correoasesor1"]);
                llamada = Convert.ToString(leer["NoLlamada"]);
                actividad = Convert.ToString(leer["Actividad"]);
                tservicio = Convert.ToString(leer["TipoServicio"]);
                asesor = Convert.ToString(leer["Asesor1"]);
                folio = Convert.ToString(leer["Folio"]);
                fecha_s.ToShortDateString();
            }
            con.Close();
        }

        public void sentencia()
        {
            SqlCommand cmd = new SqlCommand("Select *from v_FSR where Nollamada=" + lbluser.Text, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.SelectCommand = cmd;
            DataSet objdataset = new DataSet();
            adapter.Fill(objdataset);
            GridView1.DataSource = objdataset;
            GridView1.DataBind();
        }

        protected void btnEnviarRespuesta_Click(object sender, EventArgs e)
        {
            if (datepicker.Text == "")
            {
                Response.Write("<script>alert('Ingresa la nueva Fecha de Servicio');</script>");
                return;
            }

            SqlCommand cmd = new SqlCommand("SaveProponerFecha", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@fecha", SqlDbType.Date);
            cmd.Parameters.Add("@hora", SqlDbType.VarChar);
            cmd.Parameters.Add("@llamada", SqlDbType.Int);

            cmd.Parameters["@fecha"].Value = datepicker.Text;
            cmd.Parameters["@hora"].Value = dplhora.Text;
            cmd.Parameters["@llamada"].Value = lbluser.Text;

            Response.Write("<script language=javascript>if(confirm('Se ha enviado la Notificación proponiendo Nueva Fecha de Servicio el día " + datepicker.Text + "')==true){ location.href='ConfirmacionServicio.aspx?vlrtknfkgt=" + lbluser.Text + "'} else {location.href='ConfirmacionServicio.aspx?vlrtknfkgt=" + lbluser.Text + "'}</script>");

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void bloqueo()
        {
            btnconfirmar.Enabled = false;
            btnEnviarRespuesta.Enabled = false;
        }

        protected void btnconfirmar_Click(object sender, EventArgs e)
        {
            String updatedata = "update fsr set FP_Servicio=getdate(), C_FechaServicio='Y' where nollamada=" + lbluser.Text;
            con.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = updatedata;
            cmd.Connection = con;
            cmd.ExecuteNonQuery();

            Response.Write("<script language=javascript>if(confirm('Se ha enviado la Confirmación para realizar el(los) Servicio(s) programados ')==true){ location.href='ConfirmacionServicio.aspx?vlrtknfkgt=" + lbluser.Text + "'} else {location.href='ConfirmacionServicio.aspx?vlrtknfkgt=" + lbluser.Text + "'}</script>");

            con.Close();
        }

        public void enviacorreo()
        {
            string body = "<p><strong>Estimada(o) " + @asesor + "</strong></p>" +
                "<p>Se informa que se ha confirmado el siguiente servicio:</p>" +
                "<p>&nbsp;</p>" +
                "<p><strong>No. Llamada SAP:</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; " + @llamada + "</p>" +
                "<p><strong>No. Actividad:&nbsp;&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @actividad + "</p>" +
                "<p><strong>Cliente / Empresa:</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @cliente + "</p>" +
                "<p><strong>Tipo de Servicio:&nbsp;&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @tservicio + "</p>" +
                "<p><strong>Fecha de Servicio:&nbsp;&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @fecha_s.ToShortDateString() + "</p>" +
                "<p><strong>Hora:&nbsp;&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @hora + "</p>" +
                "<p><strong>Equipo:&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @equipo + "</p>" +
                "<p><strong>Ingeniero Asignado:&nbsp;</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + @ing + "</p>" +
                "<p>&nbsp;</p>" +
                "<p>Este correo se envia automáticamente, favor de NO responder al mismo</p>" +
                "<p>&nbsp;</p>";

            SmtpClient smtp = new SmtpClient("smtp.inolab.com", 465);
            smtp.Credentials = new NetworkCredential("notificaciones@inolab.com", "Notificaciones2021*");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;   // metodo de entrega
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("notificaciones@inolab.com", "Confirmación de Servicio");     //de donde se envia y display name especie de titulo 
            mail.To.Add(new MailAddress(mailasesor));           //a quien va dirigido
            mail.Bcc.Add("carlosflores@inolab.com");            // copia oculta
            mail.CC.Add("coordinacion-servicio@inolab.com");    // con copia
            mail.Subject = "Se ha confirmado el servicio para el Folio FSR No. " + folio;  // asunto
            mail.IsBodyHtml = true;
            mail.Body = body;

            smtp.Send(mail);
        }
    }
}