using GymHub.Api.DTOs;
using GymHub.Api.Entities;
using GymHub.Api.Repositories;

namespace GymHub.Api.Services;

public class MemberService(IMemberRepository repository) : IMemberService
{
    public IReadOnlyList<ReadMemberDto> GetAll(string? query) =>
        repository.Search(query).Select(ReadMemberDto.From).ToList();

    public ReadMemberDto GetById(int id) => ReadMemberDto.From(Find(id));

    public ReadMemberDto Create(CreateMemberDto dto)
    {
        Validate(dto, exceptId: null);
        return ReadMemberDto.From(repository.Add(ToEntity(dto)));
    }

    public ReadMemberDto Update(int id, UpdateMemberDto dto)
    {
        Find(id);
        Validate(dto, exceptId: id);
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

    public void Delete(int id)
    {
        if (!repository.Delete(id)) throw new NotFoundException("Socio no encontrado");
    }

    private Member Find(int id) =>
        repository.GetById(id) ?? throw new NotFoundException("Socio no encontrado");

    private void Validate(CreateMemberDto dto, int? exceptId)
    {
        DtoValidator.ValidateOrThrow(dto);
        if (repository.EmailExists(dto.Email.Trim(), exceptId))
            throw new DataValidationException("Ya existe un socio con ese correo");
    }

    private static Member ToEntity(CreateMemberDto dto) => new()
    {
        FirstName = dto.FirstName.Trim(), LastName = dto.LastName.Trim(), Age = dto.Age,
        Email = dto.Email.Trim(),
        PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim(),
        Active = dto.Active,
    };
}
