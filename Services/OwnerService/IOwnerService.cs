using Deploying_Test.Models.Dtos.OwnerDtos;
using System.Security.Claims;

namespace Deploying_Test.Services.OwnerService
{
    public interface IOwnerService
    {
        Task<(bool ok, IEnumerable<string> errors)> RegisterAsync(RegisterDto dto);

        Task<bool> LoginAsync(LoginDto dto, bool rememberMe = false);

        Task LogoutAsync();

        Task<(bool ok, string? error, bool targetIsCurrentUser)> AssignRoleAsync(
         string userEmailOrUserName, string role, ClaimsPrincipal currentPrincipal
     );


    }
}