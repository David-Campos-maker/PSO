using PSO.Classes;
using PSOAlias = PSO.Classes;

public class Test {
    public static void Main() {
        List<Event> events = new List<Event> {
            new Event {
                Name = "Event 1" , 
                Date = new DateTime(2023 , 06 , 28) ,
                Time = new TimeSpan(10 , 0 , 0) ,
                Duration = new TimeSpan(1, 0, 0) ,
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 06 , 28), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(1, 0, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 06 , 28), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(0, 30, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event C", Date = new DateTime(2023, 07 , 1), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(1, 0, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 07 , 1), Time = new TimeSpan(11, 0, 0), Duration = new TimeSpan(2, 0, 0) }
                        }
                    }
                }
            }
        };

        PSOAlias.PSO pso = new PSOAlias.PSO {
            PopulationSize = 10000,
            Iterations = 100,
            InertiaCoefficient = 0.5,
            CognitiveCoefficient = 1.0,
            SocialCoefficient = 1.0,
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