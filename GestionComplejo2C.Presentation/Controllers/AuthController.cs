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
        private readonly IAutenticacionService autenticacionService;

        public AuthController(IAutenticacionService autenticacionService)
        {
            this.autenticacionService = autenticacionService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            var login = autenticacionService.Autenticar(request);

            if (login == null)
            {
                return Unauthorized("Usuario o password incorrectos.");
            }

            return Ok(login);
        }
    }
}
