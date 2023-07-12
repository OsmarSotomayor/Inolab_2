using INOLAB_OC.Modelo.Inolab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Controlador.Ingenieros
{
   
    public class C_SCL5
    {
        private  ISCL5Repository repositorio;

        public C_SCL5(ISCL5Repository repositorio ) {
            this.repositorio = repositorio;
        }

        public C_SCL5(ISCL5Repository repositorio, string folio)
        {
            this.repositorio = repositorio;
        }
        public void actualizarValorDeCampo(string campo, string valorDeCampo, string folio)
        {
            repositorio.actualizarValorDeCampo(campo,valorDeCampo,folio);
        }

        public string seleccionarValorDeCampo(string campo, string folio)
        {
           return repositorio.seleccionarValorDeCampo(campo , folio);
        }

        public string seleccionarValorDeCampoTop(string campo, string folio)
        {
            return repositorio.seleccionarValorDeCampoTop(campo , folio);
        }
    }
}