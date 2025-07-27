using SO.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Domain.Entities.ProposalModule
{
    // Bu sınıf, proje için gereken kaynakları (İnsan, Ekipman vb.) temsil eder.
    public class ResourceRequirement : BaseEntity
    {
        public string Resource { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Bu kaydın hangi teklife ait olduğu
        public Guid ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;
    }
}
