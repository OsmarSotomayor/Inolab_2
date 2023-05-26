using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Diagnostics;
using System.IO.Packaging;
using System.Data;
using System.Drawing;
using System.Web.Services.Description;
using DocumentFormat.OpenXml.Drawing;

namespace INOLAB_OC.Modelo
{
    public class Conexion
    {
        
        private static string source;
        private static string catalog;
        private static string user;
        private static string password;
        private static SqlConnection conexion;
        private bool databaseProduction = false;
        private static bool databaseTest = true;

       
        public static void initDatabase()
        {
            if (databaseTest)
            {
                source = "INOLABSERVER03";
                catalog = "BrowserPruebas";
                user = "ventas";
                password = "V3ntas_17";
            }
            else {
                source = "INOLABSERVER03";
                catalog = "Browser";
                user = "ventas";
                password = "V3ntas_17";
            }
            string conexionString = @"Data Source=" + source + ";Initial Catalog=" + catalog + ";Persist Security Info=True;User ID=" + user + ";Password= " + password + "";
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
            }catch (SqlException ex)
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
                SqlCommand comand = new SqlCommand(query,conexion);
                conexion.Open();
                int escalar = Convert.ToInt32(comand.ExecuteNonQuery());
                conexion.Close();
                Trace.WriteLine("PASS: SUCESS");
                return escalar;

            }catch (SqlException ex)
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

            }catch (SqlException ex)
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
            }catch (SqlException ex)
            {
                Trace.WriteLine("PAS FAILED",ex.Message);
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
                SqlDataAdapter adapter = new SqlDataAdapter(query,conexion);
                adapter.Fill(table);
                conexion.Close();
                Trace.WriteLine("PASS SUCCES");
                return table.Tables[0];

            }catch (SqlException ex)
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

        public static void executeStoreProcedureLogWeb(string usuario, string ip)
        {
            initDatabase();
            SqlCommand comando = new SqlCommand("LogWeb", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@usuario", SqlDbType.VarChar);
            comando.Parameters.Add("@ip", SqlDbType.VarChar);

            comando.Parameters["@usuario"].Value = usuario;
            comando.Parameters["@ip"].Value = ip;

            conexion.Open();
            comando.ExecuteNonQuery();
            conexion.Close();
        }


    }
}