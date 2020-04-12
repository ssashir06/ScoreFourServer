using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.OnMemory.Tools
{
    class Dummy
    {
        public static async Task Delay(CancellationToken cancellationToken)
        {
            #if DEBUG
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            #endif
        }
    }
}
