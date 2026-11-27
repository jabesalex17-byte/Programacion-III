using System.Net;

namespace GymHub.Tests;

public class CoreTests : IDisposable
{
    private readonly ApiFixture _api = new();
    public void Dispose() => _api.Dispose();

    [Fact]
    public void DataValidationException_acepta_un_solo_mensaje() =>
        Assert.Single(new DataValidationException("uno").Errors);

    [Fact]
    public void DataValidationException_acepta_varios_mensajes() =>
        Assert.Equal(2, new DataValidationException(["a", "b"]).Errors.Count);

    [Fact]
    public async Task Una_ruta_de_la_api_inexistente_devuelve_404()
    {
        var (status, _) = await _api.Send(HttpMethod.Get, "/api/no-existe");
        Assert.Equal(HttpStatusCode.NotFound, status);
    }
}
