namespace PSO.Classes {
    public class Event {
        public required string Name {get; set;}
        public DateTime Date {get; set;}
        public TimeSpan Time {get; set;}
        public List<User>? Participants {get; set;}
        public TimeSpan Duration {get; set;}
    }
}
