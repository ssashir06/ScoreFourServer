using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Adapters
{
    public interface IClientTokenAdapter
    {
        Task<ClientToken> GetAsync(Guid token, CancellationToken cancellationToken);
        Task SaveAsync(ClientToken clientToken, CancellationToken cancellationToken);
    }
}
