using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.ViewModel
{
    public class MovementPatchVM
    {
        public Guid GameRoomId { get; set; }
        [Range(1, 2)]
        public Guid PlayerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Counter { get; set; }
    }
}
