namespace PSO.Classes {
    public class PSO {
        public int PopulationSize { get; set; }
        public int Iterations { get; set; }
        public double InertiaCoefficient { get; set; }
        public double CognitiveCoefficient { get; set; }
        public double SocialCoefficient { get; set; }
        public required double[] GlobalBestPosition { get; set; }

        private bool IsEventWithinSchedule(DateTime date, TimeSpan eventStart, TimeSpan eventEnd, List<Event> schedule) {
            if (schedule == null)
                return true;

            foreach (Event scheduledEvent in schedule) {
                if (scheduledEvent == null)
                    continue;

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

            DateTime currentDate = DateTime.Now.Date;
            DateTime maxFutureDate = currentDate.AddMonths(1).Date;

            for (int i = 0; i < position.Length - 1; i += 2) {
                DateTime eventDate = DateTime.FromOADate(position[i]);
                TimeSpan time = TimeSpan.FromHours(position[i + 1]);

                Event e = events[i / 2];

                if (e == null)
                    continue;

                if (eventDate < currentDate || eventDate > maxFutureDate) {
                    quality += 1000.0; // Penalize invalid dates
                }

                if (e.Participants != null) {
                    foreach (User user in e.Participants) {
                        if (user == null || user.Schedule == null)
                            continue;

                        foreach (Event scheduledEvent in user.Schedule) {
                            if (scheduledEvent == null)
                                continue;

                            DateTime scheduledDate = scheduledEvent.Date.Date;
                            TimeSpan scheduledStartTime = scheduledEvent.Time;
                            TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                            // Check if the current event overlaps with any scheduled event
                            if (eventDate == scheduledDate && !(time >= scheduledEndTime || time + e.Duration <= scheduledStartTime)) {
                                quality += 1.0; // Apply penalty for overlapping event
                            }
                        }

                        // Check if the current event is within the user's schedule
                        if (!IsEventWithinSchedule(eventDate, time, time.Add(e.Duration), user.Schedule)) {
                            quality += 1.0; // Apply penalty for event not being within the schedule
                        }
                    }
                }
            }

            // Minimize the distance between event dates
            for (int i = 0; i < position.Length - 3; i += 2) {
                DateTime currentDate2 = DateTime.FromOADate(position[i]);
                DateTime nextDate = DateTime.FromOADate(position[i + 2]);

                double daysDifference = (nextDate - currentDate2).TotalDays;
                quality += Math.Abs(daysDifference);
            }

            return quality;
        }

        private DateTime GetMinDate(DateTime eventDate, bool isPriority) {
            DateTime currentDate = DateTime.Now.Date;
            DateTime minDate;

            if (isPriority) {
                minDate = currentDate.AddDays(2).Date;
            } else {
                minDate = currentDate.Date > eventDate.AddDays(-14).Date ? currentDate : eventDate.AddDays(-14).Date;
            }

            return minDate;
        }

        private DateTime GetMaxDate(DateTime eventDate, bool isPriority) {
            DateTime maxDate;

            if (isPriority) {
                maxDate = eventDate.AddDays(2).Date;
            } else {
                maxDate = eventDate.AddDays(14).Date;
            }

            return maxDate;
        }

        private void AddEventToSchedule(string name, DateTime date, TimeSpan time, TimeSpan duration, User user) {
            if (user == null || user.Schedule == null)
                return;

            foreach (Event scheduledEvent in user.Schedule) {
                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                if (date == scheduledDate && !(time >= scheduledEndTime || time + duration <= scheduledStartTime)) {
                    return;
                }
            }

            Event newEvent = new Event {
                Name = name,
                Date = date,
                Time = time,
                Duration = duration
            };

            user.Schedule.Add(newEvent);
        }

        public void Run(List<Event> events) {
            Random random = new Random();

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

            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;

            for (int i = 0; i < populationSize; i++) {
                population[i] = new double[events.Count * 2];
                velocities[i] = new double[events.Count * 2];
                personalBestPosition[i] = new double[events.Count * 2];

                for (int j = 0; j < events.Count; j++) {
                    Event e = events[j];
                    bool isPriority = e.Priority;

                    // Generate valid date within the specified range
                    DateTime solution_minDate = GetMinDate(e.Date, isPriority);
                    DateTime solution_maxDate = GetMaxDate(e.Date, isPriority);

                    double minValue = Math.Max(solution_minDate.ToOADate(), e.Date.AddDays(-14).ToOADate());
                    double maxValue = Math.Min(solution_maxDate.ToOADate(), e.Date.AddDays(14).ToOADate());

                    // Adjust the range based on priority
                    if (isPriority) {
                        minValue = Math.Max(minValue, e.Date.AddDays(-2).ToOADate());
                        maxValue = Math.Min(maxValue, e.Date.AddDays(2).ToOADate());
                    }

                    population[i][j * 2] = Math.Round(minValue + (maxValue - minValue) * random.NextDouble(), MidpointRounding.AwayFromZero);

                    // Generate valid time within the event's time window
                    double minTime = e.Time.TotalHours;
                    double maxTime = (e.Time + e.Duration).TotalHours;
                    population[i][j * 2 + 1] = Math.Round(minTime + (maxTime - minTime) * random.NextDouble(), MidpointRounding.AwayFromZero);

                    // Initialize velocities to zero
                    velocities[i][j * 2] = 0.0;
                    velocities[i][j * 2 + 1] = 0.0;
                }

                personalBestQuality[i] = double.MaxValue;
            }

            double[] globalBestPosition = null;
            double globalBestQuality = double.MaxValue;

            // Perform optimization iterations
            for (int iter = 0; iter < iterations; iter++) {
                // Update personal best positions
                for (int i = 0; i < populationSize; i++) {
                    double[] position = population[i];
                    double quality = ObjectiveFunction(position, events);

                    if (quality < personalBestQuality[i]) {
                        personalBestQuality[i] = quality;
                        Array.Copy(position, personalBestPosition[i], position.Length);

                        if (quality < globalBestQuality) {
                            globalBestQuality = quality;
                            globalBestPosition = position;
                        }
                    }
                }

                // Update velocities and positions
                for (int i = 0; i < populationSize; i++) {
                    double[] position = population[i];
                    double[] velocity = velocities[i];
                    double[] personalBestPos = personalBestPosition[i];

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
                                AddEventToSchedule(e.Name, eventDate, eventTime, e.Duration, user);
                            }
                        }
                    }
                }
            }
        }
    }
}