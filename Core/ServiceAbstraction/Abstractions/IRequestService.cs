using Shared;
using Shared.DataTransferObjects;
using System.Numerics;

namespace ServiceAbstraction.Abstractions
{
    public interface IRequestService
    {
        public Task<bool> CreateBloodRequestAsync(CreateBloodRequestDTo bloodRequestDTo, Guid RequesterId);
        public Task DeleteBloodRequestAsync(Guid RequesterId, int BloodRequestId);
        public Task<PaginatedResult<BloodRequestDTo>> GetRequestsAsync(RequestQueryParams Params, Guid DonorId);
        public Task CloseBloodRequestAsync(Guid RequesterId, int RequestId);
        public Task<BloodRequestDTo> GetRequestByIdAsync(int RequestId, Guid UserId);
        public Task<PaginatedResult<PersonalRequestsDTo>> GetPersonalRequestsAsync(PersonalRequestsQueryParams queryParams, Guid DonorId);
    }
}
