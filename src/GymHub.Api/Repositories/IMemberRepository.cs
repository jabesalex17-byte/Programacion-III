using GymHub.Api.Data;
using GymHub.Api.Entities;

namespace GymHub.Api.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    List<Member> Search(string? query);
}
