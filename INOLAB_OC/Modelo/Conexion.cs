using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Diagnostics;

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
            string conexionString = @"Data Source=" + source + ";Initial Catalog=" + catalog + ";Persist Security Info=True;User ID=" + user + ";Password=" + password;
            conexion = new SqlConnection(conexionString);
        }

        public static bool executeQuery(string query)
        {
            try{
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
        

    }
}