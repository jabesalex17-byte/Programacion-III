using System.Net;
using GymHub.Api.DTOs;
using GymHub.Api.Repositories;
using GymHub.Api.Services;

namespace GymHub.Tests;

public class EliminarSocioTests : IDisposable
{
    private readonly ApiFixture _api = new();
    public void Dispose() => _api.Dispose();

    private static object Socio(string correo = "ana@mail.com") =>
        new { firstName = "Ana", lastName = "Pérez", age = 28, email = correo };

    [Fact]
    public void Delete_elimina_al_socio_y_un_id_inexistente_lanza_NotFoundException()
    {
        var svc = new MemberService(new MemberRepository(_api.DataDir));
        svc.Create(new CreateMemberDto { FirstName = "Ana", LastName = "Pérez", Age = 28, Email = "ana@mail.com" });
        svc.Delete(1);
        Assert.Empty(svc.GetAll(null));
        Assert.Throws<NotFoundException>(() => svc.Delete(1));
    }

    [Fact]
    public async Task Delete_responde_204_y_el_socio_ya_no_existe()
    {
        await _api.Send(HttpMethod.Post, "/api/members", Socio());
        Assert.Equal(HttpStatusCode.NoContent, (await _api.Send(HttpMethod.Delete, "/api/members/1")).Status);
        Assert.Equal(HttpStatusCode.NotFound, (await _api.Send(HttpMethod.Get, "/api/members/1")).Status);
    }

    [Fact]
    public async Task Delete_de_un_id_inexistente_devuelve_404()
    {
        var (status, body) = await _api.Send(HttpMethod.Delete, "/api/members/42");
        Assert.Equal(HttpStatusCode.NotFound, status);
        Assert.Equal("Socio no encontrado", body.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Borrar_un_socio_no_afecta_a_los_demas()
    {
        await _api.Send(HttpMethod.Post, "/api/members", Socio("a@mail.com"));
        await _api.Send(HttpMethod.Post, "/api/members", Socio("b@mail.com"));
        await _api.Send(HttpMethod.Delete, "/api/members/1");
        var (_, list) = await _api.Send(HttpMethod.Get, "/api/members");
        Assert.Equal(1, list.GetArrayLength());
        Assert.Equal(2, list[0].GetProperty("id").GetInt32());
    }
}
