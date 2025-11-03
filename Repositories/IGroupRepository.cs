using TestingPlatform.Application.Dtos;

namespace TestingPlatform.Application.Interfaces;

public interface IGroupRepository
{
    Task<List<GroupDto>> GetAllAsync();
    Task<GroupDto> GetByIdAsync(int id);

    Task<int> CreateAsync(GroupDto groupDto);

    Task UpdateAsync(GroupDto groupDto);
    Task DeleteAsync(int id);
}


