using GymHub.Api.DTOs;

namespace GymHub.Api.Services;

public interface IMemberService
{
    IReadOnlyList<ReadMemberDto> GetAll(string? query);
    ReadMemberDto GetById(int id);
    ReadMemberDto Create(CreateMemberDto dto);
    ReadMemberDto Update(int id, UpdateMemberDto dto);
    ReadMemberDto UpdateStatus(int id, UpdateMemberStatusDto dto);
    void Delete(int id);
}
