﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using INOLAB_OC.Modelo;

namespace INOLAB_OC.Controlador
{
    public class Controlador_Sesion
    {
        public static DataRow optenerDatosDeUsuario(string usuario, string contraseña)
        {
            DataRow datosDelUsuario = Conexion.getDataRow("select  Usuario, Password_,Nombre, Apellidos,idarea,idrol,IdUsuario from Usuarios where usuario='" + usuario + "' and password_='" + contraseña + "'");
            return datosDelUsuario;
        }

        public static void loggearUsuario(string usuario, string ip)
        {
            LogicaConexion.executeStoreProcedureLogWeb(usuario,ip);
        }
    }
}