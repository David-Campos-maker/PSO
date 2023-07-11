namespace PSO.Classes {
    public class Event {
        public required string Name {get; set;}
        public required DateTime Date {get; set;}
        public required TimeSpan Time {get; set;}
        public List<User>? Participants {get; set;}
        public TimeSpan Duration {get; set;}
        public Boolean Priority {get; set;}
    }
}
