using System;
using System.Collections.Generic;
using System.Linq;

public class PSO {
    public int PopulationSize {get; set;}
    public int Iterations {get; set;}
    public double InertiaCoefficient {get; set;}
    public double CognitiveCoefficient {get; set;}
    public double SocialCoefficient {get; set;}
    public double[] GlobalBestPosition {get; set;}

    private double ObjectiveFunction(double[] position, List<Event> events) {
        double quality = 0;

        for (int i = 0; i < position.Length; i++) {
            DateTime date = DateTime.FromOADate(position[i]);
            TimeSpan time = TimeSpan.FromHours(position[i + 1]);

            foreach (Event e in events) {
                foreach (User user in e.Participants) {
                    if (user.Schedule.Any(e => e.Date == date && e.Time == time)) {
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

        for (int i = 0; i < PopulationSize; i++) {
            swarm[i] = new Particle {
                Position = new double[] {},
                BestPosition = new double[] {},
                Velocity = new double[] {}
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
        for (int i = 0; i < GlobalBestPosition.Length; i++) {
            DateTime date = DateTime.FromOADate(GlobalBestPosition[i]);
            TimeSpan time = TimeSpan.FromHours(GlobalBestPosition[i + 1]);

            Event e = events[i];
            foreach (User user in e.Participants) {
                user.Schedule.Add(new Event { Name = e.Name, Date = date, Time = time });
            }
        }
    }
}