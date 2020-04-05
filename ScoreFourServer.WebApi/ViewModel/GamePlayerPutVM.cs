using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ViewModel
{
    public class GamePlayerPutVM
    {
        public Guid GameUserId { get; set; }
        public string Name { get; set; }
    }
}
