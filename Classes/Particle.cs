using System;
using System.Linq;

namespace PSO.Classes {
    public class Particle {
        public double[] Position {get; set;}
        public double[] BestPosition {get; set;}
        public double[] Velocity {get; set;}
    }
}