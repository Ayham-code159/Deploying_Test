using AutoMapper;
using Deploying_Test.Models.Dtos.OwnerDtos;
using Deploying_Test.Services.OwnerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Deploying_Test.Controllers
{
    [Route("api/owner")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        private readonly IMapper _mapper;


        public OwnerController(IOwnerService ownerService , IMapper mapper)
        {
            _ownerService = ownerService;
            _mapper = mapper;
            
        }

        // Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (ok, errors) = await _ownerService.RegisterAsync(dto);
            return ok ? Ok(new { message = "Registered" }) : BadRequest(new { errors });
        }

        // Login
        [HttpPost("Login")]
        [AllowAnonymous]

        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromQuery] bool rememberMe = false)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _ownerService.LoginAsync(dto, rememberMe);
            return ok ? Ok(new { message = "Logged in" }) : Unauthorized();
        }


        // Logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _ownerService.LogoutAsync();
            return Ok(new { message = "Logged out" });
        }


        [HttpPost("admin/assign-role")]
        //[Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> AssignRole([FromQuery] string user, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(role))
                return BadRequest(new { error = "Both 'user' and 'role' are required." });

            var (ok, error, targetIsCurrent) = await _ownerService.AssignRoleAsync(user, role, User);
            if (!ok) return BadRequest(new { error });


            return Ok(new { message = "Role assigned." });
        }






    }
}
