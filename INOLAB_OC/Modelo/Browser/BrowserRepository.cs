using INOLAB_OC.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace INOLAB_OC.Modelo.Browser
{
    public class BrowserRepository : IBrowserRepository
    {
        
        public  DataRow OptenerDatosDeUsuario(E_Usuario objetoUsuario)
        {
            DataRow datosDelUsuario = Conexion.getDataRow("select  Usuario, Password_,Nombre, Apellidos,idarea,idrol,IdUsuario from " +
                "Usuarios where usuario='" + objetoUsuario.Nombre + "' and password_='" + objetoUsuario.Contraseña + "'");
            
            return datosDelUsuario;
        }

        public  void executeStoreProcedureLogWeb(E_Usuario usuario)
        {
            SqlConnection conexion = Conexion.crearConexionABrowser();
            try
            {
                SqlCommand comando = new SqlCommand("LogWeb", conexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add("@usuario", SqlDbType.VarChar);
                comando.Parameters.Add("@ip", SqlDbType.VarChar);

                comando.Parameters["@usuario"].Value = usuario.NombreDeUsuario;
                comando.Parameters["@ip"].Value = usuario.IP;

                conexion.Open();
                comando.ExecuteNonQuery();
                Trace.WriteLine("Conexion Correcta executeStoreProcedureLogWeb");
                conexion.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("PASS: FAILED " + ex.Message);
            }

        }

        public DataSet consultarFolioServicioPorEstatus(E_Servicio servicio, string idUsuario)
        {
            string query = "select * from V_FSR where Estatus = '"+servicio.Estatus+"' and " +
                " IdIngeniero= '" + idUsuario + "' order by folio desc ";

            DataSet foliosDeServicio = Conexion.getDataSet(query);
            return foliosDeServicio;
        }

        public DataRow consultarInformacionDeFolioPorFolio(string folio)
        {
            string query = " select * from  v_fsr where Folio = " + folio;
            DataRow informacionDeFolio = Conexion.getDataRow(query);
            return informacionDeFolio;
        }

        public DataSet consultarTodosLosFoliosDeServicio(string idUsuario)
        {
           string query = "Select DISTINCT * from  v_fsr where " +
                "idingeniero = " + idUsuario + " order by folio desc";

            DataSet todosLosFoliosDelIngeniero = Conexion.getDataSet(query);
            return todosLosFoliosDelIngeniero;
        }
    }
}