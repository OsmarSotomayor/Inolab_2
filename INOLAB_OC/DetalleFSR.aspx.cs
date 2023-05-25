using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

public partial class DetalleFSR : Page
{
    int q;
    string query;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["idUsuario"] == null)
        {
            Response.Redirect("./Sesion.aspx");
        }
        else
        {
            //Linea para poner en titulo el numero del folio (ya sin error de repeticion del numero)
            titulo.Text = "Detalle de FSR N°. " + Session["folio_p"].ToString();
            lbluser.Text = Session["nameUsuario"].ToString();
            try
            {
                //Me trae el estatus actual del equipo para el check
                con.Open();
                SqlCommand comando2 = new SqlCommand("select Funcionando from FSR where Folio=" + Session["folio_p"] + ";", con);
                string fun = (string)comando2.ExecuteScalar();
                con.Close();
                //Para cambiar el estado del checken caso de que este en funcionamiento el equipo
                if (fun == "Si")
                {
                    chkOnOff.Checked = true;
                }
            }
            catch (Exception es)
            {
                con.Close();
            }
        }

        //Linea de comandos para ocultar los elementos inecesarios, una vez que se est finalizado el folio (solo para hacer cambios a anexos, proximo servicio y observaciones)
        con.Open();
        SqlCommand estatus = new SqlCommand("SELECT Top 1 IdStatus FROM FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
        int status = (int)estatus.ExecuteScalar();
        con.Close();
        if (status == 3)
        {
            AddRef.Visible = false;
            sirvebutton.Visible = false;
            AddFalla.Visible = false;
            vpbuttonid.Visible = false;
            SA.Visible = true;
        }
        else
        {
            AddRef.Visible = true;
            sirvebutton.Visible = true;
            AddFalla.Visible = true;
            vpbuttonid.Visible = true;
            SA.Visible = false;
        }

        //Carga los datos de los anexos correspondientes al folio
        cargardatos2();
        loadtable2();
    }

    //Coneccion a la base de datos (para hacer pruebas usar BrowserPruebas)
    SqlConnection con = new SqlConnection(@"Data Source=INOLABSERVER03;Initial Catalog=Browser;Persist Security Info=True;User ID=ventas;Password=V3ntas_17");

    private void cargardatos()
    {
        try
        {
            //Se inicia el servicio en caso de que aun no este iniciado (Fecha sistema)
            con.Open();
            SqlCommand inserthora = new SqlCommand("UPDATE FSR SET Inicio_Servicio = CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME) WHERE Folio = " + Session["folio_p"].ToString() + " and Inicio_Servicio is null;", con);
            inserthora.ExecuteNonQuery();
            con.Close();

            //Se actualiza el estao de folio a "En proceso"
            con.Open();
            SqlCommand updatestatus = new SqlCommand("UPDATE FSR SET IdStatus = 2 WHERE Folio = " + Session["folio_p"].ToString() + " and IdStatus = 1;", con);
            updatestatus.ExecuteNonQuery();
            con.Close();
            chkOnOff.Checked = estaFuncionando();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
    }

    private void cargardatos2()
    {//Carga los folios del ingeniero
        try
        {
            //Llena los datos de los acciones segun el folio
            SqlCommand cmd = new SqlCommand("Select *from  FSRAccion where idFolioFSR=" + Session["folio_p"], con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.SelectCommand = cmd;
            DataSet objdataset = new DataSet();
            adapter.Fill(objdataset);

            //Se actualiza el datagrid para mostrar todas las acciones
            GridView1.DataSource = objdataset;
            GridView1.DataBind();
        }
        catch (Exception e)
        {
            Response.Write(e.ToString());
        }
    }

    protected void Nuevo_Click(object sender, EventArgs e)
    {//Despliega el espacio para agregar nuevas acciones
        floatsection.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        contenone.Style.Add("filter", "blur(9px)");
        footerid.Style.Add("display", "none");
    }

    protected void Addbutton_Click(object sender, EventArgs e)
    {//Agrega la actividad nueva realizada al reporte
        String one, dos, tres;
        fechad.Text = "Fecha: ";
        //Si no ha insertado una fecha
        if (datepicker.Text == "")
        {
            fechad.Text = "Favor de ingresar fecha";
        }
        else
        {
            //Si no ha insertado alguna accion
            if (txtacciones.Text == "")
            {
                acciones.Text = "Favor de ingresar la acción realizada";
            }
            else
            {
                one = datepicker.Text;

                //Obtener la fecha de solicitud del servicio
                con.Open();
                SqlCommand datever = new SqlCommand("SELECT FechaServicio FROM FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
                DateTime fechasol = (DateTime)datever.ExecuteScalar();
                con.Close();

                //Comparacion de fechas (no puede hacerlo si la fecha es anterior a la fecha de servicio)
                //int result = DateTime.Compare(Convert.ToDateTime(fechasol), Convert.ToDateTime(one));

                //if (result > 0)
                //{
                //    Response.Write("<script>alert('No se puede añadir una acción si la fecha de esta es anterior a la fecha del servicio.');</script>");

                //}
                //else
                //{
                    //Se hace la insercion a una nueva accion con los campos que captura el formulario
                    dos = txthorasD.Text;
                    tres = txtacciones.Text;
                    if (InsertDB(one, dos, tres))
                    {
                        string[] subs = one.Split('-');
                        string onemod = subs[2] + "-" + subs[1] + "-" + subs[0];
                        datepicker.Text = "";
                        txthorasD.Text = "";
                        txtacciones.Text = "";
                        floatsection.Style.Add("display", "none");
                        contenone.Style.Add("filter", "blur(0)");
                        headerone.Style.Add("filter", "blur(0)");
                        footerid.Style.Add("display", "flex");
                        cargardatos2();
                    }
                    else
                    {

                        datepicker.Text = "";
                        txthorasD.Text = "";
                        txtacciones.Text = "";
                        floatsection.Style.Add("display", "none");
                        contenone.Style.Add("filter", "blur(0)");
                        headerone.Style.Add("filter", "blur(0)");
                        footerid.Style.Add("display", "flex");
                    }
                //}
                
            }
        }

    }

    protected bool InsertDB(String uno, String dos, String tres)
    {
        //Inserta una nueva acción realizada 
        try
        {
            con.Open();
            SqlCommand verificarhora = new SqlCommand("Insert into FSRAccion(FechaAccion,HorasAccion,AccionR,idFolioFSR,idUsuario, FechaSistema)" +
                " values(CAST('" + uno + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "' AS DATETIME)," + dos + ",'" + tres + "'," + Session["folio_p"] + "," + Session["idUsuario"] + ",CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AS DATETIME));", con);
            int c = verificarhora.ExecuteNonQuery();
            con.Close();
            if (c == 1) return true;
            else return false;
        }
        catch(Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            con.Close();
            return false;
        }
    }

    protected void closeimg_Click(object sender, ImageClickEventArgs e)
    {
        //Cierra la ventana de agregar nueva acción 
        datepicker.Text = "";
        txthorasD.Text = "";
        txtacciones.Text = "";
        floatsection.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void observacionesbtn_Click(object sender, EventArgs e)
    {
        try
        {
            //hace busqueda de si existen observaciones en el folio FSR correspondiente y si las hay, las incerta en el textbox
            con.Open();
            SqlCommand comando = new SqlCommand("select Observaciones from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object observ = comando.ExecuteScalar();

            if (observ != DBNull.Value)
            {
                txtobservaciones.Text = (string)observ;
            }
            con.Close();

            try
            {
                //Checa si se le notificara al asesor o no (Enviar SendMail2 en CargaFin)
                con.Open();
                SqlCommand comando1 = new SqlCommand("SELECT NotAsesor FROM FSR where Folio=" + Session["folio_p"] + ";", con);
                string asesor = (string)comando1.ExecuteScalar();
                con.Close();
                if (asesor == "Si")
                {
                    Chck.Checked = true;
                }
                else
                {
                    Chck.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                con.Close();
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
        finally
        {
            observaciones.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            contenone.Style.Add("filter", "blur(9px)");
            footerid.Style.Add("display", "none");
        }
    }

    protected void fallasbtn_Click(object sender, EventArgs e)
    {
        //Selecciona las fallas registradas en la base de datos
        try
        {
            con.Open();
            SqlCommand comando = new SqlCommand("select FallaEncontrada from FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            object observ = comando.ExecuteScalar();

            if (observ != DBNull.Value)
            {
                txtfallaencontrada.Text = (string)observ;
            }
            con.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
        finally
        {
            FallaEncontrada.Style.Add("display", "block");
            headerone.Style.Add("filter", "blur(9px)");
            contenone.Style.Add("filter", "blur(9px)");
            footerid.Style.Add("display", "none");
        }
    }

    protected void closebtn1_Click(object sender, ImageClickEventArgs e)
    {
        //Cierra el campo de observaciones
        txtobservaciones.Text = "";
        observaciones.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void closebtn2_Click(object sender, ImageClickEventArgs e)
    {
        //Cierra el campo de fallas encontradas
        txtfallaencontrada.Text = "";
        FallaEncontrada.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void btnguardar_Click(object sender, EventArgs e)
    {
        //Actualiza las observaciones encontradas en el campo de observaciones y si se le notificara o no al asesor de ventas 
        if(txtobservaciones.Text.Length > 0)
        {
            try
            {
                con.Open();
                SqlCommand comando = new SqlCommand(" UPDATE FSR SET Observaciones='" + txtobservaciones.Text + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
                comando.ExecuteNonQuery();
                con.Close();

                if (Chck.Checked == true)
                {
                    
                    con.Open();
                    SqlCommand com5 = new SqlCommand("update fsr set NotAsesor = 'Si' where Folio=" + Session["folio_p"].ToString() + ";", con);
                    com5.ExecuteNonQuery();
                    con.Close();

                    Session["not_ase"] = "Si";
                }
                else if (Chck.Checked == false)
                {
                    con.Open();
                    SqlCommand com6 = new SqlCommand("update fsr set NotAsesor = 'No' where Folio=" + Session["folio_p"].ToString() + ";", con);
                    com6.ExecuteNonQuery();
                    con.Close();
                    Session["not_ase"] = "No";
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                con.Close();
            }
            finally
            {
                txtobservaciones.Text = "";
                observaciones.Style.Add("display", "none");
                contenone.Style.Add("filter", "blur(0)");
                headerone.Style.Add("filter", "blur(0)");
                footerid.Style.Add("display", "flex");
            }
        }
    }

    protected void btnguardarfalla_Click(object sender, EventArgs e)
    {
        //Actualiza una falla encontrada en el campo de fallas
        if (txtfallaencontrada.Text.Length > 0)
        {
            try
            {
                con.Open();
                SqlCommand comando = new SqlCommand(" UPDATE FSR SET FallaEncontrada='" + txtfallaencontrada.Text + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
                comando.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                con.Close();
            }
            finally
            {
                txtfallaencontrada.Text = "";
                FallaEncontrada.Style.Add("display", "none");
                contenone.Style.Add("filter", "blur(0)");
                headerone.Style.Add("filter", "blur(0)");
                footerid.Style.Add("display", "flex");
            }
        }
    }

    protected void vpbutton_Click(object sender, EventArgs e)
    {
        //Actualiza si se encuentra funcionando correctamente o no el servicio y redirige a la página de Vista Previa
        try
        {
            string texto = "Si";
            if (chkOnOff.Checked)
            {
                texto = "Si";
            }
            else
            {
                texto = "No";
            }

            con.Open();
            SqlCommand comando = new SqlCommand(" UPDATE FSR SET Funcionando='" + texto + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            comando.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
        finally
        {
            Response.Redirect("VistaPrevia.aspx");
        }
    }

    protected bool estaFuncionando()
    {
        //Funcion para conocer si el equipo y tenia algun estatus de "Funcionando" para poner el chck en algun estado (si o no)
        try
        {   con.Open();
            SqlCommand comando = new SqlCommand(" SELECT Funcionando FROM FSR where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            string texto = (string)comando.ExecuteScalar();
            con.Close();
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
            con.Close();
            return false;
        }
    }

    protected void chkOnOff_CheckedChanged(object sender, EventArgs e)
    {
        //Actualizar el estado del check de si esta funcionando o no
        try
        {
            string texto = "Si";
            if (chkOnOff.Checked)
            {
                texto = "Si";
            }
            else
            {
                texto = "No";
            }

            con.Open();
            SqlCommand comando = new SqlCommand(" UPDATE FSR SET Funcionando='" + texto + "' where Folio=" + Session["folio_p"] + " and Id_Ingeniero =" + Session["idUsuario"] + ";", con);
            comando.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
    }

    protected void btnrefaccion_Click(object sender, EventArgs e)
    {
        //Se realiza la inserción en la BD onteniendo los datos del formulario. 
        string no, desc, num;
        no = textboxidrefaccion.Text;
        desc = textboxdescrefaccion.Text;
        num = textboxnumrefaccion.Text;
        if (no.Length > 0)
        {
            if (desc.Length > 0)
            {
                if (num.Length > 0)
                {
                    if (insertRefaccion(no, num, desc))
                    {
                        AddRowRef(no, num);
                        textboxidrefaccion.Text = "";
                        textboxnumrefaccion.Text = "";
                        textboxdescrefaccion.Text = "";
                        SectionNewRef.Style.Add("display", "none");
                        refacciones.Style.Add("display", "block");
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

    private void AddRowRef(string no, string num)
    {
        //Agrega los datos de refacciones 
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


    protected bool insertRefaccion(string no, string num, string desc)
    {
        //Proceso de insert a la tabla de refacciones
        try
        {
            con.Open();
            SqlCommand insertREF = new SqlCommand("Insert into Refaccion(numRefaccion,cantidadRefaccion,descRefaccion,idFSR)" +
                " values('" + no +"'," + num + ",'" + desc + "'," + Session["folio_p"] + ");", con);
            int c = insertREF.ExecuteNonQuery();
            con.Close();
            if (c == 1) return true;
            else return false;
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error al cargar la información');</script>");
            Console.Write(ex.ToString());
            con.Close();
            return false;
        }
    }

    private void loadtable2()
    {
        //Llena la información de las refacciones que se tienen actualmente
        try
        {
            con.Open();
            SqlCommand comando = new SqlCommand("select numRefaccion,cantidadRefaccion from Refaccion where idFSR=" + Session["folio_p"] + ";", con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(comando);
            int count = da.Fill(ds, "tabladetalle");
            if (count > 0)
            {
                foreach (DataRow dr in ds.Tables["tabladetalle"].Rows)
                {
                    AddRowRef(dr["numRefaccion"].ToString(), dr["cantidadRefaccion"].ToString());
                }
            }
            con.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            con.Close();
        }
    }


    protected void addrefbtn_Click(object sender, EventArgs e)
    {
        //Muestra la ventana de refacciones
        refacciones.Style.Add("display", "block");
        headerone.Style.Add("filter", "blur(9px)");
        contenone.Style.Add("filter", "blur(9px)");
        footerid.Style.Add("display", "none");
    }


    protected void closeimg1_Click(object sender, ImageClickEventArgs e)
    {
        //Cierra la ventana de refacciones
        textboxidrefaccion.Text = "";
        textboxnumrefaccion.Text = "";
        textboxdescrefaccion.Text = "";
        refacciones.Style.Add("display", "none");
        contenone.Style.Add("filter", "blur(0)");
        headerone.Style.Add("filter", "blur(0)");
        footerid.Style.Add("display", "flex");
    }

    protected void closeimg2_Click(object sender, ImageClickEventArgs e)
    {
        //Cierra la ventana de nueva refacción
        textboxidrefaccion.Text = "";
        textboxnumrefaccion.Text = "";
        textboxdescrefaccion.Text = "";
        SectionNewRef.Style.Add("display", "none");
        refacciones.Style.Add("display", "block");

    }

    protected void btnNuevoR_Click(object sender, EventArgs e)
    {
        //Muestra la creación de una nueva refacción        
        refacciones.Style.Add("display", "none");
        SectionNewRef.Style.Add("display", "block");
    }

    protected void btnProxServicio_Click(object sender, EventArgs e)
    {
        //Guarda la fecha de proximo servicio que se haya insertado (Si se oprimio sin haber seleccionado una fecha antes aparecera como 1999-01-01)
        con.Open();
        SqlCommand prox = new SqlCommand("Update FSR set Proximo_Servicio='" + datepicker1.Text + "' where Folio=" + Session["folio_p"], con);
        prox.ExecuteNonQuery();
        con.Close();
    }

    public void GridView1_OnRowComand(object sender, GridViewCommandEventArgs e)
    {
        //Al darle clic al folio deseado este se almacena en la sesión y te redirige a la ventana de FSR
        try
        {
            if (e.CommandName == "Borrar")
            {
                //Me trae el indice del datagrid que seleccione 
                e.CommandArgument.ToString();
                q = Convert.ToInt32(e.CommandArgument.ToString());
                query = GridView1.Rows[q].Cells[0].Text.ToString();                
                
                //Busqueda de los campos de la accion 
                con.Open();
                SqlCommand com = new SqlCommand("select AccionR from FSRAccion where idFSRAccion=" + query + ";", con);
                string accion = (string)com.ExecuteScalar();
                con.Close();
                con.Open();
                SqlCommand com1 = new SqlCommand("select convert(varchar, FechaAccion, 105) as FechaAccion from FSRAccion where idFSRAccion=" + query + ";", con);
                string fecha = (string)com1.ExecuteScalar();
                con.Close();
                con.Open();
                SqlCommand com2 = new SqlCommand("select HorasAccion from FSRAccion where idFSRAccion=" + query + ";", con);
                int hora = (int)com2.ExecuteScalar();
                con.Close();
                con.Open();
                SqlCommand com3 = new SqlCommand("select IDFSRAccion from FSRAccion where idFSRAccion=" + query + ";", con);
                int ida = (int)com3.ExecuteScalar();
                con.Close();

                con.Open();
                SqlCommand com4 = new SqlCommand("select idFolioFSR from FSRAccion where idFSRAccion=" + query + ";", con);
                int fol_io = (int)com4.ExecuteScalar();
                con.Close();
                con.Open();
                SqlCommand com5 = new SqlCommand("select TipoServicio from v_fsr where Folio=" + fol_io.ToString() + ";", con);
                string serv_io = (string)com5.ExecuteScalar();
                con.Close();

                fol.Text = fol_io.ToString();
                serv.Text =serv_io;
                descacci.Text = accion;
                fechacci.Text = fecha;
                horaacci.Text = hora.ToString();
                IDAccion.Text = ida.ToString();
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

    public void borrarsibtn_Click(object sender, EventArgs e)
    {
        //Codigo para borrar la accion realizada
        con.Open();
        SqlCommand del = new SqlCommand("delete from FSRAccion where idFSRAccion =" + IDAccion.Text + ";", con);
        del.ExecuteNonQuery();
        con.Close();
        cargardatos2();

        avisodel.Style.Add("display", "none");
        headerone.Style.Add("display", "block");
        footerid.Style.Add("display", "flex");
        contenone.Style.Add("display", "block");
    }
    protected void borrarnobtn_Click(object sender, EventArgs e)
    { 
        avisodel.Style.Add("display", "none");
        headerone.Style.Add("display", "block");
        footerid.Style.Add("display", "flex");
        contenone.Style.Add("display", "block");
    }


    protected void Chck_CheckedChanged(object sender, EventArgs e)
    {
        if (Chck.Checked == true)
        {
            //Codigo anterior para checar si es que se esta encontrando la notificacion del asesor para enviar el mail
            con.Open();
            SqlCommand com5 = new SqlCommand("update fsr set NotAsesor = 'Si' where Folio=" + Session["folio_p"].ToString() + ";", con);
            com5.ExecuteNonQuery();
            con.Close();

            Session["not_ase"] = "Si";
        }
        else if(Chck.Checked == false)
        {
            con.Open();
            SqlCommand com6 = new SqlCommand("update fsr set NotAsesor = 'No' where Folio=" + Session["folio_p"].ToString() + ";", con);
            com6.ExecuteNonQuery();
            con.Close();
            Session["not_ase"] = "No";
        }
    }

    protected void SA_Click(object sender, EventArgs e)
    {
        //Volver a servicios asignados
        Response.Redirect("ServiciosAsignados.aspx");
    }
}