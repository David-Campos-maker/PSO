using System;
using System.Collections.Generic;
using System.Linq;
using PSO.Classes;
using PSOAlias = PSO.Classes;

public class Test {
    public static void Main() {
        List<Event> events = new List<Event> {
            new Event {
                Name = "Event 1",
                Duration = new TimeSpan(1, 0, 0),
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 06 , 1), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(1, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 06 , 1), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(0, 30, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event C", Date = new DateTime(2023, 06 , 3), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(1, 0, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 06 , 3), Time = new TimeSpan(11, 0, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    }
                }
            }
        };

        PSOAlias.PSO pso = new PSOAlias.PSO {
            PopulationSize = 300,
            Iterations = 1000,
            InertiaCoefficient = 0.5,
            CognitiveCoefficient = 1.0,
            SocialCoefficient = 1.0,
            GlobalBestPosition = new double[events.Count * 2]
        };

        pso.Run(events);

        foreach (User user in events[0].Participants) {
            Console.WriteLine(user.Name + "` :" + "Schedule");
            foreach (Event e in user.Schedule) {
                Console.WriteLine(" - " + e.Name + ": " + e.Date.ToShortDateString() + " " + e.Time);
            }
        }
    }
}

