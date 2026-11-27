using GymHub.Api.Data;
using GymHub.Api.Entities;

namespace GymHub.Api.Repositories;

public class MemberRepository(string dataDir) : JsonRepository<Member>(dataDir), IMemberRepository
{
    public List<Member> Search(string? query)
    {
        var all = GetAll();
        if (string.IsNullOrWhiteSpace(query)) return all;
        var q = query.Trim();
        return all.Where(m =>
                m.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase)
                || m.LastName.Contains(q, StringComparison.OrdinalIgnoreCase)
                || m.Email.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
