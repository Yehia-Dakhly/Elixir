using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface IRequestsUpdate
    {
        public Task UpdateRequestAsync(int RequestId, RequestUpdateSignalRDTo Data);
    }
}
