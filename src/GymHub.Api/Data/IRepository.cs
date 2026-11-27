namespace GymHub.Api.Data;

public interface IRepository<T> where T : BaseEntity
{
    List<T> GetAll();
    T? GetById(int id);
    T Add(T item);
    bool Update(int id, T item);
    bool Delete(int id);
}
