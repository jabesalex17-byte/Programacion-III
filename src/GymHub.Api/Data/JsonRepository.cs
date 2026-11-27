using System.Text.Json;
using GymHub.Api.Entities;

namespace GymHub.Api.Data;

public class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly string _file;
    private readonly object _lock = new();

    public JsonRepository(string dataDir)
    {
        Directory.CreateDirectory(dataDir);
        var name = typeof(T).GetCustomAttributes(typeof(JsonCollectionAttribute), false)
            .Cast<JsonCollectionAttribute>().FirstOrDefault()?.Name ?? typeof(T).Name.ToLowerInvariant() + "s";
        _file = Path.Combine(dataDir, $"{name}.json");
    }

    public List<T> GetAll()
    {
        lock (_lock) return Load();
    }

    public T? GetById(int id)
    {
        lock (_lock) return Load().FirstOrDefault(x => x.Id == id);
    }

    public T Add(T item)
    {
        lock (_lock)
        {
            var items = Load();
            item.Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;
            items.Add(item);
            Save(items);
            return item;
        }
    }

    public bool Update(int id, T item)
    {
        lock (_lock)
        {
            var items = Load();
            var index = items.FindIndex(x => x.Id == id);
            if (index < 0) return false;
            item.Id = id;
            items[index] = item;
            Save(items);
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var items = Load();
            if (items.RemoveAll(x => x.Id == id) == 0) return false;
            Save(items);
            return true;
        }
    }

    private List<T> Load()
    {
        if (!File.Exists(_file)) return new List<T>();
        try
        {
            return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(_file), Options) ?? new List<T>();
        }
        catch (JsonException)
        {
            return new List<T>();
        }
    }

    private void Save(List<T> items) => File.WriteAllText(_file, JsonSerializer.Serialize(items, Options));
}
