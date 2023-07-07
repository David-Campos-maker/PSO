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
                            new Event { Name = "Event A", Date = new DateTime(2023, 07 , 07), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(0, 20, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 07 , 07), Time = new TimeSpan(9, 30, 0), Duration = new TimeSpan(0, 30, 0) } ,
                            new Event { Name = "Event C", Date = new DateTime(2023, 07 , 07), Time = new TimeSpan(8, 0, 0), Duration = new TimeSpan(1, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event D", Date = new DateTime(2023, 07 , 07), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(3, 30, 0) } ,
                            new Event { Name = "Event E", Date = new DateTime(2023, 07 , 07), Time = new TimeSpan(12, 45, 0), Duration = new TimeSpan(2, 0, 0) }
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
                            new Event { Name = "Event A", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(13, 30, 0), Duration = new TimeSpan(3, 30, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event C", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(3, 35, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(12, 45, 0), Duration = new TimeSpan(0, 10, 0) }
                        }
                    },
                    new User {
                        Name = "Rodrigo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event E", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(7, 0, 0), Duration = new TimeSpan(3, 35, 0) } ,
                            new Event { Name = "Event F", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(14, 30, 0), Duration = new TimeSpan(0, 10, 0) }
                        }
                    }
                }
            },
            new Event {
                Name = "Event 3" , 
                Date = new DateTime(2023 , 07 , 04) ,
                Time = new TimeSpan(8 , 0 , 0) ,
                Duration = new TimeSpan(1, 0, 0) ,
                Priority = false ,
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(0, 20, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(9, 30, 0), Duration = new TimeSpan(0, 30, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event C", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(3, 35, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 07 , 08), Time = new TimeSpan(12, 45, 0), Duration = new TimeSpan(0, 10, 0) }
                        }
                    },
                    new User {
                        Name = "Rodrigo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event E", Date = new DateTime(2023, 07 , 09), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event F", Date = new DateTime(2023, 07 , 09), Time = new TimeSpan(12, 0, 0), Duration = new TimeSpan(2, 0, 0) },
                            new Event { Name = "Event G", Date = new DateTime(2023, 07 , 09), Time = new TimeSpan(15, 0, 0), Duration = new TimeSpan(2, 0, 0) } ,
                            new Event { Name = "Event H", Date = new DateTime(2023, 07 , 09), Time = new TimeSpan(17, 0, 0), Duration = new TimeSpan(1, 0, 0) }
                        }
                    }
                }
            }
        };

        PSOAlias.PSO pso = new PSOAlias.PSO {
            PopulationSize = 10000 ,
            Iterations = 1000 ,
            InertiaCoefficient = 0.75 ,
            CognitiveCoefficient = 1.0 ,
            SocialCoefficient = 1.0 ,
            GlobalBestPosition = new double[events.Count * 2]
        };

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