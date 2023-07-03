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
                Participants = new List<User> {
                    new User {
                        Name = "David",
                        Schedule = new List<Event> {
                            new Event { Name = "Event A", Date = new DateTime(2023, 07 , 03), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(0, 20, 0) } ,
                            new Event { Name = "Event B", Date = new DateTime(2023, 07 , 03), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(0, 30, 0) }
                        }
                    },
                    new User {
                        Name = "Eduardo",
                        Schedule = new List<Event> {
                            new Event { Name = "Event C", Date = new DateTime(2023, 07 , 03), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(3, 35, 0) } ,
                            new Event { Name = "Event D", Date = new DateTime(2023, 07 , 03), Time = new TimeSpan(11, 0, 0), Duration = new TimeSpan(0, 10, 0) }
                        }
                    }
                }
            }
        };

        PSOAlias.PSO pso = new PSOAlias.PSO {
            PopulationSize = 100000 ,
            Iterations = 10000 ,
            InertiaCoefficient = 0.025 ,
            CognitiveCoefficient = 0.5 ,
            SocialCoefficient = 0.5 ,
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