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
        public double[] GlobalBestPosition { get; set; } //required

        public PSO (int populationSize , int iterations , double inertiaCoefficient , double cognitiveCoefficient , double socialCoefficient , int listLength) {
            PopulationSize = populationSize;
            Iterations = iterations;
            InertiaCoefficient = inertiaCoefficient;
            CognitiveCoefficient = cognitiveCoefficient;
            SocialCoefficient = socialCoefficient;
            GlobalBestPosition = new double[listLength * 2];
        }

        public void Run(List<Event> events) {
            Random random = new();

            // Initialize population
            double[][] population = new double[PopulationSize][];
            double[][] velocities = new double[PopulationSize][];
            double[] personalBestQuality = new double[PopulationSize];
            double[][] personalBestPosition = new double[PopulationSize][];

            for (int i = 0; i < PopulationSize; i++) {
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

            double globalBestQuality = double.MaxValue;

            int noImprovementCount = 0;
            int noImprovementLimit = 10;

            double initialInertiaCoefficient = InertiaCoefficient;
            double finalInertiaCoefficient = 0.4;

            // Perform optimization iterations
            for (int iter = 0; iter < Iterations; iter++) {
                // Check if the PSO is stuck in a local optimum
                if (noImprovementCount >= noImprovementLimit) {
                    // Restart PSO with a new random population
                    for (int i = 0; i < PopulationSize; i++) {
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

                    globalBestQuality = double.MaxValue;

                    noImprovementCount = 0;
                }

                // Update inertia coefficient
                InertiaCoefficient = initialInertiaCoefficient - ((initialInertiaCoefficient - finalInertiaCoefficient) * iter / Iterations);

                // Select a random subset of the population
                int sampleSize = PopulationSize / 2;
                int[] sampleIndices = Enumerable.Range(0, PopulationSize).OrderBy(x => random.Next()).Take(sampleSize).ToArray();

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
                            GlobalBestPosition = position;

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

                        if (GlobalBestPosition != null) {
                            velocity[j] = InertiaCoefficient * velocity[j]
                            + CognitiveCoefficient * rp * (personalBestPos[j] - position[j])
                            + SocialCoefficient * rg * (GlobalBestPosition[j] - position[j]);

                            position[j] += velocity[j];
                        }
                    }
                }

                noImprovementCount++;
            }

            // Update the schedule based on the global best position
            if (GlobalBestPosition != null) {
                for (int i = 0; i < GlobalBestPosition.Length - 1; i += 2) {
                    int eventIndex = i / 2;
                    Event e = events[eventIndex];

                    DateTime eventDate = DateTime.FromOADate(GlobalBestPosition[i]);
                    TimeSpan eventTime = TimeSpan.FromHours(GlobalBestPosition[i + 1]);

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