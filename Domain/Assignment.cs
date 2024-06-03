using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table(name: "ASSIGNMENT", Schema = "SYSTEM")]
    public class Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }
        [Column("TITLE")]
        public string Title { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("CREATIONDATE")]
        public DateTime CreationDate { get; set; }
        [Column("DUEDATE")]
        public DateTime? DueDate { get; set; }
        [Column("STATUS")]
        public string Status { get; set; }
    }
}