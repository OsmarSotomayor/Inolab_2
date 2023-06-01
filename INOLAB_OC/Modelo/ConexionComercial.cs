using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Drawing;
using DocumentFormat.OpenXml.Office.Word;

namespace INOLAB_OC.Modelo
{
    public class ConexionComercial
    {
        private static string source;
        private static string catalog;
        private static string user;
        private static string password;
        private static SqlConnection conexion;
        private bool databaseProduction = false;
        private static bool databaseTest = true;

        private static void initDatabase()
        {
            string conexionString = @"Data Source=INOLABSERVER03;Initial Catalog=Comercial;Persist Security Info=True;User ID=ventas;Password=V3ntas_17";
            conexion = new SqlConnection(conexionString);
        }


        public static bool executeQuery(string query)
        {
            initDatabase();
            try
            {
                SqlCommand comand = new SqlCommand(query, conexion);
                conexion.Open();
                comand.ExecuteNonQuery();
                conexion.Close();
                Trace.WriteLine("PASS: SUCESS");
                return true;
            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("PASS: FAILED");
                return false;
            }

        }

        public static int getScalar(string query)
        {
            initDatabase();
            try
            {
                SqlCommand comand = new SqlCommand(query, conexion);
                conexion.Open();
                int escalar = Convert.ToInt32(comand.ExecuteNonQuery());
                conexion.Close();
                Trace.WriteLine("PASS: SUCESS");
                return escalar;

            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("PASS: FAILED");
                return -1;
            }

        }

        public static string getText(string query)
        {
            initDatabase();
            try
            {
                DataSet table = new DataSet();
                conexion.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                adapter.Fill(table);
                conexion.Close();
                Trace.WriteLine("PASS SUCES");
                return table.Tables[0].Rows[0][0].ToString();

            }
            catch (SqlException ex)
            {
                Trace.WriteLine("PASS FAILED");
                conexion.Close();
                return null;
            }
            catch (IndexOutOfRangeException ex)
            {
                conexion.Close();
                Trace.WriteLine("PASS: FAILED ( " + ex.Message + " )");
                return "FUERA_DE_RANGO";
            }

        }

        public static bool isThereSomeInformation(string query)
        {
            initDatabase();
            try
            {
                DataSet table = new DataSet();
                conexion.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                adapter.Fill(table);
                conexion.Close();
                Trace.WriteLine("PAAS SUCCES");
                return table.Tables[0].Rows.Count > 0 ? true : false;
            }
            catch (SqlException ex)
            {
                Trace.WriteLine("PAS FAILED", ex.Message);
                conexion.Close();
                return false;
            }
        }

        public static DataTable getDataTable(string query)
        {
            initDatabase();
            try
            {
                DataSet table = new DataSet();
                conexion.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                adapter.Fill(table);
                conexion.Close();
                Trace.WriteLine("PASS SUCCES");
                return table.Tables[0];

            }
            catch (SqlException ex)
            {
                Trace.WriteLine("PASS FAILED", ex.Message);
                conexion.Close();
                return null;
            }
        }

        public static DataRow getDataRow(string query)
        {
            initDatabase();
            try
            {

                DataSet tabla = new DataSet();
                conexion.Open();
                SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion); //SqlDataAdapter, actúa como un puente entre un DataSet y SQL Server para recuperar y guardar datos.
                adaptador.Fill(tabla);
                conexion.Close();
                Trace.WriteLine("PASS: SUCESS");
                return tabla.Tables[0].Rows[0];
            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("PASS: FAILED ( " + ex.Message + " )");
                return null;
            }
        }

        public static DataSet getDataSet(string query)
        {
            initDatabase();
            try
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand(query, conexion);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                conexion.Close();
                Trace.WriteLine("PASS SUCCES");
                return dataSet;

            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("CONEXION FAILED " + ex.Message);
                return null;
            }
        }

        public static void executeStoreProcedureStp_Update_Plan(int registro, DateTime datePicker, string textoCliente, string textoComentario, string ddlTipoRegistro, string textoObjetivo)
        {
            initDatabase();
            try
            {
                SqlCommand comando = new SqlCommand("stp_update_plan", conexion);
                comando.CommandType = CommandType.StoredProcedure;
                conexion.Open();

                comando.Parameters.Add("@registro", SqlDbType.Int);
                comando.Parameters.Add("@fecha", SqlDbType.Date);
                comando.Parameters.Add("@cliente", SqlDbType.VarChar);
                comando.Parameters.Add("@comen", SqlDbType.VarChar);
                comando.Parameters.Add("@tipo", SqlDbType.VarChar);
                comando.Parameters.Add("@objetivo", SqlDbType.VarChar);

                comando.Parameters["@registro"].Value = registro;
                comando.Parameters["@fecha"].Value = datePicker;
                comando.Parameters["@cliente"].Value = textoCliente;
                comando.Parameters["@comen"].Value = textoComentario;
                comando.Parameters["@tipo"].Value = ddlTipoRegistro;
                comando.Parameters["@objetivo"].Value = textoObjetivo;

                
                comando.ExecuteNonQuery();
                conexion.Close();
                Trace.WriteLine("SUCCES STORE PROCEDURE");
            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("STORE PROCEDURE FAILED " + ex.Message);
            }
        }

        public static void executeStoreProcedureStrp_Save_Plan(string ddlTipoRegistro, string textoCliente, DateTime datePicker, string comentario,
            string user,string textoObjetivo, string hora)
        {
            initDatabase();
            try
            {
                SqlCommand comando = new SqlCommand("strp_save_plan", conexion);
                comando.CommandType = CommandType.StoredProcedure;
                conexion.Open();

                comando.Parameters.Add("@tiporegistro", SqlDbType.VarChar);
                comando.Parameters.Add("@cliente", SqlDbType.VarChar);
                comando.Parameters.Add("@fllamada", SqlDbType.Date);
                comando.Parameters.Add("@comentario", SqlDbType.VarChar);
                comando.Parameters.Add("@asesor", SqlDbType.VarChar);
                comando.Parameters.Add("@objetivo", SqlDbType.VarChar);
                comando.Parameters.Add("@hora", SqlDbType.VarChar);

                comando.Parameters["@tiporegistro"].Value = ddlTipoRegistro;
                comando.Parameters["@cliente"].Value = textoCliente;
                comando.Parameters["@fllamada"].Value = datePicker;
                comando.Parameters["@comentario"].Value = comentario;
                comando.Parameters["@asesor"].Value = user;
                comando.Parameters["@objetivo"].Value = textoObjetivo;
                comando.Parameters["@hora"].Value = hora;

                comando.ExecuteNonQuery();
                conexion.Close();
                Trace.WriteLine("SUCCES STORE PROCEDURE");
            }
            catch (SqlException ex)
            {
                conexion.Close();
                Trace.WriteLine("STORE PROCEDURE FAILED " + ex.Message);
            }
        }

    }
}