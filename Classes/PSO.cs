namespace PSO.Classes
{
    public class PSO
    {
        public int PopulationSize { get; set; }
        public int Iterations { get; set; }
        public double InertiaCoefficient { get; set; }
        public double CognitiveCoefficient { get; set; }
        public double SocialCoefficient { get; set; }
        public required double[] GlobalBestPosition { get; set; }

        private bool IsEventWithinSchedule(DateTime date, TimeSpan eventStart, TimeSpan eventEnd, List<Event> schedule) {
            foreach (Event scheduledEvent in schedule) {
                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                if (scheduledDate == date && !(eventEnd <= scheduledStartTime || eventStart >= scheduledEndTime)) {
                    return false;
                }
            }

            return true;
        }

        private double ObjectiveFunction(double[] position, List<Event> events) {
            double quality = 0.0;

            for (int i = 0; i < position.Length - 1; i += 2) {
                DateTime date = DateTime.FromOADate(position[i]);
                TimeSpan time = TimeSpan.FromHours(position[i + 1]);

                Event e = events[i / 2];

                if (e.Participants != null) {
                    foreach (User user in e.Participants) {
                        if (user.Schedule != null) {
                            foreach (Event scheduledEvent in user.Schedule) {
                                DateTime scheduledDate = scheduledEvent.Date.Date;
                                TimeSpan scheduledStartTime = scheduledEvent.Time;
                                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                                // Check if the current event overlaps with any scheduled event
                                if (date == scheduledDate && !(time >= scheduledEndTime || time + e.Duration <= scheduledStartTime)) {
                                    quality += 1.0; // Apply penalty for overlapping event
                                }
                            }

                            // Check if the current event is within the user's schedule
                            if (!IsEventWithinSchedule(date, time, time.Add(e.Duration), user.Schedule)) {
                                quality += 1.0; // Apply penalty for event not being within the schedule
                            }
                        }
                    }
                }
            }

            // Minimize the distance between event dates
            for (int i = 0; i < position.Length - 3; i += 2) {
                DateTime currentDate = DateTime.FromOADate(position[i]);
                DateTime nextDate = DateTime.FromOADate(position[i + 2]);

                double daysDifference = (nextDate - currentDate).TotalDays;
                quality += Math.Abs(daysDifference);
            }

            return quality;
        }

        private void AddEventToSchedule(Event newEvent, User user) {
            if (newEvent == null || user == null || user.Schedule == null)
                return;

            foreach (Event scheduledEvent in user.Schedule) {
                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                if (newEvent.Date == scheduledDate && !(newEvent.Time >= scheduledEndTime || newEvent.Time + newEvent.Duration <= scheduledStartTime)) {
                    return;
                }
            }

            user.Schedule.Add(newEvent);
        }

        public void Run(List<Event> events) {
            var swarm = new Particle[PopulationSize];
            var random = new Random();
            int dimensions = events.Count * 2; // Two dimensions (date and time) for each event

            DateTime minDate = DateTime.Now.Date;
            DateTime maxDate = DateTime.Now.AddYears(1).Date;

            TimeSpan minTime = new TimeSpan(7, 0, 0);
            TimeSpan maxTime = new TimeSpan(18, 0, 0);

            for (int i = 0; i < PopulationSize; i++) {
                var position = new double[dimensions];
                var velocity = new double[dimensions];

                // Initialize particle position and velocity with random values
                for (int j = 0; j < position.Length; j++) {
                    if (j % 2 == 0) {
                        // Initialize the event date
                        double minValue = minDate.ToOADate();
                        double maxValue = maxDate.ToOADate();

                        position[j] = minValue + (maxValue - minValue) * random.NextDouble();
                        velocity[j] = minValue + (maxValue - minValue) * random.NextDouble();
                    } else {
                        // Initialize the event time
                        double minValue = minTime.TotalHours;
                        double maxValue = maxTime.TotalHours;

                        position[j] = minValue + (maxValue - minValue) * random.NextDouble();
                        velocity[j] = minValue + (maxValue - minValue) * random.NextDouble();
                    }
                }

                var clonedPosition = position.Clone() as double[];
                if (clonedPosition == null) {
                    swarm[i] = new Particle {
                        Position = position,
                        Velocity = velocity,
                        BestPosition = new double[position.Length] // assign a default value
                    };
                } else {
                    swarm[i] = new Particle {
                        Position = position,
                        Velocity = velocity,
                        BestPosition = clonedPosition
                    };
                }
            }

            double[] globalBestPosition = (swarm[0].BestPosition?.Clone() as double[]) ?? new double[0];
            double globalBestFitness = ObjectiveFunction(globalBestPosition, events);

            for (int iteration = 0; iteration < Iterations; iteration++) {
                for (int i = 0; i < PopulationSize; i++) {
                    Particle particle = swarm[i];

                    for (int j = 0; j < dimensions; j++) {
                        if (globalBestPosition != null) {
                            // Update velocity
                            particle.Velocity[j] = InertiaCoefficient * particle.Velocity[j] +
                                CognitiveCoefficient * random.NextDouble() * (particle.BestPosition[j] - particle.Position[j]) +
                                SocialCoefficient * random.NextDouble() * (globalBestPosition[j] - particle.Position[j]);

                            // Update position
                            particle.Position[j] += particle.Velocity[j];

                            // Clamp position to the valid range
                            if (j % 2 == 0) {
                                particle.Position[j] = Math.Max(minDate.ToOADate(), Math.Min(maxDate.ToOADate(), particle.Position[j]));
                            } else {
                                particle.Position[j] = Math.Max(minTime.TotalHours, Math.Min(maxTime.TotalHours, particle.Position[j]));
                            }
                        }
                    }

                    double fitness = ObjectiveFunction(particle.Position, events);

                    if (particle.Position != null) {
                        double[] clonedPosition = (double[])particle.Position.Clone();

                        // Update personal best position
                        if (fitness < ObjectiveFunction(particle.BestPosition, events)) {
                            particle.BestPosition = clonedPosition;
                        }

                        // Update global best position
                        if (fitness < globalBestFitness) {
                            globalBestPosition = clonedPosition;
                            globalBestFitness = fitness;
                        }
                    }
                }
            }

            double[] localBestPosition = globalBestPosition ?? Array.Empty<double>();
            GlobalBestPosition = localBestPosition;

            if (GlobalBestPosition != null) {
                // Add the event to the participants' calendar
                for (int i = 0; i < GlobalBestPosition.Length - 1; i += 2) {
                    DateTime date = DateTime.FromOADate(GlobalBestPosition[i]);
                    TimeSpan time = TimeSpan.FromHours(GlobalBestPosition[i + 1]);

                    Event e = events[i / 2];

                    if (e.Participants != null) {
                        foreach (User user in e.Participants) {
                            AddEventToSchedule(new Event { Name = e.Name, Date = date, Time = time, Duration = e.Duration }, user);
                        }
                    }
                }
            }
        }
    }
}