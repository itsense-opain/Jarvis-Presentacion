using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Controllers
{
    public class ConsultasController : Controller
    {
        public IActionResult Consultas()
        {
            return View(); 
        }

        public IActionResult ConsultasAdmin()
        {
            return View();
        }
        public IActionResult Index(string pdfyexcel)
        {
            string filename = string.Empty;
            string filepath = filepath = "D:\\ARCHIVOS\\ArchivosTransitoConexion";
            string fullName;
            switch (pdfyexcel)
            {
                case "pdf":

                    filename = "TransitoConexionPDF.pdf";
                    fullName = Path.Combine(filepath, filename);
                    return File(new FileStream(fullName, FileMode.Open), "application/pdf", filename);
                    break;

                default:

                    filename = "TransitoConexionExcel.xlsx";
                    fullName = Path.Combine(filepath, filename);
                    return File(new FileStream(fullName, FileMode.Open), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    break;
            }            
        }
    }
}