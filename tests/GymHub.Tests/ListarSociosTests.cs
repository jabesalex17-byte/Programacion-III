using System.Net;
using GymHub.Api.DTOs;
using GymHub.Api.Entities;
using GymHub.Api.Repositories;
using GymHub.Api.Services;

namespace GymHub.Tests;

public class ListarSociosTests : IDisposable
{
    private readonly ApiFixture _api = new();
    private readonly MemberRepository _repo;

    public ListarSociosTests()
    {
        _repo = new MemberRepository(_api.DataDir);
        _repo.Add(new Member { FirstName = "Ana", LastName = "Pérez", Age = 28, Email = "ana@mail.com" });
        _repo.Add(new Member { FirstName = "Luis", LastName = "Mejía", Age = 35, Email = "luis@mail.com", Active = false });
    }

    public void Dispose() => _api.Dispose();

    [Fact]
    public void Search_sin_texto_devuelve_todos_y_con_texto_filtra()
    {
        Assert.Equal(2, _repo.Search(null).Count);
        Assert.Equal(2, _repo.Search("  ").Count);
        Assert.Single(_repo.Search("MEJÍA"));
        Assert.Empty(_repo.Search("zzz"));
    }

    [Fact]
    public void GetAll_devuelve_ReadMemberDto()
    {
        var list = new MemberService(_repo).GetAll(null);
        Assert.Equal(2, list.Count);
        Assert.False(list[1].Active);
    }

    [Fact]
    public void GetById_de_un_id_inexistente_lanza_NotFoundException() =>
        Assert.Throws<NotFoundException>(() => new MemberService(_repo).GetById(99));

    [Fact]
    public async Task Get_lista_y_busca_por_HTTP()
    {
        Assert.Equal(2, (await _api.Send(HttpMethod.Get, "/api/members")).Body.GetArrayLength());
        Assert.Equal(1, (await _api.Send(HttpMethod.Get, "/api/members?q=ana")).Body.GetArrayLength());
    }

    [Fact]
    public async Task Get_por_id_devuelve_el_socio_o_404()
    {
        var (status, body) = await _api.Send(HttpMethod.Get, "/api/members/1");
        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Equal("Ana", body.GetProperty("firstName").GetString());
        Assert.Equal(HttpStatusCode.NotFound, (await _api.Send(HttpMethod.Get, "/api/members/99")).Status);
    }

    [Fact]
    public async Task La_pagina_principal_se_sirve_en_la_raiz()
    {
        using var client = _api.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Gym Hub", await response.Content.ReadAsStringAsync());
    }
}
