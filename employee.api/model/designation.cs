using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee.api.model
{
    [Table("designation")]
    public class designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int designationId { get; set; }
        public int departmentId { get; set; }
        [Required, MaxLength(50)]
        public string designationName { get; set; } = string.Empty;
    }
    
}
