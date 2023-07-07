using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INOLAB_OC.Entidades;
using INOLAB_OC.Modelo.Browser;
using INOLAB_OC.Controlador;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Controlador
{
    [TestClass]
    public class SesionTest
    {
        [TestMethod]
        public void OtenerDatosDeUsuario_ReturnDataRow()
        {
            //Arrange 
            TestBrowserRepository repositoryio = new TestBrowserRepository();
            E_Usuario usuario = new E_Usuario();
            C_Sesion controladorSesson = new C_Sesion(repositoryio);


            // Acction
            DataRow resultadoEsperado = null;
            var valorActual = controladorSesson.optenerDatosDeUsuario(usuario);

            //Assert
            Assert.AreEqual(resultadoEsperado, valorActual);
        }

    }

    public class TestBrowserRepository : IBrowserRepository
    {
        public void actualizarDatosDeServicio(E_Servicio folioServicioFSR, string idUsuario)
        {
            throw new NotImplementedException();
        }

        public void actualizarFechayHoraFinDeServicio(E_Servicio servicio, string idIngeniero)
        {
            throw new NotImplementedException();
        }

        public void actualizarValorDeCampo(string folio, string campo, string valorDelCampo)
        {
            throw new NotImplementedException();
        }

        public void actualizarValorDeCampo(string folio, string campo, string valorDelCampo, string idUsuario)
        {
            throw new NotImplementedException();
        }

        public int consultarEstatusDeFolioServicio(string folio, string idUsuario)
        {
            throw new NotImplementedException();
        }

        public DateTime consultarFechaFinDeFolio(string folio, string idIngeniero, string campoDondeSeConsulta)
        {
            throw new NotImplementedException();
        }

        public DateTime consultarFechaInicioDeFolio(string folio, string idIngeniero, string campoDondeSeConsulta)
        {
            throw new NotImplementedException();
        }

        public DataSet consultarFolioServicioPorEstatus(E_Servicio servicio, string idUsuario)
        {
            throw new NotImplementedException();
        }

        public DataRow consultarInformacionDeFolioPorFolio(string folio)
        {
            throw new NotImplementedException();
        }

        public DataRow consultarInformacionDeFolioPorFolioYUsuario(string usuario, string folio)
        {
            throw new NotImplementedException();
        }

        public string consultarInicioDeServicio(string folio)
        {
            throw new NotImplementedException();
        }

        public DataSet consultarTodosLosFoliosDeServicio(string idUsuario)
        {
            throw new NotImplementedException();
        }

        public string consultarValorDeCampo(string folio, string idUsuario, string campo)
        {
            throw new NotImplementedException();
        }

        public string consultarValorDeCampo(string folio, string campo)
        {
            throw new NotImplementedException();
        }

        public void iniciarFolioServicio(DateTime fechaYHoraDeInicioDeServicio, string folio, string idIngeniero)
        {
            throw new NotImplementedException();
        }

        public DataRow OptenerDatosDeUsuario(E_Usuario usuario)
        {
            return null;
        }
    }
}
