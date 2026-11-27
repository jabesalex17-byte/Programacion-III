using System.Net;
using GymHub.Api.DTOs;
using GymHub.Api.Repositories;
using GymHub.Api.Services;

namespace GymHub.Tests;

public class CrearSocioTests : IDisposable
{
    private readonly ApiFixture _api = new();
    public void Dispose() => _api.Dispose();

    private static object Socio() =>
        new { firstName = "Ana", lastName = "Pérez", age = 28, email = "ana@mail.com", phoneNumber = "809-555-0101" };

    [Fact]
    public void Create_devuelve_un_ReadMemberDto_activo_y_con_id()
    {
        var svc = new MemberService(new MemberRepository(_api.DataDir));
        var read = svc.Create(new CreateMemberDto { FirstName = "Ana", LastName = "Pérez", Age = 28, Email = "ana@mail.com" });
        Assert.Equal(1, read.Id);
        Assert.True(read.Active);
    }

    [Fact]
    public async Task Post_crea_un_socio_y_responde_201_con_su_ubicacion()
    {
        var (status, body) = await _api.Send(HttpMethod.Post, "/api/members", Socio());
        Assert.Equal(HttpStatusCode.Created, status);
        Assert.Equal(1, body.GetProperty("id").GetInt32());
        Assert.True(body.GetProperty("active").GetBoolean());
    }

    [Fact]
    public async Task El_socio_creado_aparece_en_la_lista()
    {
        await _api.Send(HttpMethod.Post, "/api/members", Socio());
        await _api.Send(HttpMethod.Post, "/api/members", Socio());
        var (_, list) = await _api.Send(HttpMethod.Get, "/api/members");
        Assert.Equal(2, list.GetArrayLength());
        Assert.Equal(2, list[1].GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task Post_con_un_tipo_incorrecto_devuelve_422()
    {
        var (status, body) = await _api.Send(HttpMethod.Post, "/api/members",
            new { firstName = "A", lastName = "B", age = "abc", email = "a@b.co" });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Contains("age", body.GetProperty("errors").ToString());
    }
}
