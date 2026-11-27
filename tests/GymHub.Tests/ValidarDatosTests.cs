using System.ComponentModel.DataAnnotations;
using System.Net;
using GymHub.Api.DTOs;
using GymHub.Api.Repositories;
using GymHub.Api.Services;

namespace GymHub.Tests;

public class ValidarDatosTests : IDisposable
{
    private readonly ApiFixture _api = new();
    public void Dispose() => _api.Dispose();

    private static CreateMemberDto Dto(string correo = "ana@mail.com") => new()
    { FirstName = "Ana", LastName = "Pérez", Age = 28, Email = correo };

    private MemberService Servicio() => new(new MemberRepository(_api.DataDir));

    private class Muestra
    {
        [Required(ErrorMessage = "Nombre es obligatorio")] public string Name { get; set; } = "";
        [Range(1, 10, ErrorMessage = "Cantidad fuera de rango")] public int Qty { get; set; }
    }

    [Fact]
    public void DtoValidator_reune_todos_los_errores()
    {
        var ex = Assert.Throws<DataValidationException>(() => DtoValidator.ValidateOrThrow(new Muestra()));
        Assert.Equal(2, ex.Errors.Count);
    }

    [Fact]
    public void DtoValidator_acepta_un_dto_correcto() =>
        DtoValidator.ValidateOrThrow(new Muestra { Name = "x", Qty = 3 });

    [Theory]
    [InlineData("", "Pérez", 28, "a@b.co", "Nombre es obligatorio")]
    [InlineData("Ana", "  ", 28, "a@b.co", "Apellido es obligatorio")]
    [InlineData("Ana", "Pérez", 11, "a@b.co", "Edad debe estar entre 12 y 100")]
    [InlineData("Ana", "Pérez", 101, "a@b.co", "Edad debe estar entre 12 y 100")]
    [InlineData("Ana", "Pérez", 28, "mal", "Correo debe ser un correo válido")]
    [InlineData("Ana", "Pérez", 28, "", "Correo es obligatorio")]
    public void Create_rechaza_datos_invalidos(string nombre, string apellido, int edad, string correo, string esperado)
    {
        var dto = new CreateMemberDto { FirstName = nombre, LastName = apellido, Age = edad, Email = correo };
        var ex = Assert.Throws<DataValidationException>(() => Servicio().Create(dto));
        Assert.Contains(esperado, ex.Errors);
    }

    [Fact]
    public void Create_reune_varios_errores_a_la_vez()
    {
        var ex = Assert.Throws<DataValidationException>(() => Servicio().Create(new CreateMemberDto()));
        Assert.True(ex.Errors.Count >= 4);
    }

    [Fact]
    public void Create_rechaza_un_telefono_con_letras_y_acepta_uno_valido()
    {
        var dto = Dto();
        dto.PhoneNumber = "abc";
        Assert.Throws<DataValidationException>(() => Servicio().Create(dto));
        dto.PhoneNumber = "809-555-0101";
        Assert.Equal("809-555-0101", Servicio().Create(dto).PhoneNumber);
    }

    [Fact]
    public void Create_recorta_espacios_de_nombre_y_telefono()
    {
        var dto = new CreateMemberDto { FirstName = " Ana ", LastName = " Pérez ", Age = 28, Email = "ana@mail.com", PhoneNumber = " 809-555-0101 " };
        var read = Servicio().Create(dto);
        Assert.Equal("Ana", read.FirstName);
        Assert.Equal("Pérez", read.LastName);
        Assert.Equal("809-555-0101", read.PhoneNumber);
    }

    [Fact]
    public void Un_telefono_vacio_se_guarda_como_nulo_y_uno_en_blanco_se_rechaza()
    {
        var dto = Dto();
        dto.PhoneNumber = "";
        Assert.Null(Servicio().Create(dto).PhoneNumber);
        var otro = Dto("otro@mail.com");
        otro.PhoneNumber = "   ";
        Assert.Throws<DataValidationException>(() => Servicio().Create(otro));
    }

    [Fact]
    public void Create_rechaza_un_correo_repetido_sin_importar_mayusculas()
    {
        var svc = Servicio();
        svc.Create(Dto());
        var ex = Assert.Throws<DataValidationException>(() => svc.Create(Dto("ANA@MAIL.COM")));
        Assert.Contains("Ya existe un socio con ese correo", ex.Errors);
    }

    [Fact]
    public void Update_permite_conservar_el_propio_correo_pero_no_usar_el_de_otro()
    {
        var svc = Servicio();
        svc.Create(Dto("a@mail.com"));
        svc.Create(Dto("b@mail.com"));
        svc.Update(1, new UpdateMemberDto { FirstName = "Ana", LastName = "Pérez", Age = 30, Email = "a@mail.com" });
        Assert.Throws<DataValidationException>(() =>
            svc.Update(1, new UpdateMemberDto { FirstName = "Ana", LastName = "Pérez", Age = 30, Email = "b@mail.com" }));
    }

    [Fact]
    public async Task Post_con_datos_invalidos_devuelve_422_con_todos_los_mensajes()
    {
        var (status, body) = await _api.Send(HttpMethod.Post, "/api/members",
            new { firstName = "", lastName = "P", age = 5, email = "mal" });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Equal(3, body.GetProperty("errors").GetArrayLength());
    }

    [Fact]
    public async Task Put_con_datos_invalidos_devuelve_422_y_no_modifica_al_socio()
    {
        await _api.Send(HttpMethod.Post, "/api/members", new { firstName = "Ana", lastName = "Pérez", age = 28, email = "ana@mail.com" });
        var (status, _) = await _api.Send(HttpMethod.Put, "/api/members/1", new { firstName = "", lastName = "P", age = 28, email = "ana@mail.com" });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Equal("Ana", (await _api.Send(HttpMethod.Get, "/api/members/1")).Body.GetProperty("firstName").GetString());
    }

    [Fact]
    public async Task Post_con_correo_repetido_devuelve_422()
    {
        var socio = new { firstName = "Ana", lastName = "Pérez", age = 28, email = "ana@mail.com" };
        await _api.Send(HttpMethod.Post, "/api/members", socio);
        var (status, body) = await _api.Send(HttpMethod.Post, "/api/members", socio);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, status);
        Assert.Contains("correo", body.GetProperty("errors").ToString());
    }

    [Fact]
    public async Task Un_cuerpo_vacio_devuelve_422()
    {
        using var client = _api.CreateClient();
        var response = await client.PostAsync("/api/members", new StringContent("", System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}
