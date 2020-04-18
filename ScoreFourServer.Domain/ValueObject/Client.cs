using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.ValueObject
{
    public class Client
    {
        public Client(Guid gameUserId, Guid clientId, string name)
        {
            GameUserId = gameUserId;
            ClientId = clientId;
            Name = name;
        }

        public Guid GameUserId { get; }
        public Guid ClientId { get; }
        public string Name { get; }
    }
}
