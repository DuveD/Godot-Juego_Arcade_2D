using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.ajustes;
using Primerjuego2D.escenas.menuPrincipal.estadisticas;
using Primerjuego2D.escenas.menuPrincipal.logros;
using Primerjuego2D.escenas.menuPrincipal.perfiles;
using Primerjuego2D.escenas.menuPrincipal.principal;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    #region Nodos escena
    public ContenedorMenuPrincipal ContenedorMenuPrincipal;
    public ContenedorMenuPerfiles ContenedorMenuPerfiles;
    public ContenedorMenuAjustes ContenedorMenuAjustes;
    public ContenedorMenuLogros ContenedorMenuLogros;
    public ContenedorMenuEstadisticas ContenedorMenuEstadisticas;
    public Label LabelVersion;
    #endregion

    public ContenedorMenu UltimoContenedorMostrado;

    public IEnumerable<ContenedorMenu> Menus;

    public override void _Ready()
    {
        CargarNodos();

        LabelVersion.Text = "v" + Ajustes.Version;

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void CargarNodos()
    {
        ContenedorMenuPrincipal = GetNode<ContenedorMenuPrincipal>("ContenedorMenuPrincipal");
        ContenedorMenuPerfiles = GetNode<ContenedorMenuPerfiles>("ContenedorMenuPerfiles");
        ContenedorMenuAjustes = GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");
        ContenedorMenuLogros = GetNode<ContenedorMenuLogros>("ContenedorMenuLogros");
        ContenedorMenuEstadisticas = GetNode<ContenedorMenuEstadisticas>("ContenedorMenuEstadisticas");
        Menus = UtilidadesNodos.ObtenerNodosDeTipo<ContenedorMenu>(this);
        LabelVersion = GetNode<Label>("LabelVersion");
    }

    public void MostrarMenuPrincipal()
    {
        MostrarMenu(this.ContenedorMenuPrincipal);
    }

    public void MostrarMenuPerfilesSinBotones()
    {
        MostrarMenuPerfiles(true);
    }

    public void MostrarMenuPerfiles()
    {
        MostrarMenuPerfiles(false);
    }

    public void MostrarMenuPerfiles(bool ocultarBotones)
    {
        OcultarMenus();
        ContenedorMenuPerfiles.Show(true, ocultarBotones);
    }

    public void MostrarMenuAjustes()
    {
        MostrarMenu(this.ContenedorMenuAjustes);
    }

    public void MostrarMenuLogros()
    {
        MostrarMenu(this.ContenedorMenuLogros);
    }

    public void MostrarMenuEstadisticas()
    {
        MostrarMenu(this.ContenedorMenuEstadisticas);
    }

    private void MostrarMenu(ContenedorMenu contenedorMenu)
    {
        OcultarMenus();

        bool seleccionarPrimerElemento = !(contenedorMenu is ContenedorMenuPrincipal);
        contenedorMenu.Show(seleccionarPrimerElemento);
    }

    private void OcultarMenus()
    {
        foreach (var menu in Menus)
            menu.Visible = false;
    }

}
