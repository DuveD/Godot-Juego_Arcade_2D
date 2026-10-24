using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Godot;

namespace Primerjuego2D.nucleo.localizacion;

public static class GestorIdioma
{
    public static readonly Dictionary<string, Idioma> IdiomasDisponibles = new()
        {
            { Idioma.ES.Codigo, Idioma.ES },
            { Idioma.EN.Codigo, Idioma.EN }
        };

    private static readonly Idioma IdiomaPorDefecto = Idioma.ES;

    /// <summary>
    /// Cambia el idioma de la aplicación al objeto Idioma proporcionado.
    /// </summary>
    public static void CambiarIdioma(Idioma idioma)
    {
        if (idioma == null)
            idioma = IdiomaPorDefecto;

        TranslationServer.SetLocale(idioma.Codigo);

        var culture = new CultureInfo(idioma.Codigo.Replace("_", "-"));
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    /// <summary>
    /// Obtiene el locale del sistema.
    /// </summary>
    public static string ObtenerLocaleSistema()
    {
        return TranslationServer.GetLocale();
    }

    /// <summary>
    /// Obtiene el código de idioma corto del sistema ("es", "en", etc.).
    /// </summary>
    public static string ObtenerCodigoIdiomaDeSistema()
    {
        string localeSistema = ObtenerLocaleSistema();
        return ObtenerCodigoIdiomaDeLocale(localeSistema);
    }

    /// <summary>
    /// Obtiene el código de idioma corto del sistema ("es", "en", etc.).
    /// </summary>
    public static Idioma ObtenerIdiomaDeSistema()
    {
        string codigoidiomaSistema = ObtenerCodigoIdiomaDeSistema();
        return ObtenerIdiomaDeCodigo(codigoidiomaSistema);
    }

    /// <summary>
    /// Convierte un locale ISO ("es_ES") en código corto ("es").
    /// </summary>
    public static string ObtenerCodigoIdiomaDeLocale(string locale)
    {
        if (string.IsNullOrEmpty(locale))
            return IdiomaPorDefecto.Codigo;

        int guion = locale.IndexOf('_');
        return guion > 0 ? locale.Substring(0, guion).ToLower() : locale.ToLower();
    }

    /// <summary>
    /// Devuelve el objeto Idioma correspondiente a un código ("es", "ES", "es_ES"...),
    /// o null si no es uno de los idiomas disponibles.
    /// </summary>
    public static Idioma ObtenerIdiomaDeCodigo(string codigoIdioma)
    {
        // Normalizamos a código corto en minúsculas; si viene vacío, devuelve el del idioma por defecto.
        string codigo = ObtenerCodigoIdiomaDeLocale(codigoIdioma);

        return IdiomasDisponibles.TryGetValue(codigo, out Idioma idioma) ? idioma : null;
    }
}
