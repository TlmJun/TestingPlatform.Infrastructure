using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;
public class RefreshTokenRepository(AppDbContext appDbContext, IMapper mapper) : IRefreshTokenRepository
{
    public async Task SaveRefreshTokenAsync(int userId, string tokenRaw, DateTime expiresAt)
    {
        var hash = HashRefreshToken(tokenRaw);
        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        };

        appDbContext.RefreshTokens.Add(entity);
        await appDbContext.SaveChangesAsync();
    }

    public async Task<RefreshTokenDto> RevokeTokenAsync(string tokenRaw)
    {
        var hash = HashRefreshToken(tokenRaw);

        var refreshToken = await appDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash);

        if (refreshToken == null)
        {
            throw new EntityNotFoundException("Токен не найден.");
        }

        refreshToken.RevokedAt = DateTime.Now;
        appDbContext.RefreshTokens.Update(refreshToken);
        await appDbContext.SaveChangesAsync();

        return new RefreshTokenDto() { User = mapper.Map<UserDto>(refreshToken.User), Expires = refreshToken.ExpiresAt };
    }

    private static string HashRefreshToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
