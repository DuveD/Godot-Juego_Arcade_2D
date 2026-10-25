using System;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.nucleo.utilidades.log;

[AttributeUsage(AttributeTargets.Class)]
public class AtributoNivelLog : Attribute
{
    public NivelLog NivelLog { get; }

    public AtributoNivelLog(NivelLog nivelLog)
    {
        this.NivelLog = nivelLog;
    }
}
