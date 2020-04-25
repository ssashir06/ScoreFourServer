using ScoreFourServer.Adapters.OnMemory.Tools;
using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.OnMemory
{
    public class ClientTokenAdapter : IClientTokenAdapter
    {
        public static List<ClientToken> ClientTokens = new List<ClientToken>();

        public async Task<ClientToken> GetAsync(Guid token, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (ClientTokens)
            {
                return ClientTokens.FirstOrDefault(m => m.Token == token);
            }
        }

        public async Task SaveAsync(ClientToken clientToken, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (ClientTokens)
            {
                ClientTokens.RemoveAll(m => m.Token == clientToken.Token);
                ClientTokens.Add(clientToken);
            }
        }
    }
}
