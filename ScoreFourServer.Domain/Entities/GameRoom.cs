using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.Entities
{
    public class GameRoom
    {
        public Guid GameRoomId { get; set; }
        public string Name { get; set; }
        public IList<Client> Players { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public GameRoomStatus GameRoomStatus { get; set; }
        public Guid? WinnerGameUserId { get; set; }
    }
}
