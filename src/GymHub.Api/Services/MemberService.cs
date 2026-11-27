using GymHub.Api.DTOs;
using GymHub.Api.Entities;
using GymHub.Api.Repositories;

namespace GymHub.Api.Services;

public class MemberService(IMemberRepository repository) : IMemberService
{
    public IReadOnlyList<ReadMemberDto> GetAll(string? query) =>
        repository.Search(query).Select(ReadMemberDto.From).ToList();

    public ReadMemberDto GetById(int id) => ReadMemberDto.From(Find(id));

    public ReadMemberDto Create(CreateMemberDto dto) =>
        ReadMemberDto.From(repository.Add(ToEntity(dto)));

    public ReadMemberDto Update(int id, UpdateMemberDto dto)
    {
        Find(id);
        var member = ToEntity(dto);
        repository.Update(id, member);
        return ReadMemberDto.From(member);
    }

    public ReadMemberDto UpdateStatus(int id, UpdateMemberStatusDto dto)
    {
        var member = Find(id);
        member.Active = dto.Active;
        repository.Update(id, member);
        return ReadMemberDto.From(member);
    }

    private Member Find(int id) =>
        repository.GetById(id) ?? throw new NotFoundException("Socio no encontrado");

    private static Member ToEntity(CreateMemberDto dto) => new()
    {
        FirstName = dto.FirstName, LastName = dto.LastName, Age = dto.Age,
        Email = dto.Email, PhoneNumber = dto.PhoneNumber, Active = dto.Active,
    };
}
