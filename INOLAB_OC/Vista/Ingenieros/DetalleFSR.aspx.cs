using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using INOLAB_OC.Modelo;
using INOLAB_OC;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Diagnostics;
public partial class DetalleFSR : Page
{
    int q;
    
    const int FINALIZADO =3;
    const string sinFechaAsignada = "";
    const string sinAccionRegistrada = "";
    protected void Page_Load(object sender, EventArgs e)
    {
       agregarEncabezadosDePanel();
       definirVisibilidadDeBotonesDependiendoEstatusFolio();
       cargarAccionesDelIngeniero();
       llenarInformacionDeRefaccionesActuales();
    }

    public void agregarEncabezadosDePanel()
    {
        if (Session["idUsuario"] == null)
        {
            Response.Redirect("./Sesion.aspx");
        }
        else
        {
            titulo.Text = "Detalle de FSR N°. " + Session["folio_p"].ToString();
            lbluser.Text = Session["nameUsuario"].ToString();

        }
    }
    public void definirVisibilidadDeBotonesDependiendoEstatusFolio()
    {
        string consulta = "SELECT Top 1 IdStatus FROM FSR where Folio= " + Session["folio_p"].ToString() + " and Id_Ingeniero =" + Session["idUsuario"].ToString() + ";";
        int estatusFolioDeServicio = Conexion.getScalar(consulta);
        if (estatusFolioDeServicio == FINALIZADO)
        {
            Btn_agregar_refacciones_a_servicio.Visible = false;
            Checked_verificar_funcionamiento.Visible = true;
            Btn_reportar_falla.Visible = false;
            Btn_vista_previa_reportes.Visible = false;
            Btn_ir_a_servicios_asignados.Visible = true;
        }
        else
        {
            Btn_agregar_refacciones_a_servicio.Visible = true;
            Checked_verificar_funcionamiento.Visible = true;
            Btn_reportar_falla.Visible = true;
            Btn_vista_previa_reportes.Visible = true;
            Btn_ir_a_servicios_asignados.Visible = true;
        }
    }

    private void cargarAccionesDelIngeniero()
    {
        GridView1.DataSource = Conexion.getDataSet("Select * from  FSRAccion where idFolioFSR=" + Session["folio_p"]);
        GridView1.DataBind();
    }

    private void llenarInformacionDeRefaccionesActuales()
    {
        try
        {

            DataSet refacciones = Conexion.getDataSet("select numRefaccion,cantidadRefaccion from Refaccion where idFSR=" + Session["folio_p"] + ";");
            int cuenta = refacciones.Tables[0].Rows.Count;

            if (cuenta > 0)
            {
                foreach (DataRow dataRow in refacciones.Tables[0].Rows)
                {
                    agregarDatosDeRefacciones(dataRow["numRefaccion"].ToString(), dataRow["cantidadRefaccion"].ToString());
                }
            }

        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());

        }
    }

    private void cargardatos()
    {
        
    //Se inicia el servicio en caso de que aun no este iniciado (Fecha sistema)
    Conexion.executeQuery("UPDATE FSR SET Inicio_Servicio = CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) WHERE Folio = " + Session["folio_p"].ToString() + " and Inicio_Servicio is null;");
            
     //Se actualiza el estao de folio a "En proceso"
     Conexion.executeQuery("UPDATE FSR SET IdStatus = 2 WHERE Folio = " + Session["folio_p"].ToString() + " and IdStatus = 1;");
     CHECKED_ESTA_FUNCIONANDO.Checked = verificarEstatusDelEquipo();
        
    }

  

    protected void Agregar_nuevas_acciones_Click(object sender, EventArgs e)
    {
        seccion_nuevo_servicio.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        contenone.Style.Add("filter", "blur(9px)");
        footerid.Style.Add("display", "none");
    }

    protected void Agregar_nueva_actividad_al_reporte_Click(object sender, EventArgs e)
    {
       
        lbl_fecha_nuevo_servicio.Text = "Fecha: ";
        if (Fecha_nueva_accion_realizada.Text.Equals(sinFechaAsignada))
        {
            lbl_fecha_nuevo_servicio.Text = "Favor de ingresar fecha";
        }
        else
        {

          if (txtacciones.Text.Equals(sinAccionRegistrada))
            {
               
                acciones.Text = "Favor de ingresar la acción realizada";
            }
            else
            {
                
                DateTime fechaDeSolicitudDeServicio = Conexion.getDateTime("SELECT FechaServicio FROM FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
                //Comparacion de fechas (no puede hacerlo si la fecha es anterior a la fecha de servicio) funcionalidad pendiente

                    string fechaNuevaAccion, horasDedicadasEnNuevaAccion, nuevaAccionRealizada;
                    fechaNuevaAccion = Fecha_nueva_accion_realizada.Text;
                    horasDedicadasEnNuevaAccion = txthorasD.Text;
                    nuevaAccionRealizada = txtacciones.Text;
                    if (insertarNuevaAccionRealizada(fechaNuevaAccion, horasDedicadasEnNuevaAccion, nuevaAccionRealizada))
                    {
                        string[] vectorFechaNuevaAccion = fechaNuevaAccion.Split('-');
                        string onemod = vectorFechaNuevaAccion[2] + "-" + vectorFechaNuevaAccion[1] + "-" + vectorFechaNuevaAccion[0];

                    cerrarVentanaAgregarNuevaAccion();
                    cargarAccionesDelIngeniero();
                    }
                    else
                    {
                    cerrarVentanaAgregarNuevaAccion();
                    Response.Redirect("DetalleFSR.aspx");
                    }
            }
        }

    }

    private bool insertarNuevaAccionRealizada(String fechaNuevaAccion, String horasDedicadasEnNuevaAccion, String nuevaAccionRealizada)
    {
        try
        {
            
            int filasAfectadasPorUpdate = Conexion.getScalar("Insert into FSRAccion(FechaAccion,HorasAccion,AccionR,idFolioFSR,idUsuario, FechaSistema)" +
                " values(CAST('" + fechaNuevaAccion + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "' AS DATETIME)," + horasDedicadasEnNuevaAccion + ",'" + nuevaAccionRealizada + "'," + Session["folio_p"] + "," + Session["idUsuario"] + ",CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME));");

            return (filasAfectadasPorUpdate == 1) ? true: false;
            
        }
        catch(Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            return false;
        }
    }

    protected void Cerrar_ventana_agregar_nueva_accion_Click(object sender, ImageClickEventArgs e)
    {
        cerrarVentanaAgregarNuevaAccion();
    }

    public void cerrarVentanaAgregarNuevaAccion()
    {
        Fecha_nueva_accion_realizada.Text = "";
        txthorasD.Text = "";
        txtacciones.Text = "";

        seccion_nuevo_servicio.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }


    protected void Buscar_observaciones_folio_servicio_Click(object sender, EventArgs e)
    {
        try
        {
            
            string observacionesDeFolioServicio = Conexion.getText("select Observaciones from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            if (observacionesDeFolioServicio != null)
            {
                txtobservaciones.Text = observacionesDeFolioServicio;
            }

            verificarSiSeEnviaEmailAlAsesor();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            
        }
        finally
        {
            observaciones.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            contenone.Style.Add("filter", "blur(9px)");
            footerid.Style.Add("display", "none");
        }
    }

    private void verificarSiSeEnviaEmailAlAsesor()
    {
        try
        {
            string asesor = Conexion.getText("SELECT NotAsesor FROM FSR where Folio=" + Session["folio_p"] + ";");

            if (asesor == "Si")
            {
                Envio_de_notificacion_de_eliminacion_de_accion.Checked = true;
            }
            else
            {
                Envio_de_notificacion_de_eliminacion_de_accion.Checked = false;
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());

        }
    }

    protected void Btn_Fallas_Encontradas_Click(object sender, EventArgs e)
    {
        try
        {   
            string fallaEncontrada = Conexion.getText("select FallaEncontrada from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");

            if (fallaEncontrada != null)
            {
                txtfallaencontrada.Text = fallaEncontrada;
            }
           
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            
        }
        finally
        {
            FallaEncontrada.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            contenone.Style.Add("filter", "blur(9px)");
            footerid.Style.Add("display", "none");
        }
    }

    protected void Cerrar_campo_observaciones_Click(object sender, ImageClickEventArgs e)
    {
        cerrarCampoObservaciones();
    }

    private void cerrarCampoObservaciones()
    {
        txtobservaciones.Text = "";
        observaciones.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void Cerrar_campo_fallas_encontradas_Click(object sender, ImageClickEventArgs e)
    {
        cerrarCompoFallasEncontradas();
    }

    private void cerrarCompoFallasEncontradas()
    {
        txtfallaencontrada.Text = "";
        FallaEncontrada.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void Actualizar_observaciones_Click(object sender, EventArgs e)
    {
        if(txtobservaciones.Text.Length > 0)
        {
            try
            {
                Conexion.executeQuery(" UPDATE FSR SET Observaciones='" + txtobservaciones.Text + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
                notificarObservacionesAlAsesor();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                cerrarCampoObservaciones();
            }
        }
    }

    private void notificarObservacionesAlAsesor()
    {
        if (Envio_de_notificacion_de_eliminacion_de_accion.Checked == true)
        {
            Conexion.executeQuery("update fsr set NotAsesor = 'Si' where Folio=" + Session["folio_p"].ToString() + ";");
            Session["not_ase"] = "Si";
        }
        else if (Envio_de_notificacion_de_eliminacion_de_accion.Checked == false)
        {
            Conexion.executeQuery("update fsr set NotAsesor = 'No' where Folio=" + Session["folio_p"].ToString() + ";");
            Session["not_ase"] = "No";
        }
    }

    protected void Actualizar_fallas_encontradas_Click(object sender, EventArgs e)
    {
        if (txtfallaencontrada.Text.Length > 0)
        {
            try
            {
                Conexion.executeQuery(" UPDATE FSR SET FallaEncontrada='" + txtfallaencontrada.Text + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                cerrarCompoFallasEncontradas();
            }
        }
    }

    protected void Btn_Vista_Previa_Click(object sender, EventArgs e)
    {
        try
        {
            verificarSiServicioFuncionaCorrectamente();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
        finally
        {
            Response.Redirect("VistaPrevia.aspx");
        }
    }

    private void verificarSiServicioFuncionaCorrectamente()
    {
        string texto = "Si";
        if (CHECKED_ESTA_FUNCIONANDO.Checked)
        {
            texto = "Si";
        }
        else
        {
            texto = "No";
        }

        Conexion.executeQuery(" UPDATE FSR SET Funcionando='" + texto + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
    }

    protected bool verificarEstatusDelEquipo()
    {
        try
        {   
            string texto = Conexion.getText(" SELECT Funcionando FROM FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
            
            if (texto == "Si")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            return false;
        }
    }

    protected void Verificacion_de_estatus_esta_o_no_funcionando_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            string texto = "Si";
            if (CHECKED_ESTA_FUNCIONANDO.Checked)
            {
                texto = "Si";
            }
            else
            {
                texto = "No";
            }
            Conexion.executeQuery(" UPDATE FSR SET Funcionando='" + texto + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";");
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }

   
    protected void Agregar_refaccion_a_base_de_datos_Click(object sender, EventArgs e)
    { 
        string numeroDePartes, descripcionDeRefacion, cantidadDeRefacciones;
        numeroDePartes = txtbox_numero_de_partes.Text;
        descripcionDeRefacion = txtbox_descripcion_refaccion.Text;
        cantidadDeRefacciones = txtbox_cantidad_refaccion.Text;
        if (numeroDePartes.Length > 0)
        {
            if (descripcionDeRefacion.Length > 0)
            {
                if (cantidadDeRefacciones.Length > 0)
                {
                    if (insertarRefaccion(numeroDePartes, cantidadDeRefacciones, descripcionDeRefacion))
                    {
                        agregarDatosDeRefacciones(numeroDePartes, cantidadDeRefacciones);
                        cerrarVentanaDeNuevaRefaccion();
                    }
                }
                else
                    Response.Write("<script>alert('Favor de llenar todos los campos');</script>");
            }
            else
                Response.Write("<script>alert('Favor de llenar todos los campos');</script>");
        }
        else
            Response.Write("<script>alert('Favor de llenar todos los campos');</script>");
    }

    private void agregarDatosDeRefacciones(string no, string num)
    {
        try
        {
            TableRow row = new TableRow();
            TableCell[] cell1 = new TableCell[2];
            cell1[0] = new TableCell();
            cell1[1] = new TableCell();
            cell1[0].Text = no;
            cell1[1].Text = num + " pieza(s)";
            row.Cells.AddRange(cell1);
            row.Style.Add("HorizontalAlign", "Center");
            Table1.Rows.Add(row);
        }
        catch(Exception e)
        {
            Response.Write("<script>alert('Error al agregar a la tabla de datos');</script>");
        }
    }


    protected bool insertarRefaccion(string numeroDePartes, string cantidadDeRefacciones, string descripcionDeRefacion)
    {
        try
        {

            int numeroDeFilasAfectadas = Conexion.getNumberOfRowsAfected("Insert into Refaccion(numRefaccion,cantidadRefaccion,descRefaccion,idFSR)" +
                " values('" + numeroDePartes + "'," + cantidadDeRefacciones + ",'" + descripcionDeRefacion + "'," + Session["folio_p"] + ");");
            
            return numeroDeFilasAfectadas == 1?  true : false;
            
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            return false;
        }
    }


    protected void Mostrar_ventana_refacciones_Click(object sender, EventArgs e)
    {
        refacciones.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        contenone.Style.Add("filter", "blur(9px)");
        footerid.Style.Add("display", "none");
    }


    protected void Cerrar_Ventana_Refacciones_Click(object sender, ImageClickEventArgs e)
    {
        txtbox_numero_de_partes.Text = "";
        txtbox_cantidad_refaccion.Text = "";
        txtbox_descripcion_refaccion.Text = "";

        refacciones.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void Cerrar_ventana_nueva_refaccion_Click(object sender, ImageClickEventArgs e)
    {
        cerrarVentanaDeNuevaRefaccion();
    }

    private void cerrarVentanaDeNuevaRefaccion()
    {
        txtbox_numero_de_partes.Text = "";
        txtbox_cantidad_refaccion.Text = "";
        txtbox_descripcion_refaccion.Text = "";

        SECCION_AGREGAR_REFACCION.Style.Add("display", "none");
        refacciones.Style.Add("display", "block");
    }

    protected void Mostrar_nueva_refaccion_Click(object sender, EventArgs e)
    {       
        refacciones.Style.Add("display", "none");
        SECCION_AGREGAR_REFACCION.Style.Add("display", "block");
    }

    protected void Agrendar_proximo_servicio_Click(object sender, EventArgs e)
    {
        //Guarda la fecha de proximo servicio que se haya insertado (Si se oprimio sin haber seleccionado una fecha antes aparecera como 1999-01-01)
        Conexion.executeQuery("Update FSR set Proximo_Servicio='" + datepicker1.Text + "' where Folio=" + Session["folio_p"]);
    }

    public void GridView_de_acciones_realizadas_en_folio_OnRowComand(object sender, GridViewCommandEventArgs e)
    {
        //Al darle clic al folio deseado este se almacena en la sesión y te redirige a la ventana de FSR
        try
        {
            if (e.CommandName == "Borrar")
            {
                //Me trae el indice del datagrid que seleccione 
                e.CommandArgument.ToString();
                q = Convert.ToInt32(e.CommandArgument.ToString());

                string idFolioDeAccion;
                idFolioDeAccion = GridView1.Rows[q].Cells[0].Text.ToString();                
                
                //Busqueda de los campos de la accion 
                
                string accionEnServicio = Conexion.getText("select AccionR from FSRAccion where idFSRAccion=" + idFolioDeAccion + ";");
                string fechaDeAccion = Conexion.getText("select convert(varchar, FechaAccion, 105) as FechaAccion from FSRAccion where idFSRAccion=" + idFolioDeAccion + ";");
                
                int horaDeAccion = Conexion.getScalar("select HorasAccion from FSRAccion where idFSRAccion=" + idFolioDeAccion + ";");
                int idFolioServicioAccion = Conexion.getScalar("select IDFSRAccion from FSRAccion where idFSRAccion=" + idFolioDeAccion + ";");
               
                string idFolioDeServicio = Conexion.getText("select idFolioFSR from FSRAccion where idFSRAccion=" + idFolioDeAccion + ";");
                string tipoDeServicio = Conexion.getText("select TipoServicio from v_fsr where Folio=" + idFolioDeServicio.ToString() + ";");
               

                fol.Text = idFolioDeServicio;
                serv.Text = tipoDeServicio;
                descacci.Text = accionEnServicio;
                fechacci.Text = fechaDeAccion;
                horaacci.Text = horaDeAccion.ToString();
                IDAccion.Text = idFolioServicioAccion.ToString();
                avisodel.Style.Add("display", "block");
                headerone.Style.Add("display", "none");
                footerid.Style.Add("display", "none");
                contenone.Style.Add("display", "none");
            }
        }
        catch (SqlException ex)
        {
            Console.Write(ex.ToString());
        }
    }

    public void Borrar_accion_realizada_Click(object sender, EventArgs e)
    {
        //Codigo para borrar la accion realizada

        Conexion.executeQuery("delete from FSRAccion where idFSRAccion =" + IDAccion.Text + ";");
        cargarAccionesDelIngeniero();

        avisodel.Style.Add("display", "none");
        headerone.Style.Add("display", "block");
        footerid.Style.Add("display", "flex");
        contenone.Style.Add("display", "block");
    }

    //Enseñar a Carlos y Sandra borrarnobtn_Click
    protected void Cancelar_proceso_de_eliminar_accion_Click(object sender, EventArgs e)
    { 
        avisodel.Style.Add("display", "none");
        headerone.Style.Add("display", "block");
        footerid.Style.Add("display", "flex");
        contenone.Style.Add("display", "block");
    }


    protected void Verificar_si_se_envio_notificacion_a_usuario_CheckedChanged(object sender, EventArgs e)
    {
        if (Envio_de_notificacion_de_eliminacion_de_accion.Checked == true)
        {  
            Conexion.executeQuery("update fsr set NotAsesor = 'Si' where Folio=" + Session["folio_p"].ToString() + ";");
            Session["not_ase"] = "Si";
        }
        else if(Envio_de_notificacion_de_eliminacion_de_accion.Checked == false)
        {
            Conexion.executeQuery("update fsr set NotAsesor = 'No' where Folio=" + Session["folio_p"].ToString() + ";");
            Session["not_ase"] = "No";
        }
    }

    protected void Ir_a_servicios_asignados_Click(object sender, EventArgs e)
    {
        Response.Redirect("ServiciosAsignados.aspx");
    }
}