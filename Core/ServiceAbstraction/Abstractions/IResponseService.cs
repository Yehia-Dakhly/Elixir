using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface IResponseService
    {
        public Task<RespondBloodRequestDTo> RespondBloodRequestAsync(Guid DonorId, int BloodRequestId); // -
        public Task<ConfirmRequestResponseDTo> ConfirmBloodRequestResponse(Guid RequesterId, Guid DonorId, int BloodRequestId, bool HasDonated); // -
        public Task<IEnumerable<DonationResponseDTo>> GetAllResponseByRequestId(Guid RequesterId, int BloodRequestId);
    }
}
