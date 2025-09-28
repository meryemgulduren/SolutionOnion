using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SO.Domain.Entities.Identity
{
    public class AppPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? Module { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }
}
