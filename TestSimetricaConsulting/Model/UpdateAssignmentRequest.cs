
using System.ComponentModel.DataAnnotations;

namespace TestSimetricaConsulting.Model
{
	public class UpdateAssignmentRequest
	{

		public string Title { get; set; }

		public string Description { get; set; }

		public DateTime? DueDate { get; set; }

		public string Status { get; set; }
	}
}
