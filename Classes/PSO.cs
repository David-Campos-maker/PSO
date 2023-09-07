using System.Threading.Tasks;
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
            double[][] population = InitializePopulation(events.Count);
            double[][] velocities = InitializeVelocities(events.Count);
            double[] personalBestQuality = new double[PopulationSize];
            double[][] personalBestPosition = InitializePersonalBestPosition(events.Count);

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
                    RestartPSO(events, population, velocities, personalBestQuality, random);
                    globalBestQuality = double.MaxValue;
                    noImprovementCount = 0;
                }

                // Update inertia coefficient
                InertiaCoefficient = UpdateInertiaCoefficient(initialInertiaCoefficient, finalInertiaCoefficient, iter);

                // Select a random subset of the population
                int sampleSize = PopulationSize / 2;
                int[] sampleIndices = SelectRandomSubset(random, sampleSize);

                // Update personal best positions
                UpdatePersonalBestPositions(events, sampleIndices, population, personalBestQuality, personalBestPosition, ref globalBestQuality, ref noImprovementCount);

                // Update velocities and positions
                UpdateVelocitiesAndPositions(random, sampleIndices, population, velocities, personalBestPosition);

                noImprovementCount++;
            }

            // Update the schedule based on the global best position
            UpdateSchedule(events);
        }

        /// <summary>
        /// Initializes the population with particles containing positions for events.
        /// </summary>
        /// <param name="eventCount">The number of events for which positions are initialized.</param>
        /// <returns>A 2D matrix representing the initialized population.</returns>
        private double[][] InitializePopulation(int eventCount) {
            double[][] population = new double[PopulationSize][];
            
            for (int i = 0; i < PopulationSize; i++) {
                population[i] = new double[eventCount * 2];
            }

            return population;
        }

        /// <summary>
        /// Initializes particle velocities for events.
        /// </summary>
        /// <param name="eventCount">The number of events for which velocities are initialized.</param>
        /// <returns>A 2D matrix representing initialized speeds.</returns>
        private double[][] InitializeVelocities(int eventCount) {
            double[][] velocities = new double[PopulationSize][];

            for (int i = 0; i < PopulationSize; i++) {
                velocities[i] = new double[eventCount * 2];
            }

            return velocities;
        }

        /// <summary>
        /// Initialize personal particle positions for events.
        /// </summary>
        /// <param name="eventCount">The number of events for which personal positions are initialized.</param>
        /// <returns>A 2D matrix representing initialized personal positions.</returns>
        private double[][] InitializePersonalBestPosition(int eventCount) {
            double[][] personalBestPosition = new double[PopulationSize][];

            for (int i = 0; i < PopulationSize; i++) {
                personalBestPosition[i] = new double[eventCount * 2];
            }
            
            return personalBestPosition;
        }

        /// <summary>
        /// Resets the Particle Swarm Optimization (PSO) algorithm with a new random population.
        /// </summary>
        /// <param name="events">The list of events for which positions are generated.</param>
        /// <param name="population">The matrix representing the population of particles.</param>
        /// <param name="velocities">The matrix representing the velocities of the particles.</param>
        /// <param name="personalBestQuality">An array that stores the quality of personal best solutions for each particle.</param>
        /// <param name="random">A Random object for generating random numbers.</param>
        private void RestartPSO(List<Event> events, double[][] population, double[][] velocities, double[] personalBestQuality, Random random) {
            for (int i = 0; i < PopulationSize; i++) {
                for (int j = 0; j < events.Count; j++) {
                    Event e = events[j];

                    // Generate valid date within the specified range
                    DateTime solution_minDate = DateTime.Now.Date;
                    DateTime solution_maxDate = Helper.GetLatestPossibleDate(e);
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
        }

        /// <summary>
        /// Updates the inertia coefficient during the Particle Swarm Optimization (PSO) iterations.
        /// </summary>
        /// <param name="initialInertiaCoefficient">The initial inertia coefficient.</param>
        /// <param name="finalInertiaCoefficient">The final inertia coefficient.</param>
        /// <param name="iteration">The current iteration number.</param>
        /// <returns>The updated inertia coefficient for the given iteration.</returns>
        private double UpdateInertiaCoefficient(double initialInertiaCoefficient, double finalInertiaCoefficient, int iter) {
            return initialInertiaCoefficient - ((initialInertiaCoefficient - finalInertiaCoefficient) * iter / Iterations);
        }

        /// <summary>
        /// Selects a random subset of indices from a range.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="sampleSize">The size of the random subset to select.</param>
        /// <returns>An array of randomly selected indices.</returns>
        private int[] SelectRandomSubset(Random random, int sampleSize) {
            return Enumerable.Range(0, PopulationSize).OrderBy(x => random.Next()).Take(sampleSize).ToArray();
        }

        /// <summary>
        /// Updates the personal best positions of particles based on their current quality and the global best quality.
        /// </summary>
        /// <param name="events">The list of events for which positions are evaluated.</param>
        /// <param name="sampleIndices">An array of indices representing the subset of particles to update.</param>
        /// <param name="population">A 2D array representing the current population of particles.</param>
        /// <param name="personalBestQuality">An array storing the quality of the personal best solutions for each particle.</param>
        /// <param name="personalBestPosition">A 2D array storing the personal best positions for each particle.</param>
        /// <param name="globalBestQuality">A reference to the global best quality among all particles.</param>
        /// <param name="noImprovementCount">A reference to the count of iterations with no improvement in the global best quality.</param>
        private void UpdatePersonalBestPositions(List<Event> events, int[] sampleIndices, double[][] population, double[] personalBestQuality, double[][] personalBestPosition, ref double globalBestQuality, ref int noImprovementCount) {
            for (int i = 0; i < sampleIndices.Length; i++) {
                int index = sampleIndices[i];
                double[] position = population[index];
                double quality = Helper.FitnessFunction(position, events);

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
        }

        /// <summary>
        /// Updates the velocities and positions of particles in the Particle Swarm Optimization (PSO) algorithm.
        /// </summary>
        /// <param name="random">A random number generator for randomization.</param>
        /// <param name="sampleIndices">An array of indices representing the subset of particles to update.</param>
        /// <param name="population">A 2D array representing the current population of particles.</param>
        /// <param name="velocities">A 2D array representing the velocities of particles.</param>
        /// <param name="personalBestPosition">A 2D array storing the personal best positions of particles.</param>
        private void UpdateVelocitiesAndPositions(Random random, int[] sampleIndices, double[][] population, double[][] velocities, double[][] personalBestPosition) {
            Parallel.For(0 , sampleIndices.Length , i => {
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
            });
        }

        /// <summary>
        /// Updates the schedule with events based on the global best position in the Particle Swarm Optimization (PSO) algorithm.
        /// </summary>
        /// <param name="events">The list of events associated with the PSO optimization.</param>
        private void UpdateSchedule(List<Event> events) {
            if (GlobalBestPosition != null) {
                for (int i = 0; i < GlobalBestPosition.Length - 1; i += 2) {
                    int eventIndex = i / 2;
                    Event e = events[eventIndex];

                    DateTime eventDate = DateTime.FromOADate(GlobalBestPosition[i]);
                    TimeSpan eventTime = TimeSpan.FromHours(GlobalBestPosition[i + 1]);

                    if (e != null) {
                        if (e.Participants != null) {
                            foreach (User user in e.Participants) {
                                Helper.TryAddEventToSchedule(e.Name, eventDate, eventTime, e.Duration, user);
                            }
                        }
                    }
                }
            }
        }
    }
}