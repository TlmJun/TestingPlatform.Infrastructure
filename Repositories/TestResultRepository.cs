using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Infrastructure.Db;

namespace TestingPlatform.Infrastructure.Repositories;

public class TestResultsRepository(AppDbContext appDbContext, IMapper mapper) : ITestResultsRepository
{
    private const int NOT_SCORED = 0;
    public Task<List<TestResultDto>> GetAllAsync()
    {
        var results = appDbContext.TestResults
            .Include(t => t.Attempt)
            .Select(t => new TestResultDto
            {
                Id = t.Id,
                StudentId = t.StudentId,
                TestId = t.TestId,
                AttemptId = t.AttemptId,
                Passed = t.Passed,
                BestScore = t.Attempt.Score ?? NOT_SCORED  
            })
            .ToListAsync();

        return results;
    }

    public async Task<List<TestResultDto>> GetByStudentIdAsync(int studentId)
    {
        var results = await appDbContext.TestResults
            .Include(t => t.Attempt)
            .Where(t => t.StudentId == studentId)
            .Select(t => new TestResultDto
            {
                Id = t.Id,
                TestId = t.TestId,
                AttemptId = t.AttemptId,
                Passed = t.Passed,
                BestScore = t.Attempt.Score ?? NOT_SCORED
            })
            .ToListAsync();

        return results;
    }
}

