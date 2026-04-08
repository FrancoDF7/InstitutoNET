using Xunit;
using ISFDyT93.Negocio;

public class ValidacionesTests
{
    private readonly Validaciones _validaciones;

    public ValidacionesTests()
    {
        _validaciones = new Validaciones();
    }

    // --- Casos base ---

    [Fact]
    public void CrearSlug_TextoNormal_DeberiaConvertirCorrectamente()
    {
        var result = _validaciones.CrearSlug("Hola Mundo");

        Assert.Equal("hola-mundo", result);
    }

    [Fact]
    public void CrearSlug_TextoConAcentos_DeberiaQuitarlos()
    {
        var result = _validaciones.CrearSlug("Canción Árbol");

        Assert.Equal("cancion-arbol", result);
    }

    [Fact]
    public void CrearSlug_TextoConMayusculas_DeberiaPasarAMinusculas()
    {
        var result = _validaciones.CrearSlug("HOLA MUNDO");

        Assert.Equal("hola-mundo", result);
    }

    // --- Caracteres especiales y guiones ---

    [Fact]
    public void CrearSlug_TextoConCaracteresEspeciales_DeberiaEliminarlos()
    {
        var result = _validaciones.CrearSlug("Hola!!! Mundo@@@");

        Assert.Equal("hola-mundo", result);
    }

    [Fact]
    public void CrearSlug_TextoConMultiplesEspacios_DeberiaUnificarlos()
    {
        var result = _validaciones.CrearSlug("Hola    Mundo");

        Assert.Equal("hola-mundo", result);
    }

    [Fact]
    public void CrearSlug_TextoConMultiplesGuiones_DeberiaReducirlos()
    {
        var result = _validaciones.CrearSlug("Hola---Mundo");

        Assert.Equal("hola-mundo", result);
    }

    [Fact]
    public void CrearSlug_TextoConGuionesAlInicioYFin_DeberiaTrimarlos()
    {
        var result = _validaciones.CrearSlug("   Hola Mundo   ");

        Assert.Equal("hola-mundo", result);
    }

    // --- Edge cases ---

    [Fact]
    public void CrearSlug_TextoVacio_DeberiaRetornarStringVacio()
    {
        var result = _validaciones.CrearSlug("");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CrearSlug_TextoNull_DeberiaRetornarStringVacio()
    {
        var result = _validaciones.CrearSlug(null);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CrearSlug_TextoSoloEspacios_DeberiaRetornarStringVacio()
    {
        var result = _validaciones.CrearSlug("   ");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CrearSlug_TextoSoloCaracteresEspeciales_DeberiaRetornarStringVacio()
    {
        var result = _validaciones.CrearSlug("!@#$%");

        Assert.Equal(string.Empty, result);
    }
}