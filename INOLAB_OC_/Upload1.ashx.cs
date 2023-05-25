using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INOLAB_OC
{
    /// <summary>
    /// Descripción breve de Upload1
    /// </summary>
    public class Upload1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpPostedFile file = context.Request.Files["filedata"];
                string nome = Guid.NewGuid().ToString();

                file.SaveAs(context.Server.MapPath("~/") + "Folios" + nome);
            }
            catch(Exception)
            {
               
            }

            
          
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}