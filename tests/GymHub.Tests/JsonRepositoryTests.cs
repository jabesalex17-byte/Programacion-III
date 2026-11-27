using GymHub.Api.Data;
using GymHub.Api.Entities;

namespace GymHub.Tests;

public class JsonRepositoryTests : IDisposable
{
    [JsonCollection("things")]
    private class Thing : BaseEntity
    {
        public string Name { get; set; } = "";
    }

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "gymhub-repo-" + Guid.NewGuid().ToString("N"));
    private JsonRepository<Thing> Repo() => new(_dir);

    public void Dispose()
    {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
    }

    [Fact]
    public void Add_asigna_ids_consecutivos()
    {
        var repo = Repo();
        Assert.Equal(1, repo.Add(new Thing { Name = "a" }).Id);
        Assert.Equal(2, repo.Add(new Thing { Name = "b" }).Id);
    }

    [Fact]
    public void Los_datos_persisten_en_un_archivo_json()
    {
        Repo().Add(new Thing { Name = "a" });
        Assert.True(File.Exists(Path.Combine(_dir, "things.json")));
        Assert.Single(Repo().GetAll());
    }

    [Fact]
    public void Update_y_Delete_devuelven_false_si_no_existe()
    {
        var repo = Repo();
        Assert.False(repo.Update(9, new Thing { Name = "x" }));
        Assert.False(repo.Delete(9));
    }

    [Fact]
    public void Update_reemplaza_y_Delete_elimina()
    {
        var repo = Repo();
        var t = repo.Add(new Thing { Name = "a" });
        Assert.True(repo.Update(t.Id, new Thing { Name = "z" }));
        Assert.Equal("z", repo.GetById(t.Id)!.Name);
        Assert.True(repo.Delete(t.Id));
        Assert.Null(repo.GetById(t.Id));
    }
}
