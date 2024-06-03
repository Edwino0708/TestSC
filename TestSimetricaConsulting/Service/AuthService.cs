using Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using DataAcessRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace TestSimetricaConsulting.Service
{
    public class AuthService : IAuthService
    {
        private readonly OracleDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(OracleDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null; // El usuario no existe
            }

            if (!VerifyPassword(password, user.Password))
            {
                return null; // Contraseña incorrecta
            }

            return user; // Usuario autenticado correctamente
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var signingKey = new SymmetricSecurityKey(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
                }),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        public async Task<string> RegisterUser(User user)
        {
            user.Password = HashPassword(user.Password);
            bool isUserRegiste = await IsUserRegistered(user.Username, user.Email);
            // Verificar si el usuario ya está registrado
            if (isUserRegiste)
            {
                return "El usuario ya está registrado.";
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                Console.WriteLine($"Error al guardar el usuario: {ex.Message}");
                throw;
            }

            return "Usuario registrado exitosamente.";
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public async Task<bool> IsUserRegistered(string username, string email)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Username == username || u.Email == email)
                    .ToListAsync();

                return users.Any();
            }
            catch (Exception) 
            {
                return false;
            }
            // Verificar si el usuario ya está registrado por nombre de usuario o email
        }
    }
}
