using AutoMapper;
using Deploying_Test.Models.Dtos.OwnerDtos;
using Deploying_Test.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlTypes;
using System.Security.Claims;

namespace Deploying_Test.Services.OwnerService
{
    public class OwnerService : IOwnerService
    {
        private readonly UserManager<Owner> _userManager;
        private readonly SignInManager<Owner> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; // make sure its identityRole not owner
        private readonly IMapper _mapper;


        public OwnerService(
            UserManager<Owner> userManager,
            SignInManager<Owner> signInManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;

            
        }

        public async Task<(bool ok, IEnumerable<string> errors)> RegisterAsync(RegisterDto dto)
        {
            var user = _mapper.Map<Owner>(dto);
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user,dto.Password);

            if(!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, "User");

            return (true, Array.Empty<string>());


        }

        public async Task<bool> LoginAsync(LoginDto dto, bool rememberMe = false)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if(user == null) return false;

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, rememberMe,lockoutOnFailure:false);
            return result.Succeeded;

        }

        public async Task LogoutAsync()=>await _signInManager.SignOutAsync();

        public async Task<(bool ok, string? error, bool targetIsCurrentUser)> AssignRoleAsync(
         string userEmailOrUserName, string role, ClaimsPrincipal currentPrincipal)

        {
            if(!await _roleManager.RoleExistsAsync(role))
                return(false,"Role doesn't exist.",false);

            var user = await _userManager.FindByEmailAsync(userEmailOrUserName)
                      ?? await _userManager.FindByNameAsync(userEmailOrUserName);

            if (user is null) return (false, "User not found.", false);

            var res = await _userManager.AddToRoleAsync(user, role);
            if (!res.Succeeded)
                return (false, string.Join("; ", res.Errors.Select(e => e.Description)), false);

            // Tell caller whether the assigned user is the current signed-in user
            var currentUser = await _userManager.GetUserAsync(currentPrincipal);
            var isCurrent = currentUser != null && currentUser.Id == user.Id;
            return (true, null, isCurrent);

        }

        /// Ensure role: 








    }
}
