using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.ValueObject
{
    public class Movement
    {
        public Movement(int x, int y, int counter, Guid gameUserId, Guid gameRoomId, DateTimeOffset createDate)
        {
            this.X = x;
            this.Y = y;
            this.Counter = counter;
            this.GameUserId = gameUserId;
            this.GameRoomId = gameRoomId;
            this.CreateDate = createDate;
        }

        public int X { get; }
        public int Y { get; }
        public int Counter { get; }
        public Guid GameUserId { get; }
        public Guid GameRoomId { get; }
        public DateTimeOffset CreateDate { get; }
    }
}
