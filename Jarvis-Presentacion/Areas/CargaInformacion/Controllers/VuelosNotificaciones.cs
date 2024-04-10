using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers
{
    public partial class VuelosController : Controller
    {
        public async Task EnviarCorreos(int idAreolinea, DateTime Fecha)
        {
            _logger.LogInformation($"Inicio notificación 6");
            _logger.LogInformation($"Notificación 6, Id Aeroliniea = " + idAreolinea);
            var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
            try
            {

                string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientesProcesados").Value, idAreolinea, Fecha.ToShortDateString());
                int respuesta = await servicioApi.GetAsync<int>(rutaRelativa);
                _logger.LogInformation($"Cantidad vuelos pendientes procesados =" + respuesta);


                if (respuesta == 0)
                {
                    _logger.LogInformation($"Ingresa notificación 6, Vuelos pendientes Procesados = " + respuesta);
                    //esto es lo que llega...
                    IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
                    string file = "https://" + Request.Host + "/images/firma.png";
                    //List<UsuarioOtd> usuariosFiltrados = new List<UsuarioOtd>();

                    foreach (var datosUsuario in usuarios.Where(x => x.UsuarioAerolinea.Any()))
                    {
                        foreach (var aerolinea in datosUsuario.UsuarioAerolinea)
                        {
                            if (aerolinea.IdAerolinea == idAreolinea)
                            {
                                _logger.LogInformation($"Notificación 6 Vuelos Procesados Usuario Aerolinea" + datosUsuario.NumeroDocumento + "  " + datosUsuario.NormalizedUserName);
                                _logger.LogInformation($"Notificación 6 Vuelos Procesados Id Aerolinea" + idAreolinea);
                                await emailSender.SendEmailAsync(
                                 datosUsuario.Email.Trim(),
                                 "Vuelos Procesados Notificacion 6",
                                 "<p>Buen día estimada aerolínea,</p></br></br></br><p>Nos permitimos informarles que sus vuelos con fecha " + Fecha.ToString("dd/MM/yyyy") + " han sido revisados, ya puede proceder a generar la respectiva infrasa, en el caso de detectar novedades o cobros, realizar la solicitud en el módulo contáctenos</p></br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='120'/><p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                                _logger.LogInformation($"Notificación 6 Vuelos Procesados, Email Usuario: {datosUsuario.Email.Trim()} ");
                            }
                        }
                    }
                }
                _logger.LogInformation($"Fin notificación 6 Vuelos Procesados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
            }
        }

        public async Task CargueExitosoNotificacionHallazgosUnoYDos(CargueExitosoNotificacionHallazgosOtd
            _CargueExitosoNotificacionHallazgos)
        {
            #region CodigoMejorado
            /*try
            {
                //Enviamos correo Pendiente
                string file = "https://" + Request.Host + "/images/firma.png";

                #region Cargue de vuelos exitoso  1
                //Notificacion para todo
                _logger.LogInformation($"Inicio cargue de vuelos exitoso notificacion 1, Email Usuario: {_CargueExitosoNotificacionHallazgos.EmailUser.Trim()} ");

                await emailSender.SendEmailAsync(
                   _CargueExitosoNotificacionHallazgos.EmailUser.Trim(),
                   "Cargue de vuelos exitoso 1",
                   "<p>Buen día estimada aerolínea,</p></br></br>Nos permitimos informales que el cargue de la información reportada por parte de ustedes se realizó exitosamente, la cuál será revisada previamente por el equipo de facturación</br></br> Por favor estar muy atentos a los cambios que se puedan presentar. </br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='120'/></p><p></p> </br><P> </br>Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");

                await emailSender.SendEmailAsync("luis.cantin@componenteserviex.com", "Notificación 1 copia"," Usuario " + _CargueExitosoNotificacionHallazgos.EmailUser.Trim());
                _logger.LogInformation($"Notificacion1: Usuario " + _CargueExitosoNotificacionHallazgos.EmailUser.Trim() + " Cantida Sin Cargar " + _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " Numero " + _CargueExitosoNotificacionHallazgos.Contador);

                #endregion

                #region Notificación de hallazgos 2
                if (_CargueExitosoNotificacionHallazgos.CantidadSinCargar > 0)
                {
                    _logger.LogInformation($"Notificación de hallazgos 2, Email Usuario: {_CargueExitosoNotificacionHallazgos.EmailUser.Trim()}");
                    await emailSender.SendEmailAsync(
                       _CargueExitosoNotificacionHallazgos.EmailUser.Trim(),
                       "Notificación de hallazgos 2",
                       "<p>Buen día estimada aerolínea,</p></br></br></br>Se informa que de los " + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, " + _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " presentan novedades.</br> Por favor estar muy atentos con los hallazgos reportados.</br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br><p> <img src='" + file + "' width='250' height='120'/></p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");

                    await emailSender.SendEmailAsync("luis.cantin@componenteserviex.com", "Notificación 2 copia", 
                        " Usuario " + _CargueExitosoNotificacionHallazgos.EmailUser.Trim()
                        + " Se informa que de  los " + _CargueExitosoNotificacionHallazgos.Contador 
                        + "  vuelos cargados, " + _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " presentan novedades.");

                    _logger.LogInformation($"Notificación de hallazgos 2 Usuario "
                        + _CargueExitosoNotificacionHallazgos.EmailUser + " Se informa que de  los "
                        + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, " 
                        + _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " presentan novedades.");
                }
                #endregion
                #region Notificación de hallazgos 3
                else
                {
                    _logger.LogInformation($"Notificación de hallazgos 3, Email Usuario: {_CargueExitosoNotificacionHallazgos.EmailUser}");
                    await emailSender.SendEmailAsync(
                       _CargueExitosoNotificacionHallazgos.EmailUser,
                       "Notificación de hallazgos 3",
                       "<p>Buen día estimada aerolínea,</p></br></br></br>Se informa que de los " + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados,  ninguno presenta novedades.</br> Por favor estar muy atentos con los hallazgos reportados.</br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br><p> <img src='" + file + "' width='250' height='120'/></p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");

                    await emailSender.SendEmailAsync("luis.cantin@componenteserviex.com", "Notificación 3 copia",
                    " Usuario " + _CargueExitosoNotificacionHallazgos.EmailUser.Trim()
                    + " Se informa que de  los " + _CargueExitosoNotificacionHallazgos.Contador
                    + "  vuelos cargados, ninguno presenta novedades.");


                    _logger.LogInformation($"Notificación de hallazgos 3 Usuario"
                        +_CargueExitosoNotificacionHallazgos.EmailUser +"  Se informa que de los " 
                        + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, ninguno presenta novedades.");
                }
                #endregion

                _logger.LogInformation($"Se envia Cargue de vuelos exitoso 1 y Notificación de hallazgos 2 o Notificación de hallazgos 3");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
            }
            */
            #endregion

            string file = "https://" + Request.Host + "/images/firma.png";
            string ConCopia = "";
            if (configuration.GetSection("SendGrid:cc").Value != null)
            {
                ConCopia = configuration.GetSection("SendGrid:cc").Value;
            }

            if (_CargueExitosoNotificacionHallazgos.EmailUser.IndexOf(";") > 0)
            {
                _CargueExitosoNotificacionHallazgos.EmailUser = 
                    _CargueExitosoNotificacionHallazgos.EmailUser.Substring(0, _CargueExitosoNotificacionHallazgos.EmailUser.Length - 1);
            }

            string MensajeNotificacion1 = "<p>Buen día estimada aerolínea,</p>" +
                "<p>Nos permitimos informales que el cargue de la información reportada por parte de ustedes " +
                "se realizó exitosamente, la cuál será revisada previamente por el equipo de facturación</p><br><br>" +
                "<p>Por favor estar muy atentos a los cambios que se puedan presentar.</p></br></br>" +
                "<p>Cordialmente,</p></br><b>Equipo de facturación</b></br><b>OPAIN S.A.</b></br>" +
                "<p><img src='" + file + "' width='250' height='120'/></p>" +
                "<P> </br>Bogotá D.C. - Colombia </br>www.eldorado.aero</br>" +
                "Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

            try
            {
                await SendMail("Cargue de vuelos exitoso 1",
                    _CargueExitosoNotificacionHallazgos.EmailUser.Trim(),
                    ConCopia, MensajeNotificacion1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
            }

            if (_CargueExitosoNotificacionHallazgos.CantidadSinCargar > 0)
            {
                string MensajeNotificacion2 = "<p>Buen día estimada aerolínea,</p></br></br></br>" +
                    "Se informa que de los " + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, " + 
                    _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " presentan novedades.</br> " +
                    "Por favor estar muy atentos con los hallazgos reportados.</br></br><p>Cordialmente,</p></br><b> " +
                    "Equipo de facturación </b></br><b> OPAIN S.A.</b></br><p> " +
                    "<img src='" + file + "' width='250' height='120'/></p><p></p> </br> " +
                    "Bogotá D.C. - Colombia </br>www.eldorado.aero " +
                    "</br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

                _logger.LogInformation($"Notificación de hallazgos 2 Usuario "
                       + _CargueExitosoNotificacionHallazgos.EmailUser + " Se informa que de  los "
                       + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, "
                       + _CargueExitosoNotificacionHallazgos.CantidadSinCargar + " presentan novedades.");

                try
                { 
                    await SendMail("Notificación de hallazgos 2",
                    _CargueExitosoNotificacionHallazgos.EmailUser.Trim(),
                    ConCopia, MensajeNotificacion2);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ResourceMessage.ErrorSystem);
                }
            }
            else
            {
                string MensajeNotificacion3 = "<p>Buen día estimada aerolínea,</p></br></br></br>" +
                    "Se informa que de los " + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados,  " +
                    "ninguno presenta novedades.</br> Por favor estar muy atentos con los hallazgos reportados.</br></br>" +
                    "<p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br><p>" +
                    "<img src='" + file + "' width='250' height='120'/></p><p></p> </br> " +
                    "Bogotá D.C. - Colombia </br>www.eldorado.aero " +
                    "</br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

                _logger.LogInformation($"Notificación de hallazgos 3 Usuario"
                        + _CargueExitosoNotificacionHallazgos.EmailUser + "  Se informa que de los "
                        + _CargueExitosoNotificacionHallazgos.Contador + "  vuelos cargados, ninguno presenta novedades.");

                try
                {
                    await SendMail("Notificación de hallazgos 3",
                    _CargueExitosoNotificacionHallazgos.EmailUser.Trim(),
                    ConCopia, MensajeNotificacion3);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ResourceMessage.ErrorSystem);
                }
            }

            _logger.LogInformation($"Se envia Cargue de vuelos exitoso 1 y Notificación de hallazgos 2 o Notificación de hallazgos 3");
        }

        public void enviarCorreo(string destino, string subject, string body)
        {
            //prueba Envio correo smtp

            String FROM = "jarvis@opain.com.co";
            String FROMNAME = "Sender Name";
            String TO = destino;
            String SMTP_USERNAME = "jarvis@opain.com.co";
            String SMTP_PASSWORD = "=hioE_b8ke2Y";
            String HOST = "slmp-550-66.slc.westdc.net";
            int PORT = 465;
            String SUBJECT = subject;
            String BODY = body;
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            message.To.Add(new MailAddress(TO));
            message.Subject = SUBJECT;
            message.Body = BODY;
            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                client.EnableSsl = true;
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }
        }

    }
}