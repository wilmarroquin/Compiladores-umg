using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IRepositorioAnalisis
{
    Task<int> GuardarAnalisisAsync(Analisis analisis);
}
