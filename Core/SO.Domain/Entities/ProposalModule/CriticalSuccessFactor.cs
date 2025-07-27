using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class CriticalSuccessFactor : BaseEntity
    {
        // --- Temel Özellikler & Veri Alanları ---
        public string Description { get; set; } // Faktörün açıklaması. Örn: "Proje boyunca müşteri temsilcisinin haftada en az 5 saat ayırması."


        // --- İlişkiler (Relationships) ---

        // Bu faktör hangi teklife ait?
        public Guid ProposalId { get; set; } // Foreign Key

        // Navigation Property
        public virtual Proposal Proposal { get; set; }
    }
}
