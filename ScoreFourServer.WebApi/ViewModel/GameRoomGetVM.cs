using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ViewModel
{
    public class GameRoomGetVM
    {
        public Guid GameRoomId { get; set; }
        public string Name { get; set; }
        public IList<Player> Players { get; set; }
    }
}
