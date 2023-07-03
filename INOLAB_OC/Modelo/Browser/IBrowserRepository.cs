using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using INOLAB_OC.Entidades;

namespace INOLAB_OC.Modelo.Browser
{
    public interface IBrowserRepository
    {
        DataRow OptenerDatosDeUsuario(E_Usuario usuario); 
    }
}
