using System;
using System.Collections.Generic;
using System.Linq;

namespace PSO.Classes {
    public class PSO {
        public int PopulationSize {get; set;}
        public int Iterations {get; set;}
        public double InertiaCoefficient {get; set;}
        public double CognitiveCoefficient {get; set;}
        public double SocialCoefficient {get; set;}
        public double[] GlobalBestPosition {get; set;}

        private bool IsAnyDayAvailableInCurrentYear(List<Event> events) {
            // Check if there is any day available in the current year for any user
            foreach (Event e in events) {
                foreach (User user in e.Participants) {
                    for (int i = 0; i < user.Schedule.Count - 1; i++) {
                        if (user.Schedule[i].Date.Year == DateTime.Now.Year && user.Schedule[i].Time.Add(user.Schedule[i].Duration) <= user.Schedule[i + 1].Time) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private double ObjectiveFunction(double[] position, List<Event> events) {
            double quality = 0;

            for (int i = 0; i < position.Length - 1; i += 2) {
                DateTime date = DateTime.FromOADate(position[i]);
                TimeSpan time = TimeSpan.FromHours(position[i + 1]);

                // Penalize solutions that assign events to invalid times
                if (time.Hours < 7 || time.Hours > 18) {
                    quality += 1000;
                }

                // Penalize solutions that assign events to invalid dates
                if (date < DateTime.Now || (date.Year > DateTime.Now.Year && !IsAnyDayAvailableInCurrentYear(events))) {
                    quality += 1000;
                }

                foreach (Event e in events) {
                    DateTime eventStart = date.Add(time);
                    DateTime eventEnd = eventStart.Add(e.Duration);

                    foreach (User user in e.Participants) {
                        // Penalize solutions that assign events outside of the user's available schedule
                        bool isAvailable = false;
                        for (int j = 0; j < user.Schedule.Count - 1; j++) {
                            if (user.Schedule[j].Time.Add(user.Schedule[j].Duration) <= time && time.Add(e.Duration) <= user.Schedule[j + 1].Time) {
                                isAvailable = true;
                                break;
                            }
                        }
                        if (!isAvailable) {
                            quality += 1000;
                        }

                        if (user.Schedule.Any(scheduledEvent => 
                            (scheduledEvent.Date == date && scheduledEvent.Time >= time && scheduledEvent.Time < eventEnd.TimeOfDay) ||
                            (scheduledEvent.Date == date && scheduledEvent.Time < time && scheduledEvent.Time.Add(scheduledEvent.Duration) > time))) {
                            quality += 1;
                        }
                    }
                }
            }

            return quality;
        }

        public void Run(List<Event> events) {
            // Initialize the particle swarm with random values
            var swarm = new Particle[PopulationSize];
            var random = new Random();
            int dimensions = events.Count * 2; // Two dimensions (date and time) for each event

            for (int i = 0; i < PopulationSize; i++) {
                var position = new double[dimensions];
                var velocity = new double[dimensions];

                // Initialize particle position and velocity with random values
                for (int j = 0; j < position.Length; j++) {
                    if (j % 2 == 0) {
                        // Initialize the event date
                        double minValue = DateTime.Now.ToOADate();
                        double maxValue = DateTime.Now.AddYears(1).ToOADate();

                        position[j] = minValue + (maxValue - minValue) * random.NextDouble();
                        velocity[j] = minValue + (maxValue - minValue) * random.NextDouble();
                    } else {
                        // Initialize event time
                        int minValue = 0;
                        int maxValue = 23;

                        position[j] = random.Next(minValue, maxValue + 1);
                        velocity[j] = random.Next(minValue, maxValue + 1);
                    }
                }

                swarm[i] = new Particle {
                    Position = position,
                    BestPosition = position,
                    Velocity = velocity
                };
            }

            // Iterate for the specified number of iterations
            for (int i = 0; i < Iterations; i++) {

                // Update the velocity and position of each particle
                foreach (var particle in swarm) {

                    // Update particle speed
                    for (int j = 0; j < particle.Velocity.Length; j++) {
                        particle.Velocity[j] = InertiaCoefficient * particle.Velocity[j] + CognitiveCoefficient * new Random().NextDouble() 
                        * (particle.BestPosition[j] - particle.Position[j]) + SocialCoefficient * new Random().NextDouble() * (GlobalBestPosition[j] - particle.Position[j]);
                    }

                    // Update particle position
                    for (int j = 0; j < particle.Position.Length; j++) {
                        particle.Position[j] += particle.Velocity[j];
                    }

                    // Check if the new position is better than the best known position of the particle
                    if (ObjectiveFunction(particle.Position, events) < ObjectiveFunction(particle.BestPosition, events)) {
                        particle.BestPosition = particle.Position;
                    }

                    // Check if the new position is better than the best known global position
                    if (ObjectiveFunction(particle.Position, events) < ObjectiveFunction(GlobalBestPosition, events)) {
                        GlobalBestPosition = particle.Position;
                    }
                }
            }

            // Assign events to users' schedules using the global best particle position
            for (int i = 0; i < GlobalBestPosition.Length - 1; i += 2) {
                DateTime date = DateTime.FromOADate(GlobalBestPosition[i]);
                TimeSpan time = TimeSpan.FromHours(GlobalBestPosition[i + 1]);

                Event e = events[i / 2];
                foreach (User user in e.Participants) {
                    user.Schedule.Add(new Event { Name = e.Name, Date = date, Time = time });
                }
            }
        }
    }
}