using INOLAB_OC.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Modelo.Browser
{
    public class BrowserRepository : IBrowserRepository
    {
        
        public  DataRow OptenerDatosDeUsuario(E_Usuario objetoUsuario)
        {
            DataRow datosDelUsuario = Conexion.getDataRow("select  Usuario, Password_,Nombre, Apellidos,idarea,idrol,IdUsuario from " +
                "Usuarios where usuario='" + objetoUsuario.Nombre + "' and password_='" + objetoUsuario.Contraseña + "'");
            
            return datosDelUsuario;
        }
    }
}