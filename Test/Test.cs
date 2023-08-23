using PSO.Classes;
using PSOAlias = PSO.Classes;

public class Test {
    public static void Main() {
        List<Event> events = new List<Event> {
            new Event {
                Name = "Event 1" , 
                Date = new DateTime(2023 , 07 , 03) ,
                Time = new TimeSpan(10 , 0 , 0) ,
                Duration = new TimeSpan(1, 0, 0) ,
                Priority = false ,
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event C", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event E", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event F", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event G", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event H", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event I", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event J", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event K", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event L", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event M", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event N", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event O", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event P", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    }
                }
            },
            new Event {
                Name = "Event 2" , 
                Date = new DateTime(2023 , 07 , 03) ,
                Time = new TimeSpan(8 , 0 , 0) ,
                Duration = new TimeSpan(1, 0, 0) ,
                Priority = true ,
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event C", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event E", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event F", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event G", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event H", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event I", Date = new DateTime(2023, 08 , 26), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event J", Date = new DateTime(2023, 08 , 26), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event K", Date = new DateTime(2023, 08 , 26), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event L", Date = new DateTime(2023, 08 , 26), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event M", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event N", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event O", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event P", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Rodrigo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event Q", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event R", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event S", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event T", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event U", Date = new DateTime(2023, 08 , 25), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event V", Date = new DateTime(2023, 08 , 25), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event W", Date = new DateTime(2023, 08 , 25), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event X", Date = new DateTime(2023, 08 , 25), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    }
                }
            },
            new Event {
                Name = "Event 3" , 
                Date = new DateTime(2023 , 07 , 04) ,
                Time = new TimeSpan(8 , 0 , 0) ,
                Duration = new TimeSpan(3, 0, 0) ,
                Priority = true ,
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event C", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event E", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event F", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event G", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event H", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event I", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event J", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event K", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event L", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event M", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event N", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event O", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event P", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event Q", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event R", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event S", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event T", Date = new DateTime(2023, 08 , 22), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event U", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event V", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event W", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event X", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Rodrigo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event Y", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event Z", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event A2", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event B2", Date = new DateTime(2023, 08 , 21), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event C2", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event D2", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event E2", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event F2", Date = new DateTime(2023, 08 , 24), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event G2", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event H2", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(10, 5, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event I2", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(13, 10, 0), Duration = new TimeSpan(3, 0, 0) } ,
                            new Event { Name = "Event J2", Date = new DateTime(2023, 08 , 23), Time = new TimeSpan(16, 15, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    }
                }
            }
        };

        //Initializing PSO attributes dynamically according to list size
        int populationSize = 1000;
        int iterations = 100;
        double inertiaCoefficient = 0.5;
        double cognitiveCoefficient = 1.0;
        double socialCoefficient = 1.0;

        PSOAlias.PSO pso = new(populationSize , iterations , inertiaCoefficient , cognitiveCoefficient , socialCoefficient , events.Count);

        pso.Run(events);

        foreach (var currentEvent in events) {
            if(currentEvent.Participants != null) {
                foreach (User user in currentEvent.Participants) {
                    Console.WriteLine(user.Name + "`s: " + "Schedule");
                    
                    if(user.Schedule != null) {
                        foreach (Event e in user.Schedule) {
                            Console.WriteLine(" - " + e.Name + ": " + e.Date.ToShortDateString() + " " + e.Time);
                        }
                    }
                }
            }
        }
    }
}