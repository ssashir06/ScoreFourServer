using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.ValueObject
{
    public class Player
    {
        public Guid GameUserId { get; set; }
        public string Name { get; set; }
    }
}
