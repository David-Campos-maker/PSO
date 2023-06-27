using System;
using System.Linq;

namespace PSO.Classes {
    public class Particle {
        public required double[] Position {get; set;}
        public required double[] BestPosition {get; set;}
        public required double[] Velocity {get; set;}
    }
}