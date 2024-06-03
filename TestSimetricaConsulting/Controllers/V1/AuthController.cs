using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestSimetricaConsulting.Model;
using TestSimetricaConsulting.Service;

namespace TestSimetricaConsulting.Controllers.V1
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="user">Datos del nuevo usuario.</param>
        /// <returns>El usuario creado.</returns>
        /// <response code="200">Usuario creada</response>
        /// <response code="204">No se pudo creare el usuario</response>
        /// <response code="400">Datos del usuario inválidos</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest user)
        {
            // Validar el modelo
            if (!ModelState.IsValid || string.IsNullOrEmpty(user.Username))
            {
                return BadRequest(ModelState);
            }

            try
            {
                string result = await _authService.RegisterUser(new Domain.User()
                {
                    Username = user.Username,
                    Password = user.Password,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                });

                if (result is null)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Manejar errores
                return StatusCode(500, new { message = "Error al registrar el usuario." });
            }
        }
        /// <summary>
        /// Iniciar session con el usuario.
        /// </summary>
        /// <param name="login">Datos del nuevo usuario.</param>
        /// <returns>Token del usuario fue creado.</returns>
        /// <response code="200">Token creado del usuario</response>
        /// <response code="400">Datos del usuario inválidos</response>
        /// <response code="401">Autenticación requerida</response>
        /// <response code="500">Error procesando la respuesta</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest login)
        {
            // Validar el modelo
            if (!ModelState.IsValid || string.IsNullOrEmpty(login.UserName))
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _authService.Authenticate(login.UserName, login.Password);

                if (user == null)
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

                var token = _authService.GenerateToken(user);
                return Ok(new LoginUserResponse(){ Token = token });
            }
            catch (Exception ex)
            {
                // Manejar errores
                return StatusCode(500, new { message = "Error al intentar logiar el usuario." });
            }
        }
    }
}
