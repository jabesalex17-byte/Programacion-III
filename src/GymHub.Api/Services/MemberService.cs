using GymHub.Api.DTOs;
using GymHub.Api.Entities;
using GymHub.Api.Repositories;

namespace GymHub.Api.Services;

public class MemberService(IMemberRepository repository) : IMemberService
{
    public IReadOnlyList<ReadMemberDto> GetAll(string? query) =>
        repository.Search(query).Select(ReadMemberDto.From).ToList();

    public ReadMemberDto GetById(int id) => ReadMemberDto.From(Find(id));

    private Member Find(int id) =>
        repository.GetById(id) ?? throw new NotFoundException("Socio no encontrado");
}
