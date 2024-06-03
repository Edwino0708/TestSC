
using System.ComponentModel.DataAnnotations;

namespace TestSimetricaConsulting.Model
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "El campo de Usuario (Username) es obligatorio.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "El campo de Constraseña(Password) es obligatorio.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "El campo de Email es obligatorio.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress] 
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo de Nombre (FirstName) es obligatorio.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo de Apellido (LastName) es obligatorio.")]
        public string LastName { get; set; }
    }
}
