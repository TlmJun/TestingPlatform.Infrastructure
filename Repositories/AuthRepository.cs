using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class AuthRepository(AppDbContext appDbContext, IMapper mapper) : IAuthRepository
{
    public async Task<UserDto> AuthorizeUser(UserLoginDto userLoginDto)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == userLoginDto.Login);

        if (user == default)
            throw new EntityNotFoundException($"Пользователь {userLoginDto.Login} не найден.");

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
            throw new ArgumentException("Введен неверный пароль.");

        return mapper.Map<UserDto>(user);
    }
}