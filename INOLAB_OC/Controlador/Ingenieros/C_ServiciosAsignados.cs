using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo;
using INOLAB_OC.Modelo.Browser;

namespace INOLAB_OC.Controlador
{
    public class C_ServiciosAsignados
    {
        private static  IBrowserRepository _browserRepository;

        public C_ServiciosAsignados(IBrowserRepository browserRepository) {
            _browserRepository = browserRepository;
        }
        public  DataRow consultarInformacionDeFolioParaReporteServicios(string folio)
        {
            DataRow informacionFolio = _browserRepository.consultarInformacionDeFolioPorFolio(folio);
            return informacionFolio;
        }

        public DataSet consultarFolioServicioPorEstatus(E_Servicio servicio, string idUsuario)
        {
            DataSet foliosDeServicio = _browserRepository.consultarFolioServicioPorEstatus(servicio, idUsuario);
            return foliosDeServicio;
        }

        public DataSet consultarTodosLosFoliosDeIngeniero(string idUsuario)
        {
            DataSet foliosDeServicio = _browserRepository.consultarTodosLosFoliosDeServicio(idUsuario);
            return foliosDeServicio;
        }
    }
}