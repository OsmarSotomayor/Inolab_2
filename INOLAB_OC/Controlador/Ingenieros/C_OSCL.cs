using INOLAB_OC.Modelo;
using INOLAB_OC.Modelo.Inolab;
using INOLAB_OC.Modelo.Inolab.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INOLAB_OC.Controlador.Ingenieros
{
    public class C_OSCL
    {
        private readonly IOSCL_Repository repositorio;

        public C_OSCL(IOSCL_Repository repositorio) {
            this.repositorio = repositorio;
        }

        public void verificarSiSeCierraLaLLamada(string numeroDeValoresEnEstatus, bool nulo, int callId)
        {
            if (numeroDeValoresEnEstatus == "1" && nulo == false)
            {
                repositorio.cambiarValorDeEstatusFolioAFinalizado(callId);

            }
        }

    }
}