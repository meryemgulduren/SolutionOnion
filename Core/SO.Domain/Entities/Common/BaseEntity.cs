using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedById { get; set; } // Kullanıcı ID'si
        public bool IsDeleted { get; set; } = false;
        virtual public DateTime ModifiedDate { get; set; }
        virtual public string? ModifiedBy { get; set; }
        virtual public string? ModifiedById { get; set; } // Kullanıcı ID'si
    }
}
