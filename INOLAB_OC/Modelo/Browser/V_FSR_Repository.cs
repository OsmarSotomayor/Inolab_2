using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo.Browser.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Modelo.Browser
{
    public class V_FSR_Repository : IV_FSR_Repository
    {
        public DataSet consultarFolioServicioPorEstatus(E_V_FSR servicio, string idUsuario)
        {
            string query = "select * from V_FSR where Estatus = '" + servicio.Estatus + "' and " +
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