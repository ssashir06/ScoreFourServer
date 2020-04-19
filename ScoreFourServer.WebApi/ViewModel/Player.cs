using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ViewModel
{
    public class Player
    {
        public string Name { get; set; }
        public Guid GameUserId { get; set; }
    }
}
