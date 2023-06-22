using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Entidades
{
    public class E_Usuario
    {
        public string IP { get; set; }
        public string Nombre { get; set; }

        public string NombreDeUsuario { get; set; }
        public string Apellidos { get; set; }

        public string Contraseña { get; set; }

        public int IdArea { get; set; }

        public int Activo { get; set; }

        public int IdRol { get; set; }

        public string Firma { get; set; }

        public string Mail { get; set; }

        public int IngArea { get; set; }

    }
}