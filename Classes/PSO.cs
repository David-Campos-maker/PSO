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

            // Divide the list of events into sub-lists, each containing only one event
            List<List<Event>> eventSubLists = new List<List<Event>>();
            for (int i = 0; i < events.Count; i++) {
                eventSubLists.Add(new List<Event> { events[i] });
            }

            // Calculate the quality of each sub-list
            for (int i = 0; i < eventSubLists.Count; i++) {
                List<Event> eventSubList = eventSubLists[i];
                Event e = eventSubList[0];

                if (e == null) continue;

                // Calculate the quality of the current sub-list
                double subListQuality = 0.0;
                DateTime eventDate = DateTime.FromOADate(position[i * 2]);
                TimeSpan time = TimeSpan.FromHours(position[i * 2 + 1]);

                if (eventDate < currentDate || eventDate > maxFutureDate) {
                    subListQuality += 1000.0; // Penalize invalid dates
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
                                subListQuality += 1.0; // Apply penalty for overlapping event
                            }
                        }

                        // Check if the current event is within the user's schedule
                        if (!IsEventWithinSchedule(eventDate, time, time.Add(e.Duration), user.Schedule)) {
                            subListQuality += 1.0; // Apply penalty for event not being within the schedule
                        }
                    }
                }

                // Minimize the distance between event dates
                for (int j = 0; j < position.Length - 3; j += 2) {
                    DateTime currentDate2 = DateTime.FromOADate(position[j]);
                    DateTime nextDate = DateTime.FromOADate(position[j + 2]);

                    double daysDifference = (nextDate - currentDate2).TotalDays;
                    subListQuality += Math.Abs(daysDifference);
                }

                // Add the quality of the current sub-list to the total quality
                quality += subListQuality;
            }

            return quality;
        }

        private DateTime GetMinDate(DateTime eventDate, bool isPriority) {
            DateTime currentDate = DateTime.Now.Date;
            DateTime minDate;

            if (isPriority) {
                minDate = currentDate;
            } else {
                minDate = currentDate;
            }

            return minDate;
        }

        private DateTime GetMaxDate(DateTime eventDate, bool isPriority) {
            DateTime currentDate = DateTime.Now.Date;
            DateTime maxDate;

            if (isPriority) {
                maxDate = currentDate.AddDays(2);
            } else {
                maxDate = currentDate.AddDays(14);
            }

            return maxDate;
        }

        public double GenerateValidTime(List<Event> events, int eventIndex, Random random) {
            Event e = events[eventIndex];

            double minTime = Math.Max(e.Time.TotalHours + e.Duration.TotalHours, 8);
            double maxTime = 19;
            double time = minTime + (maxTime - minTime) * random.NextDouble();
            time = Math.Round(time / 0.25) * 0.25;
            
            // Continuar gerando horários até encontrar um horário válido
            while (HasConflict(events, eventIndex, time)) {
                time = minTime + (maxTime - minTime) * random.NextDouble();
                time = Math.Round(time / 0.25) * 0.25;
            }
            
            return time;
        }

        public bool HasConflict(List<Event> events, int eventIndex, double time) {
            Event e = events[eventIndex];
            double endTime = time + e.Duration.TotalHours;
            
            // Verificar se o horário gerado conflita com a agenda dos participantes
            if (e.Participants != null) {
                foreach (User participant in e.Participants) {
                    // Verificar conflito com eventos já otimizados pelo PSO
                    for (int i = 0; i < eventIndex; i++) {
                        Event scheduledEvent = events[i];
                        double scheduledEndTime = scheduledEvent.Time.TotalHours + scheduledEvent.Duration.TotalHours;
                        if (e.Date == scheduledEvent.Date && ((time >= scheduledEvent.Time.TotalHours && time < scheduledEndTime) || (endTime > scheduledEvent.Time.TotalHours && endTime <= scheduledEndTime) || (time <= scheduledEvent.Time.TotalHours && endTime >= scheduledEndTime))) {
                            return true;
                        }
                    }
                    
                    // Verificar conflito com eventos na agenda do participante
                    if (participant.Schedule != null) {
                        foreach (Event scheduledEvent in participant.Schedule) {
                            double scheduledEndTime = scheduledEvent.Time.TotalHours + scheduledEvent.Duration.TotalHours;
                            if (e.Date == scheduledEvent.Date && ((time >= scheduledEvent.Time.TotalHours && time < scheduledEndTime) || (endTime > scheduledEvent.Time.TotalHours && endTime <= scheduledEndTime) || (time <= scheduledEvent.Time.TotalHours && endTime >= scheduledEndTime))) {
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
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
                    double minValue = solution_minDate.ToOADate();
                    double maxValue = solution_maxDate.ToOADate();

                    population[i][j * 2] = Math.Round(minValue + (maxValue - minValue) * random.NextDouble(), MidpointRounding.AwayFromZero);

                    // Generate valid time within the event's time window
                    population[i][j * 2 + 1] = GenerateValidTime(events, j, random);

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