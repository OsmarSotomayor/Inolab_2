using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace INOLAB_OC
{
    public partial class Servicio_OC1 : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

        protected void Page_Load(object sender, EventArgs e)
        {
            //string valor = Request.QueryString["valor"];

            //if (Request.Params["valor"] != null)
            //{               
            //    lbluser.Text = Request.Params["valor"];
            //    lblidarea.Text = Request.Params["idar"];
            //    lblrol.Text = Request.Params["idr"];
            //}
            if (Session["valor"] == null)
            {
                Response.Redirect("./Sesion.aspx");

            }
            else
            {
                //titulo.Text = "Detalle de FSR N°. " + Session["folio_p"].ToString();
                lbluser.Text = Session["valor"].ToString();
                lblidarea.Text = Session["idar"].ToString();
                lblrol.Text = Session["idr"].ToString();
            }
            if (lblrol.Text == "2")
            {
                btnOC_Servicio.Visible = false;
                btnOC_Equipo.Visible = false;
                //Button2.Visible = false;
            }
            else
            {
                btnOC_Servicio.Visible = true;
                btnOC_Equipo.Visible = true;
                //Button2.Visible = true;
            }
            btnInforme_A.Visible = true;

        }

        protected void txtImporte_TextChanged(object sender, EventArgs e)
        {

        }

        //Boton Guardar Registro
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            //decimal mxn = Convert.ToDecimal(txtImporte.Text);
            //decimal usd = Convert.ToDecimal(txtImporte.Text);
            //decimal op=0;

            //if(cmbMoneda.Text=="MXN")
            //{
            //    op =mxn/19;
            //}
            //if (cmbMoneda.Text == "USD")
            //{
            //    op = usd;
            //}
            if (cmbservicio.Text == "Contrato" && cmbtipo.Text == "")
            {
                Response.Write("<script>alert('Selecciona el Tipo de Contrato');</script>");
                return;
            }
            if (cmbservicio.Text == "Contrato" && cmbTiempo.Text == "")
            {
                Response.Write("<script>alert('Selecciona los meses del Contrato');</script>");
                return;
            }

            adjunto1.Text = FileUpload1.FileName;
            adjunto2.Text = FileUpload2.FileName;
            adjunto3.Text = FileUpload3.FileName;
            adjunto4.Text = FileUpload4.FileName;
            adjunto5.Text = FileUpload5.FileName;
            string val;
            if (chValidar.Checked == false)
            {
                Response.Write("<script>alert('Activa la casilla Autorizar Informacion para validar el Registro');</script>");
                return;
            }
            else
            {
                val = "true";
            }

            if (FileUpload1.HasFile == true)
            {
                FileUpload1.SaveAs(Server.MapPath("~/Adjuntos/" + FileUpload1.FileName));
            }
            if (FileUpload2.HasFile == true)
            {
                FileUpload2.SaveAs(Server.MapPath("~/Adjuntos/" + FileUpload2.FileName));
            }
            if (FileUpload3.HasFile == true)
            {
                FileUpload3.SaveAs(Server.MapPath("~/Adjuntos/" + FileUpload3.FileName));
            }
            if (FileUpload4.HasFile == true)
            {
                FileUpload4.SaveAs(Server.MapPath("~/Adjuntos/" + FileUpload4.FileName));
            }
            if (FileUpload5.HasFile == true)
            {
                FileUpload5.SaveAs(Server.MapPath("~/Adjuntos/" + FileUpload5.FileName));
            }

            SqlCommand cmd = new SqlCommand("SaveServicioOC", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@idcliente", SqlDbType.Int);
            cmd.Parameters.Add("@cliente", SqlDbType.VarChar);
            cmd.Parameters.Add("@importe", SqlDbType.Decimal);
            cmd.Parameters.Add("@moneda", SqlDbType.VarChar);
            cmd.Parameters.Add("@servicio_t", SqlDbType.VarChar);
            cmd.Parameters.Add("@fecha_R", SqlDbType.Date);
            cmd.Parameters.Add("@fecha_oc", SqlDbType.Date);
            cmd.Parameters.Add("@usuario", SqlDbType.VarChar);
            cmd.Parameters.Add("@adjunto1", SqlDbType.VarChar);
            cmd.Parameters.Add("@adjunto2", SqlDbType.VarChar);
            cmd.Parameters.Add("@adjunto3", SqlDbType.VarChar);
            cmd.Parameters.Add("@adjunto4", SqlDbType.VarChar);
            cmd.Parameters.Add("@adjunto5", SqlDbType.VarChar);
            cmd.Parameters.Add("@nota", SqlDbType.VarChar);
            cmd.Parameters.Add("@oc", SqlDbType.VarChar);
            cmd.Parameters.Add("@tipo_s", SqlDbType.VarChar);
            cmd.Parameters.Add("@validar_A", SqlDbType.VarChar);
            cmd.Parameters.Add("@mes", SqlDbType.VarChar);

            cmd.Parameters["@idcliente"].Value = cmbEmpresa.SelectedValue;
            cmd.Parameters["@cliente"].Value = lblempresa.Text;
            cmd.Parameters["@importe"].Value = txtImporte.Text;
            cmd.Parameters["@moneda"].Value = cmbMoneda.Text;
            cmd.Parameters["@servicio_t"].Value = cmbservicio.Text;

            if (datepicker.Text == "")
            {

                cmd.Parameters["@fecha_R"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@fecha_R"].Value = Convert.ToDateTime(datepicker.Text);
            }
            if (datepicker.Text == "")
            {

                cmd.Parameters["@fecha_oc"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@fecha_oc"].Value = Convert.ToDateTime(datepicker2.Text);
            }

            cmd.Parameters["@usuario"].Value = lbluser.Text;
            cmd.Parameters["@adjunto1"].Value = adjunto1.Text;
            cmd.Parameters["@adjunto2"].Value = adjunto2.Text;
            cmd.Parameters["@adjunto3"].Value = adjunto3.Text;
            cmd.Parameters["@adjunto4"].Value = adjunto4.Text;
            cmd.Parameters["@adjunto5"].Value = adjunto5.Text;
            cmd.Parameters["@nota"].Value = txtNota.Text;
            cmd.Parameters["@oc"].Value = txtOC.Text;
            cmd.Parameters["@tipo_s"].Value = cmbtipo.Text;

            cmd.Parameters["@validar_A"].Value = val;
            cmd.Parameters["@mes"].Value = cmbTiempo.Text;

            //Response.Write("<script>alert('Registro Guardado Exitosamente');</script>");

            string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            //Response.Redirect("Servicio_OC.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);

            //Response.Write("<script language=javascript>if(confirm('Registro Guardado Exitosamente')==true){ location.href='Servicio_OC.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr + "'} else {location.href='Servicio_OC.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr + "'}</script>");
            Response.Write("<script language=javascript>if(confirm('Registro Guardado Exitosamente')==true){ location.href='Servicio_OC1.aspx'} else {location.href='Servicio_OC1.aspx'}</script>");


            con.Open();
            cmd.ExecuteNonQuery();

            //string cli = cmbEmpresa.Text, imp = txtImporte.Text, mon = cmbMoneda.Text, oc = txtOC.Text, ser = cmbservicio.Text, fr = datepicker.Text, foc = datepicker2.Text, not = txtNota.Text, a1 = adjunto1.Text, a2 = adjunto2.Text, a3 = adjunto3.Text, a4 = adjunto4.Text, a5 = adjunto5.Text, us = lbluser.Text,ti=cmbtipo.Text;
            //string _open = "window.open('RegistroPrevio.aspx?cli=" + cli + "&imp=" + imp + "&mon=" + mon + "&oc=" + oc + "&ser=" + ser + "&fr=" + fr + "&foc=" + foc + "&not=" + not + "&a1=" + a1 + "&a2=" + a2 + "&a3=" + a3 + "&a4=" + a4 + "&a5=" + a5 + "&us=" + us + "&ti="+ ti+"','_blank');";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), _open, true);
            //Limpiar();

            //Response.Write("<script>alert('Registro Guardado Exitosamente');</script>");
            //string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            //Response.Redirect("Servicio_OC.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);
        }
        //Limpia Datos
        public void Limpiar()
        {
            cmbEmpresa.Text = null;
            txtImporte.Text = "0";
            cmbMoneda.Text = null;
            cmbservicio.Text = null;
            datepicker.Text = null;
            datepicker2.Text = null;
            txtOC.Text = null;
            txtNota.Text = null;

            adjunto1.Text = null;
            adjunto2.Text = null;
            adjunto3.Text = null;
            adjunto4.Text = null;
            adjunto5.Text = null;
            cmbtipo.Visible = false;
            chValidar.Checked = false;

        }

        //Boton Salir
        protected void Button1_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Sesion.aspx");
        }

        //Boton Registros OC Servicio
        protected void Button2_Click(object sender, EventArgs e)
        {
            string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            //Response.Redirect("Registros.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);
            Response.Redirect("Registros1.aspx");
        }

        //Combo servicio
        protected void cmbservicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbservicio.Text == "Contrato")
            {
                cmbtipo.Visible = true;
                cmbTiempo.Visible = true;
            }
            else
            {
                cmbtipo.Visible = false;
                cmbtipo.Text = null;
                cmbTiempo.Visible = false;
            }

        }

        protected void Button4_Click1(object sender, EventArgs e)
        {
            string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            //Response.Redirect("Registros_Equipos.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);
            Response.Redirect("Registro_Equipos1.aspx");
        }

        protected void btnNuevoOC_Equipo_Click(object sender, EventArgs e)
        {

        }

        protected void cmbMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cmbtipo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cmbtipo_SelectedIndexChanged1(object sender, EventArgs e)
        {

        }

        protected void txtNota_TextChanged(object sender, EventArgs e)
        {

        }

        protected void chValidar_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void Button3_Click(object sender, EventArgs e)
        {

        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            //string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            //Response.Redirect("Equipo_OC.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);
            //Response.Redirect("Equipo_OC1.aspx");
        }

        protected void BuscarEmpresa_Click(object sender, EventArgs e)
        {

        }

        protected void txtNota_TextChanged1(object sender, EventArgs e)
        {

        }

        protected void datepicker_TextChanged(object sender, EventArgs e)
        {

        }

        protected void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TextBox1.Text = cmbEmpresa.SelectedValue;
            //cmbem.Text = cmbEmpresa.Text;
            lblempresa.Text = cmbEmpresa.SelectedItem.ToString();
        }

        protected void txtNota_TextChanged2(object sender, EventArgs e)
        {

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void cmbem_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox1_TextChanged1(object sender, EventArgs e)
        {

        }

        protected void btnNuevaEmpresa_Click(object sender, EventArgs e)
        {
            //string valor = lbluser.Text, idar = lblidarea.Text, idr = lblrol.Text;
            ////Response.Redirect("Clientes.aspx?valor=" + valor + "&idar=" + idar + "&idr=" + idr);
            //Response.Redirect("Clientes.aspx");

        }

        protected void btnInforme_A_Click(object sender, EventArgs e)
        {
            //string valor = lbluser.Text;
            ////Response.Redirect("Informe_A.aspx?valor=" + valor);
            ////Response.Write("<script> window.open('Adjuntos/" + adjunto4.Text + "','_blank'); </script>");
            ////Response.Write("<script> window.open('Informe_A.aspx?valor=" + valor + "','_blank'); </script>");
            //Response.Write("<script> window.open('Informe_A.aspx','_blank'); </script>");
        }


    }
}