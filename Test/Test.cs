using System;
using System.Collections.Generic;
using System.Linq;
using PSO.Classes;
using PSOAlias = PSO.Classes;

public class Test {
    public static void Main() {
        List<Event> events = new List<Event> {
            new Event {
                Name = "Evento 1",
                Duration = new TimeSpan(1, 0, 0),
                Participants = new List<User> {
                    new User {
                        Name = "João",
                        Schedule = new List<Event> {
                            new Event { Name = "Evento A", Date = new DateTime(2022, 12, 1), Time = new TimeSpan(9, 0, 0), Duration = new TimeSpan(1, 0, 0) }
                        }
                    },
                    new User {
                        Name = "Maria",
                        Schedule = new List<Event> {
                            new Event { Name = "Evento C", Date = new DateTime(2022, 12, 3), Time = new TimeSpan(10, 0, 0), Duration = new TimeSpan(1, 0, 0) }
                        }
                    }
                }
            }
        };

        PSOAlias.PSO pso = new PSOAlias.PSO {
            PopulationSize = 100,
            Iterations = 1000,
            InertiaCoefficient = 0.5,
            CognitiveCoefficient = 1.0,
            SocialCoefficient = 1.0,
            GlobalBestPosition = new double[] {}
        };
        
        pso.Run(events);

        foreach (User user in events[0].Participants) {
            Console.WriteLine("Agenda do usuário " + user.Name + ":");
            foreach (Event e in user.Schedule) {
                Console.WriteLine(" - " + e.Name + ": " + e.Date.ToShortDateString() + " " + e.Time);
            }
        }
    }
}

