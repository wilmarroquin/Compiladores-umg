using MiniLenguajeLexico.Application.DTOs;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IServicioAnalizadorLexico
{
    Task<ResultadoAnalisisDto> AnalizarAsync(SolicitudAnalisisDto solicitud);
}