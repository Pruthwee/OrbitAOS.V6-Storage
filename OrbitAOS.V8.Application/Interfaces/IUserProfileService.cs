using OrbitAOS.V8.Application.DTOs;

namespace OrbitAOS.V8.Application.Interfaces;

/// <summary>
/// Service interface for user profile operations.
/// Migration note: In legacy MVC 6.0, this logic was typically embedded directly in
/// AccountController or HomeController. Clean Architecture extracts it to a service layer.
/// </summary>
public interface IUserProfileService
{
    /// <summary>Gets a user profile by its ID.</summary>
    Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a user profile by the associated Identity user ID.</summary>
    Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Gets all user profiles.</summary>
    Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new user profile.</summary>
    Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user profile.</summary>
    Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deactivates a user profile (soft delete).</summary>
    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
}
