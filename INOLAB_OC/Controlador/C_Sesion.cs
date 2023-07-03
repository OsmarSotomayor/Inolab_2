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
    public class C_Sesion
    {
        private static  IBrowserRepository _browserRepository;
        public C_Sesion(IBrowserRepository browserRepository)
        {
            _browserRepository = browserRepository;
        }

        public DataRow optenerDatosDeUsuario(E_Usuario usuario)
        { 
            DataRow datosDelUsuario  = _browserRepository.OptenerDatosDeUsuario(usuario);
            return datosDelUsuario;
        }

        public void loggearUsuario(E_Usuario usuario)
        {
            _browserRepository.executeStoreProcedureLogWeb(usuario);
        }
    }
}