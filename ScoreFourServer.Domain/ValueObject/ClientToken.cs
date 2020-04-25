using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Domain.ValueObject
{
    public class ClientToken
    {
        public ClientToken(Guid gameUserId, Guid clientId, Guid token, DateTimeOffset timeout)
        {
            GameUserId = gameUserId;
            ClientId = clientId;
            Token = token;
            Timeout = timeout;
        }

        public Guid GameUserId { get; }
        public Guid ClientId { get;}
        public Guid Token { get; }
        public DateTimeOffset Timeout { get; }

        public bool IsExpired => Timeout < DateTimeOffset.Now;
    }
}
