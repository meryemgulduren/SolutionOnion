using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class ProposalItem : BaseEntity
    {
        public string Name { get; set; } // Kalemin adı, örn: "Web Sitesi Tasarımı"
        public string? Description { get; set; } // Kalemin detayı

        public int Quantity { get; set; } // Miktar, örn: 20 (saat), 1 (adet)
        public string Unit { get; set; } // Birim, örn: "Saat", "Adet", "Ay"

        public decimal UnitPrice { get; set; } // Birim Fiyat

        // Bu kalemin toplam tutarı (Miktar * Birim Fiyat).
        // Bu alanın veritabanında bir kolon olmasına gerek yok.
        // Kod içerisinden hesaplanması daha doğru olur.
        public decimal TotalPrice => Quantity * UnitPrice;


        // --- İlişkiler (Relationships) ---

        // Bu kalem hangi teklife ait?
        public Guid ProposalId { get; set; } // Foreign Key

        // Navigation Property
        public virtual Proposal Proposal { get; set; }
    }
}
