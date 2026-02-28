using Shared;
using Shared.DataTransferObjects;
using System.Numerics;

namespace ServiceAbstraction
{
    public interface IRequestService
    {
        public Task<bool> CreateBloodRequestAsync(CreateBloodRequestDTo bloodRequestDTo, Guid RequesterId);
        public Task DeleteBloodRequestAsync(Guid RequesterId, int BloodRequestId);
        public Task<PaginatedResult<BloodRequestDTo>> GetRequestsAsync(RequestQueryParams Params);
        public Task CloseBloodRequestAsync(Guid RequesterId, int RequestId);
        public Task<BloodRequestDTo> GetRequestByIdAsync(int RequestId);
    }
}
