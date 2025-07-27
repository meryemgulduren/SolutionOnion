using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposal
{
    // Bu sınıf, bir teklifi güncelleme isteğini ve güncellenecek verileri temsil eder.
    public class UpdateProposalCommandRequest : IRequest<UpdateProposalCommandResponse>
    {
        public string Id { get; set; } // Hangi teklifin güncelleneceğini belirtir
        public string ProposalName { get; set; }
        public string Status { get; set; }
        // Not: Teklifin kalemleri, kilometre taşları gibi listelerin güncellenmesi
        // daha karmaşık bir mantık gerektirir ve genellikle ayrı komutlarla veya
        // bu komutun içine eklenen listelerle yönetilir. Şimdilik ana bilgileri güncelliyoruz.
    }
}