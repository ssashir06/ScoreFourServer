using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.Adapters.OnMemory.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.OnMemory
{
    public class WaitingPlayerAdapter : IWaitingPlayerAdapter
    {
        internal static List<Tuple<Client, DateTimeOffset>> WaitingPlayers { get; } = new List<Tuple<Client, DateTimeOffset>>();

        public async Task<Client> DequeueAsync(CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (WaitingPlayers)
            {
                WaitingPlayers.RemoveAll(m => DateTimeOffset.Now >= m.Item2);
                if (WaitingPlayers.Any())
                {
                    var waiting = WaitingPlayers.First();
                    WaitingPlayers.Remove(waiting);
                    return waiting.Item1;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task EnqueueAsync(Client player, DateTimeOffset timeOut, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (WaitingPlayers)
            {
                WaitingPlayers.RemoveAll(m => DateTimeOffset.Now >= m.Item2);
                if (WaitingPlayers.Any(m => m.Item1.ClientId == player.ClientId))
                {
                    Trace.WriteLine($"Game user id {player.ClientId} is already enqueued.");
                }
                else
                {
                    WaitingPlayers.Add(Tuple.Create(player, timeOut));
                }
            }
        }
    }
}
