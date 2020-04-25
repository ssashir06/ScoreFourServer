using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ViewModel
{
    public class GamePlayerPutVM
    {
        public Guid GameUserId { get; set; }
        public Guid ClientId { get; set; }
        [MaxLength(30), MinLength(1)]
        public string Name { get; set; }
    }
}
