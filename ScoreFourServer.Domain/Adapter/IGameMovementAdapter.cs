﻿using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Adapter
{
    public interface IGameMovementAdapter
    {
        Task AddAsync(GameRoom gameRoom, Movement movement, CancellationToken cancellationToken);
        Task<Movement> GetAsync(GameRoom gameRoom, int counter, CancellationToken cancellationToken);
        Task<IList<Movement>> GetListAsync(GameRoom gameRoom, CancellationToken cancellationToken);
    }
}