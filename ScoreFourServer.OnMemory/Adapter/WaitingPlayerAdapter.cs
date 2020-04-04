using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
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
        internal static List<Player> WaitingPlayers { get; } = new List<Player>();

        public async Task<Player> DequeueAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            lock (WaitingPlayers)
            {
                if (WaitingPlayers.Count > 0)
                {
                    var waiting = WaitingPlayers.First();
                    WaitingPlayers.Remove(waiting);
                    return waiting;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task EnqueueAsync(Player player, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            lock (WaitingPlayers)
            {
                if (WaitingPlayers.Any(m => m.GameUserId == player.GameUserId))
                {
                    Trace.WriteLine($"Game user id {player.GameUserId} is already enqueued.");
                } else
                {
                    WaitingPlayers.Add(player);
                }
            }
        }
    }
}
