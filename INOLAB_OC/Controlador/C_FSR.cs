using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo;
using INOLAB_OC.Modelo.Browser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Controlador
{
    public class C_FSR
    {
        private  IBrowserRepository _browserRepository;
        private string _idUsuario;

        public C_FSR(IBrowserRepository browserRepository, string idUsuario) {
            _browserRepository = browserRepository;
            _idUsuario = idUsuario;
        }
        public  string seleccionarInicioServicio(string folio)
        {
            string inicioDeServicio = _browserRepository.consultarInicioDeServicio(folio);
            return inicioDeServicio;
        }

        public  void actualizarDatosDeServicio(E_Servicio folioServicioFSR)
        {
            _browserRepository.actualizarDatosDeServicio(folioServicioFSR, _idUsuario);
        }

        public void iniciarFolioServicio(DateTime fechaYHoraDeInicioDeServicio, string folio)
        {
            _browserRepository.iniciarFolioServicio(fechaYHoraDeInicioDeServicio, folio, _idUsuario);
        }

        public static void actualizarFolioSap(string folio)
        {
            string consulta = "Select ClgID FROM SCL5 where U_FSR = " + folio;
            int idFolioActividadSap = ConexionInolab.getScalar(consulta);
            ConexionInolab.executeQuery(" UPDATE OCLG SET tel = '" + folio + " En Proceso' where ClgCode=" + idFolioActividadSap.ToString() + ";");

        }

        public  void actualizarFechayHoraFinDeServicio(E_Servicio servicio)
        {
            _browserRepository.actualizarFechayHoraFinDeServicio(servicio, _idUsuario);
        }

        public  DateTime traerFechaYhoraDeInicioDeFolio(string folio)
        {
            object fecha;
            DateTime fechaYHoraInicioServicio;
            string campoDondeSeConsulta;
            try
            {
                campoDondeSeConsulta = "WebFechaIni";
                fechaYHoraInicioServicio = _browserRepository.consultarFechaInicioDeFolio(folio, _idUsuario, campoDondeSeConsulta);
 
                return fechaYHoraInicioServicio;

            }
            catch (Exception ex)
            {
                campoDondeSeConsulta = "Inicio_Servicio";
                fechaYHoraInicioServicio = _browserRepository.consultarFechaInicioDeFolio(folio, _idUsuario, campoDondeSeConsulta);
                
                return fechaYHoraInicioServicio;
            }

        }


        public  DateTime traerFechaYhoraDeFinDeFolio(string folio)
        {
            DateTime fechaYHoraFinServicio;
            string campoDondeSeConsulta;
            try
            {
                campoDondeSeConsulta = "WebFechaFin";
                fechaYHoraFinServicio = _browserRepository.consultarFechaFinDeFolio(folio, _idUsuario, campoDondeSeConsulta);
                return fechaYHoraFinServicio;

            }
            catch (Exception ex)
            {
                campoDondeSeConsulta = "Fin_Servicio";
                fechaYHoraFinServicio = _browserRepository.consultarFechaFinDeFolio(folio, _idUsuario, campoDondeSeConsulta);
                return fechaYHoraFinServicio;
            }

        } 

        public  DataRow consultarInformacionFolioServicioPorFolioYUsuario( string usuario, string folio)
        {
            DataRow informacionServicio = _browserRepository.consultarInformacionDeFolioPorFolioYUsuario(usuario, folio);
            return informacionServicio;
        }

        public string consultalValorDeCampo(string folio, string idUsuario, string campo)
        {
           return _browserRepository.consultarValorDeCampo(folio, idUsuario, campo);
        }
    }
}