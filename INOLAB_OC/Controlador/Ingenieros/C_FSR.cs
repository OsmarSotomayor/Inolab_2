using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Controlador
{
    public class C_FSR
    {
        public static string seleccionarInicioServicio(string folio)
        {
            string inicioDeServicio = Conexion.getText("select top 1 Inicio_Servicio from FSR where Folio = " + folio + " and Inicio_Servicio is not null;");
            return inicioDeServicio;
        }

        public static void actualizarDatosDeServicio(E_Servicio folioServicioFSR, string idUsuario)
        {
            Conexion.executeQuery(" UPDATE FSR SET Marca='" + folioServicioFSR.Marca + "', Modelo='" + folioServicioFSR.Modelo + "', NoSerie='" + folioServicioFSR.NoSerie +
                    "', Equipo='" + folioServicioFSR.DescripcionEquipo + "', IdEquipo_C='" + folioServicioFSR.IdEquipo + "',Cliente='" + folioServicioFSR.Cliente + "',Telefono='" + folioServicioFSR.Telefono +
                    "',N_Responsable='" + folioServicioFSR.N_Responsable + "',N_Reportado='" + folioServicioFSR.N_Reportado + "',Mail='" + folioServicioFSR.Email + "',Direccion='" + folioServicioFSR.Direccion +
                    "',Localidad='" + folioServicioFSR.Localidad + "',Depto='" + folioServicioFSR.Departamento + "',IdT_Problema='" + folioServicioFSR.IdProblema + "',IdT_Contrato='" + folioServicioFSR.IdContrato +
                    "', IdT_Servicio='" + folioServicioFSR.idServicio + "'" + " where Folio=" + folioServicioFSR.Folio + " and Id_Ingeniero ='" +
                    idUsuario + "';");
        }

        public static void iniciarFolioServicio(DateTime fechaYHoraDeInicioDeServicio, string folio, string idIngeniero)
        {
            Conexion.executeQuery(" UPDATE FSR SET idStatus = '2' ,Inicio_Servicio='" +
             DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', WebFechaIni='" + fechaYHoraDeInicioDeServicio.ToString("yyyy - MM - dd HH: mm:ss.fff")
             + "' where Folio=" + folio + " and Id_Ingeniero =" + idIngeniero + ";");
        }

        public static void actualizarFolioSap(string folio)
        {
            string consulta = "Select ClgID FROM SCL5 where U_FSR = " + folio;
            int idFolioActividadSap = ConexionInolab.getScalar(consulta);
            ConexionInolab.executeQuery(" UPDATE OCLG SET tel = '" + folio + " En Proceso' where ClgCode=" + idFolioActividadSap.ToString() + ";");

        }

        public static void actualizarFechayHoraFinDeServicio(E_Servicio servicio, string idIngeniero)
        {
            Conexion.executeQuery(" UPDATE FSR SET WebFechaIni ='" + servicio.FechaInicio +
           "' ,WebFechaFin='" + servicio.FechaFin + "' where Folio= " + servicio.Folio +
           " and Id_Ingeniero ='" + idIngeniero + "';");
        }

        public static DateTime traerFechaYhoraDeInicioDeFolio(string folio, string idIngeniero)
        {
            object fecha;
            DateTime fechaYHoraInicioServicio;
            try
            {
                fecha = Conexion.getObject("select WebFechaIni from FSR where Folio=" + folio + " and Id_Ingeniero =" + idIngeniero + ";");
                fechaYHoraInicioServicio = (DateTime)fecha;
                return fechaYHoraInicioServicio;

            }
            catch (Exception ex)
            {
                fecha = Conexion.getObject("select Inicio_Servicio from FSR where Folio=" + folio + " and Id_Ingeniero =" + idIngeniero + ";");
                fechaYHoraInicioServicio = (DateTime)fecha;
                return fechaYHoraInicioServicio;
            }

        }


        public static DateTime traerFechaYhoraDeFinDeFolio(string folio, string idIngeniero)
        {
            object fecha;
            DateTime fechaYHoraFinServicio;
            try
            {
                fecha = Conexion.getObject("select WebFechaFin from FSR where Folio= '" + folio + "' and Id_Ingeniero ='" + idIngeniero + "';");
                fechaYHoraFinServicio = (DateTime)fecha;
                return fechaYHoraFinServicio;

            }
            catch (Exception ex)
            {
                fecha = Conexion.getObject("select Fin_Servicio from FSR where Folio='" + folio + "' and Id_Ingeniero ='" +idIngeniero+ "';");
                fechaYHoraFinServicio = (DateTime)fecha;
                return fechaYHoraFinServicio;
            }

        } 

        public static DataRow consultarInformacionFolioServicio(string idUsuario, string folio)
        {
            string query = "select * from v_fsr where idingeniero = '" + idUsuario + "' and Folio = '" + folio + "'; ";
            DataRow informacionServicio = Conexion.getDataRow(query);
            return informacionServicio;
        }
    }
}