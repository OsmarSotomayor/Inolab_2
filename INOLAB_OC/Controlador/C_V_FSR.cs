using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo.Browser;
using INOLAB_OC.Modelo.Browser.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Controlador
{
    public class C_V_FSR
    {
        private  IV_FSR_Repository v_FsrRepository;

        public C_V_FSR(IV_FSR_Repository v_FsrRepository)
        {
            this.v_FsrRepository = v_FsrRepository;
        }
        public DataRow consultarInformacionDeFolioParaReporteServicios(string folio)
        {
            DataRow informacionFolio = v_FsrRepository.consultarInformacionDeFolioPorFolio(folio);
            return informacionFolio;
        }

        public DataSet consultarFolioServicioPorEstatus(E_V_FSR servicio, string idUsuario)
        {
            DataSet foliosDeServicio = v_FsrRepository.consultarFolioServicioPorEstatus(servicio, idUsuario);
            return foliosDeServicio;
        }

        public DataSet consultarTodosLosFoliosDeIngeniero(string idUsuario)
        {
            DataSet foliosDeServicio = v_FsrRepository.consultarTodosLosFoliosDeServicio(idUsuario);
            return foliosDeServicio;
        }

    }
}