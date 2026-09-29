using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            var login = authService.Authenticate(request);

            if (login == null)
            {
                return Unauthorized("Usuario o password incorrectos.");
            }

            return Ok(login);
        }
    }
}
