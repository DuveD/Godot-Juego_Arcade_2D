using System;
using System.Collections.Generic;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;

namespace Primerjuego2D.nucleo.sistema.perfil;

public class Perfil
{
    public string Id { get; private set; }

    public string Nombre { get; private set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaUltimaPartida { get; set; }

    public EstadisticasGlobales EstadisticasGlobales { get; private set; }
    private readonly List<Logro> _logros;

    public IReadOnlyList<Logro> Logros => _logros;

    internal bool AnadirOActualizarLogro(Logro logro)
    {
        if (logro == null)
            throw new ArgumentNullException(nameof(logro));

        int index = _logros.FindIndex(l => l.Id == logro.Id);

        if (index >= 0)
        {
            _logros[index] = logro;
            return false; // actualizado
        }

        _logros.Add(logro);
        return true; // añadido
    }

    public Perfil(string id, string nombre, DateTime fechaCreacion, DateTime? fechaUltimaPartida)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El perfil debe tener un Id válido.", nameof(id));

        this.Id = id;
        this.Nombre = nombre;
        this.FechaCreacion = fechaCreacion;
        this.FechaUltimaPartida = fechaUltimaPartida;
        this.EstadisticasGlobales = new();
        this._logros = new();
    }
}
