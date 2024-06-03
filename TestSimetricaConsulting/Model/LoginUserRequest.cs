using System.ComponentModel.DataAnnotations;

namespace TestSimetricaConsulting.Model
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "El campo de Usuario (Username) es obligatorio.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo de Constraseña(Password) es obligatorio.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
