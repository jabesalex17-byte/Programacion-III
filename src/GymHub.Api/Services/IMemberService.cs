using GymHub.Api.DTOs;

namespace GymHub.Api.Services;

public interface IMemberService
{
    IReadOnlyList<ReadMemberDto> GetAll(string? query);
    ReadMemberDto GetById(int id);
    ReadMemberDto Create(CreateMemberDto dto);
}
