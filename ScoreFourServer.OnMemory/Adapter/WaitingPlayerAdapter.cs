using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.OnMemory.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.OnMemory.Adapter
{
    public class WaitingPlayerAdapter : IWaitingPlayerAdapter
    {
        internal static List<Tuple<Player, DateTimeOffset>> WaitingPlayers { get; } = new List<Tuple<Player, DateTimeOffset>>();

        public async Task<Player> DequeueAsync(CancellationToken cancellationToken)
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

        public async Task EnqueueAsync(Player player, DateTimeOffset timeOut, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (WaitingPlayers)
            {
                if (WaitingPlayers.Any(m => m.Item1.GameUserId == player.GameUserId))
                {
                    Trace.WriteLine($"Game user id {player.GameUserId} is already enqueued.");
                }
                else
                {
                    WaitingPlayers.Add(Tuple.Create(player, timeOut));
                }
            }
        }
    }
}
