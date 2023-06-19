using System;
using System.Collections.Generic;

public class Event {
    public string Name {get; set;}
    public DateTime Date {get; set;}
    public TimeSpan Time {get; set;}
    public List<User> Participants {get; set;}
}