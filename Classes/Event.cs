using System;
using System.Collections.Generic;
using System.Linq;

namespace PSO.Classes {
    public class Event {
        public string Name {get; set;}
        public DateTime Date {get; set;}
        public TimeSpan Time {get; set;}
        public List<User> Participants {get; set;}
        public TimeSpan Duration {get; set;}
    }
}
