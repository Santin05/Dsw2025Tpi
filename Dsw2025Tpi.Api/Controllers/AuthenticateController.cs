using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly JwtTokenService jwtTokenService;
        private readonly CustomersManagementService customersManagementService;
        public AuthenticateController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            JwtTokenService jwtTokenService,
            CustomersManagementService customersManagementService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtTokenService = jwtTokenService;
            this.customersManagementService = customersManagementService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel request) 
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user==null) { return Unauthorized("El usuario o contraseña ingresados no son correctos."); }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) { return Unauthorized("El usuario o contraseña ingresados no son correctos."); }

            var role = await userManager.GetRolesAsync(user);
            var token = jwtTokenService.CreateToken(user.UserName, role.FirstOrDefault());
            return Ok(new {token});
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel request) 
        {
            if (request.Role != "Admin" && request.Role != "User") { return BadRequest("Role ingresado invalido (User o Admin)."); }

            if (request.Role == "User")
            {
                try
                {
                    await customersManagementService.addCustomer(request);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (DuplicateEntityException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            var user = new IdentityUser { UserName = request.Username, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) { await customersManagementService.deleteCustomer(request.Username); return BadRequest(result.Errors); }

            var role = request.Role;
            var roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await customersManagementService.deleteCustomer(request.Username);
                return BadRequest(roleResult.Errors);
            }

            return Ok("Usuario registrado exitosamente.");
        }

        [HttpPatch]
        public async Task<IActionResult> DeleteUser([FromBody] LoginModel request)
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                return NotFound($"Usuario con el Username '{request.Username}' no encontrado.");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var role = await userManager.GetRolesAsync(user);
                if (role.FirstOrDefault() == "User") { await customersManagementService.deleteCustomer(request.Username); }
                return Ok($"Usuario '{user.UserName}' eliminado exitosamente.");
            }
            else
            {

                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors, Message = "Fallo al eliminar el usuario." });
            }
        }

        [HttpPost("register/admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel request)
        {
            var user = new IdentityUser { UserName = request.Username, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) { return BadRequest(result.Errors); }

            const string defaultRole = "Admin";
            var roleResult = await userManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded) { return BadRequest(roleResult.Errors); }

            return Ok("Usuario registrado exitosamente.");
        }
    }
}
