using System;
using System.Collections.Generic;
using System.Linq;
using PSO.Helpers;

namespace PSO.Classes {
    public class PSO {
        public int PopulationSize { get; set; }
        public int Iterations { get; set; }
        public double InertiaCoefficient { get; set; }
        public double CognitiveCoefficient { get; set; }
        public double SocialCoefficient { get; set; }
        public required double[] GlobalBestPosition { get; set; }

        public void Run(List<Event> events) {
            Random random = new();

            int populationSize = PopulationSize;
            int iterations = Iterations;
            double inertiaCoefficient = InertiaCoefficient;
            double cognitiveCoefficient = CognitiveCoefficient;
            double socialCoefficient = SocialCoefficient;

            // Initialize population
            double[][] population = new double[populationSize][];
            double[][] velocities = new double[populationSize][];
            double[] personalBestQuality = new double[populationSize];
            double[][] personalBestPosition = new double[populationSize][];

            for (int i = 0; i < populationSize; i++) {
                population[i] = new double[events.Count * 2];
                velocities[i] = new double[events.Count * 2];
                personalBestPosition[i] = new double[events.Count * 2];

                for (int j = 0; j < events.Count; j++) {
                    Event e = events[j];
                    bool isPriority = e.Priority;

                    // Generate valid date within the specified range
                    DateTime solution_minDate = DateTime.Now.Date;
                    DateTime solution_maxDate = Helper.GetMaxDate(isPriority);
                    double minValue = solution_minDate.ToOADate();
                    double maxValue = solution_maxDate.ToOADate();

                    population[i][j * 2] = Math.Round(minValue + (maxValue - minValue) * random.NextDouble(), MidpointRounding.AwayFromZero);

                    // Generate valid time within the event's time window
                    population[i][j * 2 + 1] = Helper.GenerateValidTime(events, j, random);

                    // Initialize velocities to zero
                    velocities[i][j * 2] = 0.0;
                    velocities[i][j * 2 + 1] = 0.0;
                }

                personalBestQuality[i] = double.MaxValue;
            }

            double[] globalBestPosition = new double[events.Count * 2];
            double globalBestQuality = double.MaxValue;

            int noImprovementCount = 0;
            int noImprovementLimit = 10;

            double initialInertiaCoefficient = InertiaCoefficient;
            double finalInertiaCoefficient = 0.4;

            // Perform optimization iterations
            for (int iter = 0; iter < iterations; iter++) {
                // Check if the PSO is stuck in a local optimum
                if (noImprovementCount >= noImprovementLimit) {
                    // Restart PSO with a new random population
                    for (int i = 0; i < populationSize; i++) {
                        for (int j = 0; j < events.Count; j++) {
                            Event e = events[j];
                            bool isPriority = e.Priority;

                            // Generate valid date within the specified range
                            DateTime solution_minDate = DateTime.Now.Date;
                            DateTime solution_maxDate = Helper.GetMaxDate(isPriority);
                            double minValue = solution_minDate.ToOADate();
                            double maxValue = solution_maxDate.ToOADate();

                            population[i][j * 2] = Math.Round(minValue + (maxValue - minValue) * random.NextDouble(), MidpointRounding.AwayFromZero);

                            // Generate valid time within the event's time window
                            population[i][j * 2 + 1] = Helper.GenerateValidTime(events, j, random);

                            // Initialize velocities to zero
                            velocities[i][j * 2] = 0.0;
                            velocities[i][j * 2 + 1] = 0.0;
                        }

                        personalBestQuality[i] = double.MaxValue;
                    }

                    globalBestPosition = new double[events.Count * 2];
                    globalBestQuality = double.MaxValue;

                    noImprovementCount = 0;
                }

                // Update inertia coefficient
                inertiaCoefficient = initialInertiaCoefficient - ((initialInertiaCoefficient - finalInertiaCoefficient) * iter / iterations);

                // Select a random subset of the population
                int sampleSize = populationSize / 2;
                int[] sampleIndices = Enumerable.Range(0, populationSize).OrderBy(x => random.Next()).Take(sampleSize).ToArray();

                // Update personal best positions
                for (int i = 0; i < sampleSize; i++) {
                    int index = sampleIndices[i];
                    double[] position = population[index];
                    double quality = Helper.ObjectiveFunction(position, events);

                    if (quality < personalBestQuality[index]) {
                        personalBestQuality[index] = quality;
                        Array.Copy(position, personalBestPosition[index], position.Length);

                        if (quality < globalBestQuality) {
                            globalBestQuality = quality;
                            globalBestPosition = position;

                            noImprovementCount = 0;
                        }
                    }
                }

                // Update velocities and positions
                for (int i = 0; i < sampleSize; i++) {
                    int index = sampleIndices[i];
                    double[] position = population[index];
                    double[] velocity = velocities[index];
                    double[] personalBestPos = personalBestPosition[index];

                    for (int j = 0; j < position.Length; j++) {
                        double rp = random.NextDouble();
                        double rg = random.NextDouble();

                        if (globalBestPosition != null) {
                            velocity[j] = inertiaCoefficient * velocity[j]
                            + cognitiveCoefficient * rp * (personalBestPos[j] - position[j])
                            + socialCoefficient * rg * (globalBestPosition[j] - position[j]);

                            position[j] += velocity[j];
                        }
                    }
                }

                noImprovementCount++;
            }

            // Update the schedule based on the global best position
            if (globalBestPosition != null) {
                for (int i = 0; i < globalBestPosition.Length - 1; i += 2) {
                    int eventIndex = i / 2;
                    Event e = events[eventIndex];

                    DateTime eventDate = DateTime.FromOADate(globalBestPosition[i]);
                    TimeSpan eventTime = TimeSpan.FromHours(globalBestPosition[i + 1]);

                    if (e != null) {
                        if (e.Participants != null) {
                            foreach (User user in e.Participants) {
                                Helper.AddEventToSchedule(e.Name, eventDate, eventTime, e.Duration, user);
                            }
                        }
                    }
                }
            }
        }
    }
}