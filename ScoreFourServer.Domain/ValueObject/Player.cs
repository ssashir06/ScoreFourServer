using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.ValueObject
{
    public class Player
    {
        public Player(Guid gameUserId, string name)
        {
            GameUserId = gameUserId;
            Name = name;
        }

        public Guid GameUserId { get; }
        public string Name { get; }
    }
}
