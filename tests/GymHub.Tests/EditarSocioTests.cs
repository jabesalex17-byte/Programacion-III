using System.Net;
using GymHub.Api.DTOs;
using GymHub.Api.Repositories;
using GymHub.Api.Services;

namespace GymHub.Tests;

public class EditarSocioTests : IDisposable
{
    private readonly ApiFixture _api = new();
    public void Dispose() => _api.Dispose();

    private static object Socio(string nombre = "Ana", int edad = 28) =>
        new { firstName = nombre, lastName = "Pérez", age = edad, email = "ana@mail.com" };

    private MemberService Servicio()
    {
        var svc = new MemberService(new MemberRepository(_api.DataDir));
        svc.Create(new CreateMemberDto { FirstName = "Ana", LastName = "Pérez", Age = 28, Email = "ana@mail.com" });
        return svc;
    }

    [Fact]
    public void Update_reemplaza_los_datos_y_conserva_el_id()
    {
        var read = Servicio().Update(1, new UpdateMemberDto { FirstName = "Anita", LastName = "Pérez", Age = 29, Email = "a@b.co" });
        Assert.Equal(1, read.Id);
        Assert.Equal("Anita", read.FirstName);
        Assert.Equal(29, read.Age);
    }

    [Fact]
    public void UpdateStatus_cambia_solo_el_estado()
    {
        var read = Servicio().UpdateStatus(1, new UpdateMemberStatusDto { Active = false });
        Assert.False(read.Active);
        Assert.Equal("Ana", read.FirstName);
    }

    [Fact]
    public void Update_y_UpdateStatus_de_un_id_inexistente_lanzan_NotFoundException()
    {
        var svc = Servicio();
        Assert.Throws<NotFoundException>(() => svc.Update(9, new UpdateMemberDto()));
        Assert.Throws<NotFoundException>(() => svc.UpdateStatus(9, new UpdateMemberStatusDto()));
    }

    [Fact]
    public async Task Put_actualiza_el_socio_por_HTTP()
    {
        await _api.Send(HttpMethod.Post, "/api/members", Socio());
        var (status, body) = await _api.Send(HttpMethod.Put, "/api/members/1", Socio("Anita", 30));
        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Equal("Anita", body.GetProperty("firstName").GetString());
        var (_, again) = await _api.Send(HttpMethod.Get, "/api/members/1");
        Assert.Equal(30, again.GetProperty("age").GetInt32());
    }

    [Fact]
    public async Task Patch_status_desactiva_al_socio_por_HTTP()
    {
        await _api.Send(HttpMethod.Post, "/api/members", Socio());
        var (status, body) = await _api.Send(HttpMethod.Patch, "/api/members/1/status", new { active = false });
        Assert.Equal(HttpStatusCode.OK, status);
        Assert.False(body.GetProperty("active").GetBoolean());
    }

    [Fact]
    public async Task Put_y_Patch_de_un_id_inexistente_devuelven_404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _api.Send(HttpMethod.Put, "/api/members/9", Socio())).Status);
        Assert.Equal(HttpStatusCode.NotFound,
            (await _api.Send(HttpMethod.Patch, "/api/members/9/status", new { active = true })).Status);
    }
}
