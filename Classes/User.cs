using System;
using System.Collections.Generic;
using System.Linq;

namespace PSO.Classes {
    public class User {
        public required string Name {get; set;}
        public List<Event>? Schedule {get; set;}
    }
}