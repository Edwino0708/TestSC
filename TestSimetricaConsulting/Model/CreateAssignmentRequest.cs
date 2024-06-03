
using System.ComponentModel.DataAnnotations;

namespace TestSimetricaConsulting.Model
{
    public class CreateAssignmentRequest
    {

        [Required(ErrorMessage = "El campo de Title es obligatorio.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El campo de Description es obligatorio.")]
        public string Description { get; set; }

        public DateTime? CreationDate { get; set; }

        [Required(ErrorMessage = "El campo de DueDate es obligatorio.")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "El campo de Status es obligatorio.")]
        public string Status { get; set; }
    }
}
