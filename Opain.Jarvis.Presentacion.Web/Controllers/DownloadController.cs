using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Opain.Jarvis.Presentacion.Web.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IConfiguration configuration;
        // GET: DownloadController
       
        public DownloadController(IConfiguration cfg)
        {
            configuration = cfg;
        }
        public FileResult Index(string pdfyexcel,string filename)
        {
            //"D:\\ARCHIVOS\\ArchivosTransitoConexion";
            string filepath = configuration.GetSection("Config:RutaArchivosTransitoConexion").Value; 
            string fullName = string.Empty;

            switch (pdfyexcel)
            {
                case ".pdf":

                    //"TransitoConexionPDF.pdf";
                    fullName = Path.Combine(filepath, filename);
                    return File(new FileStream(fullName, FileMode.Open), "application/pdf", filename);
                    break;

                default:

                    //"TransitoConexionExcel.xlsx";
                    fullName = Path.Combine(filepath, filename);
                    FileContentResult resultado= new FileContentResult(System.IO.File.ReadAllBytes(fullName), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    resultado.FileDownloadName = filename;
                    return resultado;
                    break;
            }
        }
        
        public FileResult CierreManual(string filepath, string filename)
        {     
            string fullName = filepath.Replace("xx","\\");
            string Extencion = System.IO.Path.GetExtension(fullName);
            string[] pathsplit = fullName.Split("\\");
            int items = pathsplit.Count();
            filename = pathsplit[items-1];

            if (Extencion == ".pdf")
            {                
                return File(new FileStream(fullName, FileMode.Open), "application/pdf", filename);
            }
            else
            {
                return File(new FileStream(fullName, FileMode.Open), "application/txt", filename);
            }
        }

        public FileResult TransitoManual(string filepath)
        {
            string fullName = filepath.Replace("xx", "\\");
            string Extencion = System.IO.Path.GetExtension(fullName);
            string[] pathsplit = fullName.Split("\\");
            int items = pathsplit.Count();
            string filename = string.Empty;
            filename = pathsplit[items - 1];

            if (Extencion == ".pdf")
            {
                return File(new FileStream(fullName, FileMode.Open), "application/pdf", filename);
            }
            else
            {
                return File(new FileStream(fullName, FileMode.Open), "application/txt", filename);
            }
        }

    }
}
