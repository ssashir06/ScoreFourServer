using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ViewModel
{
    public class MovementPutVM
    {
        [Range(1, 2)]
        public int PlayerNumber { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Counter { get; set; }
    }
}
