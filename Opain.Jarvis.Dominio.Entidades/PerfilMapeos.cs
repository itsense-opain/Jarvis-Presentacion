
using AutoMapper;
using System.Linq;

namespace Opain.Jarvis.Dominio.Entidades
{
    public class PerfilMapeos : Profile
    {
        public PerfilMapeos()
        {

            CreateMap<OperacionVueloOtd, OperacionesVuelo>()
                .ForMember(d => d.FechaVuelo, s => s.MapFrom(src => src.Fecha))
                .ForMember(d => d.MatriculaVuelo, s => s.MapFrom(src => src.Matricula))
                .ForMember(d => d.HoraVuelo, s => s.MapFrom(src => src.Hora))
                .ForMember(d => d.PAX, s => s.MapFrom(src => src.PAX))
                .ForMember(d => d.INF, s => s.MapFrom(src => src.INF))
                .ForMember(d => d.TTL, s => s.MapFrom(src => src.TTL))
                .ForMember(d => d.TTC, s => s.MapFrom(src => src.TTC))
                .ForMember(d => d.EX, s => s.MapFrom(src => src.EX))
                .ForMember(d => d.TRIP, s => s.MapFrom(src => src.TRIP))
                .ForMember(d => d.TotalEmbarcados, s => s.MapFrom(src => src.TotalEmbarcados))
                .ForMember(d => d.PagoCOP, s => s.MapFrom(src => src.PagoCOP))
                .ForMember(d => d.PagoUSD, s => s.MapFrom(src => src.PagoUSD))
                .ForMember(d => d.ConfirmacionPasajeros, s => s.MapFrom(src => src.ConfirmacionPasajeros))
                .ForMember(d => d.ConfirmacionTransitos, s => s.MapFrom(src => src.ConfirmacionTransitos))
                .ForMember(d => d.ConfirmacionGenDec, s => s.MapFrom(src => src.ConfirmacionGenDec))
                .ForMember(d => d.CanfirmacionManifiesto, s => s.MapFrom(src => src.ConfirmacionManifiesto))
                .ForMember(d => d.ConfirmacionOperacion, s => s.MapFrom(src => src.ConfirmacionOperacion))
                .ForMember(d => d.IdVuelo, s => s.MapFrom(src => src.IdVuelo))
                .ForMember(d => d.IdAerolinea, s => s.MapFrom(src => src.IdAerolinea))
                .ForMember(d => d.TipoVuelo, s => s.MapFrom(src => src.Tipo))
                .ForMember(d => d.NumeroVuelo, s => s.MapFrom(src => src.Vuelo))
                .ForMember(d => d.Destino, s => s.MapFrom(src => src.Destino))
                .ForMember(d => d.IdCargue, s => s.MapFrom(src => src.IdConsecutivoCargue))
                .ForMember(d => d.tasasReportadas, s => s.MapFrom(src => src.tasasReportadas))
                .ForMember(d => d.PAX_LIQ, s => s.MapFrom(src => src.PAX_LIQ))
                .ForMember(d => d.INF_LIQ, s => s.MapFrom(src => src.INF_LIQ))
                .ForMember(d => d.TTL_LIQ, s => s.MapFrom(src => src.TTL_LIQ))
                .ForMember(d => d.TTC_LIQ, s => s.MapFrom(src => src.TTC_LIQ))
                .ForMember(d => d.EX_LIQ, s => s.MapFrom(src => src.EX_LIQ))
                .ForMember(d => d.TRIP_LIQ, s => s.MapFrom(src => src.TRIP_LIQ))
                .ForMember(d => d.TotalEmbarcados_LIQ, s => s.MapFrom(src => src.TotalEmbarcados_LIQ))
                .ForMember(d => d.PAGOCOP_LIQ, s => s.MapFrom(src => src.PAGOCOP_LIQ))
                .ForMember(d => d.PAGOUSD_LIQ, s => s.MapFrom(src => src.PAGOUSD_LIQ));

            //.ForMember(d => d.EstadoProceso, s => s.MapFrom(src => src.EstadoProceso == "Finalizado" ? 1 : 0));

            CreateMap<OperacionesVuelo, OperacionVueloOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Fecha, s => s.MapFrom(src => src.FechaVuelo))
                .ForMember(d => d.Matricula, s => s.MapFrom(src => src.MatriculaVuelo))
                .ForMember(d => d.Hora, s => s.MapFrom(src => src.HoraVuelo))
                .ForMember(d => d.PAX, s => s.MapFrom(src => src.PAX))
                .ForMember(d => d.INF, s => s.MapFrom(src => src.INF))
                .ForMember(d => d.TTL, s => s.MapFrom(src => src.TTL))
                .ForMember(d => d.TTC, s => s.MapFrom(src => src.TTC))
                .ForMember(d => d.EX, s => s.MapFrom(src => src.EX))
                .ForMember(d => d.TRIP, s => s.MapFrom(src => src.TRIP))
                .ForMember(d => d.TotalEmbarcados, s => s.MapFrom(src => src.TotalEmbarcados))
                .ForMember(d => d.PagoCOP, s => s.MapFrom(src => src.PagoCOP))
                .ForMember(d => d.PagoUSD, s => s.MapFrom(src => src.PagoUSD))
                .ForMember(d => d.NombreAerolinea, s => s.MapFrom(src => src.Aerolinea.Nombre))
                .ForMember(d => d.Destino, s => s.MapFrom(src => src.Destino))
                .ForMember(d => d.Vuelo, s => s.MapFrom(src => src.NumeroVuelo))
                .ForMember(d => d.PDFPasajeros, s => s.MapFrom(src => src.Aerolinea.PDFPasajeros))
                .ForMember(d => d.Tipo, s => s.MapFrom(src => src.TipoVuelo))
                .ForMember(d => d.ConfirmacionPasajeros, s => s.MapFrom(src => src.ConfirmacionPasajeros))
                .ForMember(d => d.ConfirmacionTransitos, s => s.MapFrom(src => src.ConfirmacionTransitos))
                .ForMember(d => d.ConfirmacionGenDec, s => s.MapFrom(src => src.ConfirmacionGenDec))
                .ForMember(d => d.ConfirmacionManifiesto, s => s.MapFrom(src => src.CanfirmacionManifiesto))
                .ForMember(d => d.ConfirmacionOperacion, s => s.MapFrom(src => src.ConfirmacionOperacion))
                .ForMember(d => d.ConsecutivoCargue, s => s.MapFrom(src => string.Format("{0}{1}{2}-{3}", src.Cargue.FechaHora.Year, src.Cargue.FechaHora.Month, src.Cargue.FechaHora.Day, src.IdCargue)))
                .ForMember(d => d.IdConsecutivoCargue, s => s.MapFrom(src => src.IdCargue))
                .ForMember(d => d.IdVuelo, s => s.MapFrom(src => src.IdVuelo))
                .ForMember(d => d.Id_Daily, s => s.MapFrom(src => src.Id_Daily))
                .ForMember(d => d.ArchivoGendec, s => s.MapFrom(src => src.Archivos.FirstOrDefault(x => x.Tipo.Equals("GENDEC")).Nombre))
                .ForMember(d => d.ArchivoPasajeros, s => s.MapFrom(src => src.Archivos.FirstOrDefault(x => x.Tipo.Equals("PASAJEROS")).Nombre))
                .ForMember(d => d.ArchivoTransito, s => s.MapFrom(src => src.Archivos.FirstOrDefault(x => x.Tipo.Equals("TRANSITOS")).Nombre))
                .ForMember(d => d.ArchivoManifiesto, s => s.MapFrom(src => src.Archivos.FirstOrDefault(x => x.Tipo.Equals("MANIFIESTO")).Nombre))
                .ForMember(d => d.EstadoProceso, s => s.MapFrom(src => src.EstadoProceso))
                .ForMember(d => d.tasasReportadas, s => s.MapFrom(src => src.tasasReportadas))
                //.ForMember(d => d.EstadoProceso, s => s.MapFrom(src => src.EstadoProceso == 1 ? "Finalizado" : "En Proceso"))
                .ForMember(d => d.NovedadCargue, s => s.MapFrom(src => src.NovedadesProceso.Count(x => x.TipoNovedad.Equals(1))))
                .ForMember(d => d.NovedadProceso, s => s.MapFrom(src => src.NovedadesProceso.Count(x => x.TipoNovedad.Equals(2))))

                .ForMember(d => d.PAX_LIQ, s => s.MapFrom(src => src.PAX_LIQ))
                .ForMember(d => d.INF_LIQ, s => s.MapFrom(src => src.INF_LIQ))
                .ForMember(d => d.TTL_LIQ, s => s.MapFrom(src => src.TTL_LIQ))
                .ForMember(d => d.TTC_LIQ, s => s.MapFrom(src => src.TTC_LIQ))
                .ForMember(d => d.EX_LIQ, s => s.MapFrom(src => src.EX_LIQ))
                .ForMember(d => d.TRIP_LIQ, s => s.MapFrom(src => src.TRIP_LIQ))
                .ForMember(d => d.TotalEmbarcados_LIQ, s => s.MapFrom(src => src.TotalEmbarcados_LIQ))
                .ForMember(d => d.PAGOCOP_LIQ, s => s.MapFrom(src => src.PAGOCOP_LIQ))
                .ForMember(d => d.PAGOUSD_LIQ, s => s.MapFrom(src => src.PAGOUSD_LIQ));

            CreateMap<PasajeroOtd, Pasajero>()
                .ForMember(d => d.IdCategoriaPasajero, s => s.MapFrom(src => src.Categoria))
                .ForMember(d => d.IdOperacionVuelo, s => s.MapFrom(src => src.Operacion))
                .ForMember(d => d.FechaVuelo, s => s.MapFrom(src => src.Fecha))
                .ForMember(d => d.NumeroVuelo, s => s.MapFrom(src => src.NumeroVuelo))
                .ForMember(d => d.MatriculaVuelo, s => s.MapFrom(src => src.MatriculaVuelo))
                .ForMember(d => d.realiza_viaje, s => s.MapFrom(src => src.realiza_viaje))
                .ForMember(d => d.motivo_exencion, s => s.MapFrom(src => src.motivo_exencion))
                .ForMember(d => d.IdCargue, s => s.MapFrom(src => src.IdCargue))
                .ForMember(d => d.NombrePasajero, s => s.MapFrom(src => src.NombrePasajero));

            CreateMap<Pasajero, PasajeroOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Categoria, s => s.MapFrom(src => src.IdCategoriaPasajero))
                .ForMember(d => d.Operacion, s => s.MapFrom(src => src.IdOperacionVuelo))
                .ForMember(d => d.NombrePasajero, s => s.MapFrom(src => src.NombrePasajero))
                .ForMember(d => d.Fecha, s => s.MapFrom(src => src.FechaVuelo))
                .ForMember(d => d.MatriculaVuelo, s => s.MapFrom(src => src.MatriculaVuelo))
                .ForMember(d => d.NombreAerolinea, s => s.MapFrom(src => src.OperacionesVuelo.Aerolinea.Nombre))
                .ForMember(d => d.TipoVuelo, s => s.MapFrom(src => src.OperacionesVuelo.TipoVuelo))
                .ForMember(d => d.realiza_viaje, s => s.MapFrom(src => src.realiza_viaje))
                .ForMember(d => d.motivo_exencion, s => s.MapFrom(src => src.motivo_exencion))
                .ForMember(d => d.IdCargue, s => s.MapFrom(src => src.IdCargue))
                .ForMember(d => d.NumeroVuelo, s => s.MapFrom(src => src.NumeroVuelo));

            CreateMap<PasajeroTransitoOtd, PasajeroTransito>()
               .ForMember(d => d.FechaHoraCargue, s => s.MapFrom(src => src.FechaHoraCargue))
               .ForMember(d => d.Firmado, s => s.MapFrom(src => src.Firmado))
               .ForMember(d => d.FechaHoraFirma, s => s.MapFrom(src => src.FechaHoraFirma))
               .ForMember(d => d.FechaLlegada, s => s.MapFrom(src => src.FechaLlegada))
               .ForMember(d => d.IdOperacionVuelo, s => s.MapFrom(src => src.Operacion))
               .ForMember(d => d.HoraLlegada, s => s.MapFrom(src => src.HoraLlegada))
               .ForMember(d => d.FechaSalida, s => s.MapFrom(src => src.FechaSalida))
               .ForMember(d => d.HoraSalida, s => s.MapFrom(src => src.HoraSalida))
               .ForMember(d => d.NombrePasajero, s => s.MapFrom(src => src.NombrePasajero))
               .ForMember(d => d.IdVueloSalida, s => s.MapFrom(src => 0))
               .ForMember(d => d.IdVueloSalida, s => s.MapFrom(src => 0))
               .ForMember(d => d.NumeroVueloSalida, s => s.MapFrom(src => src.NumeroVueloSalida))
               .ForMember(d => d.Destino, s => s.MapFrom(src => src.Destino))
               .ForMember(d => d.NumeroVueloLlegada, s => s.MapFrom(src => src.NumeroVueloLlegada))
               .ForMember(d => d.Origen, s => s.MapFrom(src => src.Origen))
               .ForMember(d => d.AerolineaLlegada, s => s.MapFrom(src => src.AerolineaLlegada))
               .ForMember(d => d.IdCargue, s => s.MapFrom(src => src.IdCargue))
               .ForMember(d => d.Categoria, s => s.MapFrom(src => src.TTC == 1 ? "TTC" : "TTL"));

            CreateMap<PasajeroTransito, PasajeroTransitoOtd>()
               .ForMember(d => d.AerolineaSalida, s => s.MapFrom(src => src.AerolineaSalida/*OperacionesVuelo.Aerolinea.Nombre*/))
               .ForMember(d => d.IdCargue, s => s.MapFrom(src => src.IdCargue))
               .ForMember(d => d.AerolineaLlegada, s => s.MapFrom(src => src.AerolineaLlegada))
               .ForMember(d => d.FechaHoraCargue, s => s.MapFrom(src => src.FechaHoraCargue))
               .ForMember(d => d.Firmado, s => s.MapFrom(src => src.Firmado))
               .ForMember(d => d.FechaHoraFirma, s => s.MapFrom(src => src.FechaHoraFirma))
               .ForMember(d => d.FechaLlegada, s => s.MapFrom(src => src.FechaLlegada))
               .ForMember(d => d.HoraLlegada, s => s.MapFrom(src => src.HoraLlegada))
               .ForMember(d => d.FechaSalida, s => s.MapFrom(src => src.FechaSalida))
               .ForMember(d => d.HoraSalida, s => s.MapFrom(src => src.HoraSalida))
               .ForMember(d => d.NombrePasajero, s => s.MapFrom(src => src.NombrePasajero))
               .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
               .ForMember(d => d.NumeroVueloLlegada, s => s.MapFrom(src => src.NumeroVueloLlegada))
               .ForMember(d => d.Origen, s => s.MapFrom(src => src.Origen))
               .ForMember(d => d.NumeroVueloSalida, s => s.MapFrom(src => src.NumeroVueloSalida))
               .ForMember(d => d.Destino, s => s.MapFrom(src => src.Destino))
               .ForMember(d => d.Operacion, s => s.MapFrom(src => src.IdOperacionVuelo))
                .ForMember(d => d.NombreAerolinea, s => s.MapFrom(src => src.OperacionesVuelo.Aerolinea.Nombre))
                .ForMember(d => d.TipoVuelo, s => s.MapFrom(src => src.OperacionesVuelo.TipoVuelo))
                .ForMember(d => d.Tipo, s => s.MapFrom(src => src.Categoria))
               .ForMember(d => d.TTC, s => s.MapFrom(src => src.Categoria == "TTC" ? 1 : 0))
               .ForMember(d => d.TTL, s => s.MapFrom(src => src.Categoria == "TTL" ? 1 : 0));

            CreateMap<ArchivoOtd, Archivo>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Nombre, s => s.MapFrom(src => src.Nombre))
                .ForMember(d => d.Tipo, s => s.MapFrom(src => src.Tipo))
                .ForMember(d => d.IdOperacionVuelo, s => s.MapFrom(src => src.Operacion))
                .ReverseMap();

            CreateMap<Usuario, UsuarioOtd>()
                .ForMember(d => d.UsuarioAerolinea, s => s.MapFrom(src => src.UsuariosAerolineas))
                .ForMember(d => d.TwoFactorEnabled, s => s.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(d => d.UserName, s => s.MapFrom(src => src.UserName))
                .ForMember(d => d.SecurityStamp, s => s.MapFrom(src => src.SecurityStamp))
                .ForMember(d => d.PhoneNumberConfirmed, s => s.MapFrom(src => src.PhoneNumberConfirmed))
                .ForMember(d => d.PhoneNumber, s => s.MapFrom(src => src.PhoneNumber))
                .ForMember(d => d.PasswordHash, s => s.MapFrom(src => src.PasswordHash))
                .ForMember(d => d.NormalizedUserName, s => s.MapFrom(src => src.NormalizedUserName))
                .ForMember(d => d.NormalizedEmail, s => s.MapFrom(src => src.NormalizedEmail))
                .ForMember(d => d.Nombre, s => s.MapFrom(src => src.Nombre))
                .ForMember(d => d.LockoutEnd, s => s.MapFrom(src => src.LockoutEnd))
                .ForMember(d => d.LockoutEnabled, s => s.MapFrom(src => src.LockoutEnabled))
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.EmailConfirmed, s => s.MapFrom(src => src.EmailConfirmed))
                .ForMember(d => d.Email, s => s.MapFrom(src => src.Email))
                .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(src => src.ConcurrencyStamp))
                .ForMember(d => d.Apellido, s => s.MapFrom(src => src.Apellido))
                .ForMember(d => d.Telefono, s => s.MapFrom(src => src.Telefono))
                .ForMember(d => d.Cargo, s => s.MapFrom(src => src.Cargo))
                .ForMember(d => d.Activo, s => s.MapFrom(src => src.Activo))
                .ForMember(d => d.TipoDocumento, s => s.MapFrom(src => src.TipoDocumento))
                .ForMember(d => d.NumeroDocumento, s => s.MapFrom(src => src.NumeroDocumento))
                .ForMember(d => d.AccessFailedCount, s => s.MapFrom(src => src.AccessFailedCount))
                .ForMember(d => d.RolesUsuario, s => s.MapFrom(src => src.RolesUsuarios))
                .ForMember(d => d.Aerolinea, s => s.MapFrom(src => src.UsuariosAerolineas.FirstOrDefault().Aerolinea.Nombre))
                .ForMember(d => d.Perfil, s => s.MapFrom(src => src.RolesUsuarios.FirstOrDefault().Rol.Name))
                .ReverseMap();

            CreateMap<UsuariosAerolineas, UsuariosAerolineasOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.IdAerolinea, s => s.MapFrom(src => src.IdAerolinea))
                .ForMember(d => d.IdUsuario, s => s.MapFrom(src => src.IdUsuario))
                .ReverseMap();


            CreateMap<HorarioAerolinea, HorarioAerolineaOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Fecha, s => s.MapFrom(src => src.Fecha))
                .ForMember(d => d.HoraFin, s => s.MapFrom(src => src.HoraFin))
                .ForMember(d => d.HoraInicio, s => s.MapFrom(src => src.HoraInicio))
                .ForMember(d => d.IdAerolinea, s => s.MapFrom(src => src.IdAerolinea))
                .ForMember(d => d.Aerolinea, s => s.MapFrom(src => src.Aerolinea.Nombre))
                .ReverseMap();

            CreateMap<Aerolinea, AerolineaOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Nombre, s => s.MapFrom(src => src.Nombre))
                 .ForMember(d => d.Sigla, s => s.MapFrom(src => src.Sigla))
                .ForMember(d => d.Codigo, s => s.MapFrom(src => src.Codigo))
                .ForMember(d => d.IdEstado, s => s.MapFrom(src => src.IdEstado == true ? "True" : "False"))
                .ForMember(d => d.PDFPasajeros, s => s.MapFrom(src => src.PDFPasajeros))
                .ForMember(d => d.CantidadUsuarios, s => s.MapFrom(src => src.CantidadUsuarios));

            CreateMap<AerolineaOtd, Aerolinea>()
               .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
               .ForMember(d => d.Nombre, s => s.MapFrom(src => src.Nombre))
               .ForMember(d => d.Sigla, s => s.MapFrom(src => src.Sigla))
               .ForMember(d => d.IdEstado, s => s.MapFrom(src => src.IdEstado == "True" ? true : false))
               .ForMember(d => d.PDFPasajeros, s => s.MapFrom(src => src.PDFPasajeros))
               .ForMember(d => d.CantidadUsuarios, s => s.MapFrom(src => src.CantidadUsuarios));

            CreateMap<RolesUsuarios, RolesUsuariosOtd>()
                .ForMember(d => d.Rol, s => s.MapFrom(src => src.Rol));

            CreateMap<Rol, RolOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Name, s => s.MapFrom(src => src.Name))
                .ForMember(d => d.NormalizedName, s => s.MapFrom(src => src.NormalizedName))
                .ForMember(d => d.ConcurrencyStamp, s => s.MapFrom(src => src.ConcurrencyStamp))
                .ReverseMap();

            CreateMap<TasaAeroportuaria, TasaAeroportuariaOtd>().ReverseMap();

            CreateMap<HorarioOperacion, HorarioOperacionOtd>().ReverseMap();

            CreateMap<Ingreso, IngresoOtd>().ReverseMap();

            CreateMap<Ticket, TicketOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.IdAerolinea, s => s.MapFrom(src => src.IdAerolinea))
                .ForMember(d => d.NumeroTicket, s => s.MapFrom(src => src.NumeroTicket))
                .ForMember(d => d.TipoConsulta, s => s.MapFrom(src => src.TipoConsulta))
                .ForMember(d => d.Asunto, s => s.MapFrom(src => src.Asunto))
                .ForMember(d => d.FechaVuelo, s => s.MapFrom(src => src.FechaVuelo))
                .ForMember(d => d.FechaCreacion, s => s.MapFrom(src => src.FechaCreacion))
                .ForMember(d => d.Mensaje, s => s.MapFrom(src => src.Mensaje))
                .ForMember(d => d.Adjunto, s => s.MapFrom(src => src.Adjunto))
                .ForMember(d => d.Estado, s => s.MapFrom(src => src.Estado))
                .ForMember(d => d.NombreAerolinea, s => s.MapFrom(src => src.Aerolinea.Nombre))
                .ForMember(d => d.NombreUsuario, s => s.MapFrom(src => string.Format("{0} {1}", src.Usuario.Nombre, src.Usuario.Apellido)))
                .ForMember(d => d.IdUsuario, s => s.MapFrom(src => src.IdUsuario))
                .ForMember(d => d.Seguimiento, s => s.MapFrom(src => src.Seguimiento))
               .ForMember(d => d.Respuestas, s => s.MapFrom(src => src.RespuestasTickets));

            CreateMap<TicketOtd, Ticket>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.IdAerolinea, s => s.MapFrom(src => src.IdAerolinea))
                .ForMember(d => d.NumeroTicket, s => s.MapFrom(src => src.NumeroTicket))
                .ForMember(d => d.TipoConsulta, s => s.MapFrom(src => src.TipoConsulta))
                .ForMember(d => d.Asunto, s => s.MapFrom(src => src.Asunto))
                .ForMember(d => d.FechaVuelo, s => s.MapFrom(src => src.FechaVuelo))
                .ForMember(d => d.FechaCreacion, s => s.MapFrom(src => src.FechaCreacion))
                .ForMember(d => d.Mensaje, s => s.MapFrom(src => src.Mensaje))
                .ForMember(d => d.Adjunto, s => s.MapFrom(src => src.Adjunto))
                .ForMember(d => d.Estado, s => s.MapFrom(src => src.Estado))
                .ForMember(d => d.IdUsuario, s => s.MapFrom(src => src.IdUsuario))
                .ForMember(d => d.Seguimiento, s => s.MapFrom(src => src.Seguimiento));

            CreateMap<RespuestaTicket, RespuestaTicketOtd>()
               .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
               .ForMember(d => d.IdTicket, s => s.MapFrom(src => src.IdTicket))
               .ForMember(d => d.FechaCreacion, s => s.MapFrom(src => src.FechaCreacion))
               .ForMember(d => d.Mensaje, s => s.MapFrom(src => src.Mensaje))
               .ForMember(d => d.Adjunto, s => s.MapFrom(src => src.Adjunto))
               .ForMember(d => d.NombreUsuario, s => s.MapFrom(src => string.Format("{0} {1}", src.Usuario.Nombre, src.Usuario.Apellido)))
               .ForMember(d => d.IdUsuario, s => s.MapFrom(src => src.IdUsuario));

            CreateMap<RespuestaTicketOtd, RespuestaTicket>()
               .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
               .ForMember(d => d.IdTicket, s => s.MapFrom(src => src.IdTicket))
               .ForMember(d => d.FechaCreacion, s => s.MapFrom(src => src.FechaCreacion))
               .ForMember(d => d.Mensaje, s => s.MapFrom(src => src.Mensaje))
               .ForMember(d => d.Adjunto, s => s.MapFrom(src => src.Adjunto))
               .ForMember(d => d.IdUsuario, s => s.MapFrom(src => src.IdUsuario));

            CreateMap<Acceso, AccesoOtd>().ReverseMap();
            CreateMap<PoliticasDeTratamientoDeDatos, PoliticasDeTratamientoDeDatosOtd>().ReverseMap();

            CreateMap<CargueArchivo, CargueOtd>().ReverseMap();

            CreateMap<Causal, CausalOtd>().ReverseMap();

            CreateMap<NovedadOtd, NovedadProceso>()
              .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
              .ForMember(d => d.IdOperacionVuelo, s => s.MapFrom(src => src.Operacion))
              .ForMember(d => d.TipoNovedad, s => s.MapFrom(src => src.TipoNovedad == "Cargue" ? 1 : 2))
              .ForMember(d => d.IdCausal, s => s.MapFrom(src => src.IdCausal))
              .ForMember(d => d.Descripcion, s => s.MapFrom(src => src.DescNovedad))
              .ForMember(d => d.IdRegistro, s => s.MapFrom(src => src.IdRegistro))
              .ForMember(d => d.FechaHora, s => s.MapFrom(src => src.FechaHora));

            CreateMap<NovedadProceso, NovedadOtd>()
              .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
              .ForMember(d => d.Operacion, s => s.MapFrom(src => src.IdOperacionVuelo))
              .ForMember(d => d.TipoVuelo, s => s.MapFrom(src => src.OperacionesVuelo.TipoVuelo))
              .ForMember(d => d.NumeroVuelo, s => s.MapFrom(src => src.OperacionesVuelo.NumeroVuelo))
              .ForMember(d => d.FechaVuelo, s => s.MapFrom(src => src.OperacionesVuelo.FechaVuelo))
              .ForMember(d => d.HoraVuelo, s => s.MapFrom(src => src.OperacionesVuelo.HoraVuelo))
              .ForMember(d => d.NumeroMatricula, s => s.MapFrom(src => src.OperacionesVuelo.MatriculaVuelo))
              .ForMember(d => d.TipoNovedad, s => s.MapFrom(src => src.TipoNovedad == 1 ? "Cargue" : "Proceso"))
              .ForMember(d => d.IdCausal, s => s.MapFrom(src => src.IdCausal))
              .ForMember(d => d.CodCausal, s => s.MapFrom(src => src.Causal.Codigo))
              .ForMember(d => d.DescCausal, s => s.MapFrom(src => src.Causal.Descripcion))
              .ForMember(d => d.DescNovedad, s => s.MapFrom(src => src.Descripcion))
              .ForMember(d => d.IdRegistro, s => s.MapFrom(src => src.IdRegistro))
              .ForMember(d => d.FechaHora, s => s.MapFrom(src => src.FechaHora));

            CreateMap<ConsecutivoCargueOtd, Cargue>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.FechaHora, s => s.MapFrom(src => src.FechaHora))
                .ForMember(d => d.Usuario, s => s.MapFrom(src => src.Usuario))
                .ForMember(d => d.Archivo, s => s.MapFrom(src => src.Archivo))
                .ForMember(d => d.Tipo, s => s.MapFrom(src => src.Tipo == "Manual" ? 1 : 0));

            CreateMap<Cargue, ConsecutivoCargueOtd>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.FechaHora, s => s.MapFrom(src => src.FechaHora))
                .ForMember(d => d.Usuario, s => s.MapFrom(src => src.Usuario))
                .ForMember(d => d.Consecutivo, s => s.MapFrom(src => string.Format("{0}{1}{2}-{3}", src.FechaHora.Year, src.FechaHora.Month, src.FechaHora.Day, src.Id)))
                .ForMember(d => d.Archivo, s => s.MapFrom(src => src.Archivo))
                .ForMember(d => d.Tipo, s => s.MapFrom(src => src.Tipo == 1 ? "Manual" : "Archivo"));
        }
    }
}


