
using System;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.logros;

public abstract class Logro(string id, string nombre, string descripcion, string evento)
{
    public string Id { get; } = id;
    public string Nombre { get; } = nombre;
    public string Descripcion { get; } = descripcion;

    public string Evento { get; } = evento;

    public bool Desbloqueado { get; set; }

    public DateTime? FechaDesbloqueado { get; set; }

    public abstract bool ProcesarEvento(string evento, object datos);

    public virtual void Desbloquear()
    {
        if (Desbloqueado) return;

        Desbloqueado = true;
        FechaDesbloqueado = DateTime.UtcNow;

        LoggerJuego.Info($"Logro desbloqueado: {Id}");
    }
}
