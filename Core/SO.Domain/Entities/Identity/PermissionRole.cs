using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SO.Domain.Entities.Identity
{
    public class PermissionRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int PermissionId { get; set; }
        
        [Required]
        public string RoleId { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("PermissionId")]
        public virtual AppPermission Permission { get; set; } = null!;
        
        [ForeignKey("RoleId")]
        public virtual AppRole Role { get; set; } = null!;
    }
}
