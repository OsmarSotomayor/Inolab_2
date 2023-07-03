using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Diagnostics;
using INOLAB_OC.Entidades;

namespace INOLAB_OC.Modelo
{
    public class LogicaConexion
    {
        public static void executeStoreProcedureLogWeb(E_Usuario usuario)
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
            }catch (Exception ex)
            {
                Trace.WriteLine("PASS: FAILED "+ ex.Message);
            }
            
        }

     
    }
}