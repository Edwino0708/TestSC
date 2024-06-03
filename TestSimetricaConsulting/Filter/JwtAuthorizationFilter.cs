using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TestSimetricaConsulting.Filter
{
    public sealed class JwtAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        private string _jwtPrivateKey;
        private string _issuer;
        private string _audience;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));

            // Obtener los valores de configuración
            _jwtPrivateKey = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];

            // Verificar si se incluye el encabezado de autorización
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var bearerToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // El token debe comenzar con "Bearer "
            var token = bearerToken.ToString().StartsWith("Bearer ") ? bearerToken.ToString().Substring(7) : bearerToken.ToString();

            // Validar el token JWT
            if (!IsValidToken(token))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private bool IsValidToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                // Token inválido
                return false;
            }
        }

        private TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes(_jwtPrivateKey);
            return new TokenValidationParameters
            {
                ValidateIssuer = true,          // Validar emisor
                ValidateAudience = true,        // Validar audiencia
                ValidateLifetime = true,        // Validar vigencia
                ValidateIssuerSigningKey = true,// Validar clave de firma

                ValidIssuer = _issuer,         // Emisor válido
                ValidAudience = _audience,    // Audiencia válida
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        }
    }
}
