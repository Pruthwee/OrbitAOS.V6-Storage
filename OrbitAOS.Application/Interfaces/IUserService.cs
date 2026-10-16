using OrbitAOS.Application.DTOs;
using OrbitAOS.Domain.Common;

namespace OrbitAOS.Application.Interfaces
{
    /// <summary>
    /// Service interface for user authentication and identity management operations.
    /// Abstracts ASP.NET Core Identity from the application layer.
    /// </summary>
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto, CancellationToken cancellationToken = default);
        Task<Result> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<Result> LogoutAsync(CancellationToken cancellationToken = default);
        Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<Result> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    }
}
