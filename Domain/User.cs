using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table(name: "USERS", Schema = "SYSTEM")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USERNAME")]
        public string Username { get; set; }
        [Column("PASSWORD")]
        public string Password { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("FIRSTNAME")]
        public string FirstName { get; set; }
        [Column("LASTNAME")]
        public string LastName { get; set; }
    }
}
