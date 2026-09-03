using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee.api.model
{
    [Table("employee")]
    public class employeemodel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeId { get; set; }
        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string contact { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string city { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string state { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string pincode { get; set; } = string.Empty;

        [StringLength(10)]
        public string? altContact { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string address { get; set; } = string.Empty;

        [Required]
        public int designationId { get; set; }

        public DateTime? createdDate { get; set; }

        public DateTime? modifiedDate { get; set; } 

        public string role { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        public string contact { get; set; } = string.Empty;
    }
}