using PSO.Classes;

namespace PSO.Helpers {
     public static class Helper {
        /// <summary>
        /// Checks if a new event can be scheduled within the day bounds without overlapping existing events.
        /// </summary>
        /// <param name="newEvent">The new event to be scheduled.</param>
        /// <param name="scheduledEvents">The list of already scheduled events for the day.</param>
        /// <returns>True if the new event can be scheduled without overlap; otherwise, false.</returns>
        private static bool IsEventWithinDayBounds(Event newEvent, List<Event> scheduledEvents) {
            // Sort scheduled events by start time
            scheduledEvents.Sort((e1, e2) => e1.Time.CompareTo(e2.Time));

            // Check if the new event fits before the first scheduled event
            if (newEvent.Time + newEvent.Duration <= scheduledEvents[0].Time) {
                return true;
            }

            // Check if the new event fits between two scheduled events
            for (int i = 0; i < scheduledEvents.Count - 1; i++) {
                TimeSpan startTime = scheduledEvents[i].Time + scheduledEvents[i].Duration;
                TimeSpan endTime = scheduledEvents[i + 1].Time;

                if (newEvent.Time >= startTime && newEvent.Time + newEvent.Duration <= endTime) {
                    return true;
                }
            }

            // Check if the new event fits after the last scheduled event
            if (newEvent.Time >= scheduledEvents[^1].Time + scheduledEvents[scheduledEvents.Count - 1].Duration) {
                return true;
            }

            // The new event does not fit on the day
            return false;
        }

        /// <summary>
        /// Checks if an event overlaps with any scheduled events on a specific date and time.
        /// </summary>
        /// <param name="date">The date of the event to check.</param>
        /// <param name="eventStart">The start time of the event to check.</param>
        /// <param name="eventEnd">The end time of the event to check.</param>
        /// <param name="schedule">The list of scheduled events to compare against, or null if no events are scheduled.</param>
        /// <returns>True if the event overlaps with any scheduled events; otherwise, false.</returns>
        private static bool DoesEventOverlapWithSchedule(DateTime date, TimeSpan eventStart, TimeSpan eventEnd, List<Event> schedule) {
            // If the schedule is null, the event is on schedule
            if (schedule == null)
                return true;

            // Check if the event overlaps with any scheduled events
            return !schedule.Any(scheduledEvent => {
                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                return scheduledDate == date && !(eventEnd <= scheduledStartTime || eventStart >= scheduledEndTime);
            });
        }

        /// <summary>
        /// Calculates the fitness quality of a particle's position in the context of scheduling events.
        /// </summary>
        /// <param name="position">The position of the particle, representing event scheduling data.</param>
        /// <param name="events">The list of events to be scheduled.</param>
        /// <returns>The fitness quality value for the given particle position.</returns>
        public static double FitnessFunction(double[] position, List<Event> events) {
            double quality = 0.0;

            // Calculate the quality of each event
            Parallel.For(0 , events.Count , i => {
                Event e = events[i];

                if (e != null) {
                    // Calculate the quality of the current event
                    double eventQuality = 0.0;
                    DateTime eventDate = DateTime.FromOADate(position[i * 2]);
                    TimeSpan time = TimeSpan.FromHours(position[i * 2 + 1]);

                    // Check if the current event is within the user's schedule
                    if (e.Participants != null) {
                        foreach (User user in e.Participants.Where(user => user != null && user.Schedule != null)) {
                            if (!DoesEventOverlapWithSchedule(eventDate, time, time.Add(e.Duration), user.Schedule)) {
                                eventQuality += 10000.0; // Apply a large penalty for event not being within the schedule
                            }
                        }
                    }

                    // Check if the current event fits within the day
                    List<Event> eventsOnSameDay = events.Where(ev => ev != null && DateTime.FromOADate(position[events.IndexOf(ev) * 2]) == eventDate).ToList();
                    if (!IsEventWithinDayBounds(e, eventsOnSameDay)) {
                        eventQuality += 10000.0; // Apply penalty for event not fitting within the day
                    }

                    // Add the quality of the current event to the total quality
                    quality += eventQuality;
                }
            });

            return quality;
        }

        /// <summary>
        /// Calculates and returns the latest possible date for scheduling an event, considering event priority and participant schedules.
        /// </summary>
        /// <param name="e">The event for which the latest possible date is calculated.</param>
        /// <returns>The latest possible date for scheduling the event.</returns>
        public static DateTime GetLatestPossibleDate(Event e) {
            if (e.Priority) {
                bool isFullDay = false; 

                foreach (User participant in e.Participants.Where(participant => participant != null)) {
                    TimeSpan totalHoras = TimeSpan.Zero;

                    foreach (Event scheduledEvent in participant.Schedule.Where(scheduledEvent => scheduledEvent != null)) {
                        totalHoras += scheduledEvent.Duration; 
                    }

                    if (totalHoras.TotalHours >= 8 && totalHoras.TotalHours <= 12) {
                        isFullDay = true; 
                        break;
                    }
                }

                if (isFullDay) {
                    return DateTime.Now.AddDays(4); 
                } else {
                    return DateTime.Now.AddDays(2); 
                }
            } else {
                return DateTime.Now.AddDays(21); 
            }
        }

        /// <summary>
        /// Generates a valid time for scheduling an event, ensuring it does not overlap with existing events.
        /// </summary>
        /// <param name="events">The list of events, including the event to be scheduled.</param>
        /// <param name="eventIndex">The index of the event to be scheduled within the list.</param>
        /// <param name="random">A random number generator for generating the valid time.</param>
        /// <returns>A valid time for scheduling the event that does not overlap with existing events.</returns>
        public static double GenerateValidTime(List<Event> events, int eventIndex, Random random) {
            Event e = events[eventIndex];
            double earliestPossibleTime = Math.Max(e.Time.TotalHours + e.Duration.TotalHours, 7);
            double latestPossibleTime = 18;
            double validTime = GenerateRandomTime(earliestPossibleTime, latestPossibleTime, random);

            return validTime;
        }

        /// <summary>
        /// Generates a random time within a specified time range, rounded to the nearest quarter-hour.
        /// </summary>
        /// <param name="minTime">The minimum allowable time (inclusive).</param>
        /// <param name="maxTime">The maximum allowable time (exclusive).</param>
        /// <param name="random">A random number generator for generating the random time.</param>
        /// <returns>A random time within the specified range, rounded to the nearest quarter-hour.</returns>
        private static double GenerateRandomTime(double minTime, double maxTime, Random random) {
            double time = minTime + (maxTime - minTime) * random.NextDouble();
            return Math.Round(time / 0.25) * 0.25;
        }

        /// <summary>
        /// Attempts to add a new event to a user's schedule, checking for schedule conflicts.
        /// </summary>
        /// <param name="name">The name of the event to be added.</param>
        /// <param name="date">The date of the event to be added.</param>
        /// <param name="time">The start time of the event to be added.</param>
        /// <param name="duration">The duration of the event to be added.</param>
        /// <param name="user">The user for whom the event is scheduled.</param>
        public static void TryAddEventToSchedule(string name, DateTime date, TimeSpan time, TimeSpan duration, User user) {
            if (user == null || user.Schedule == null)
                return;

            bool hasConflict = user.Schedule.Any(scheduledEvent => {
                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                return date == scheduledDate && !(time >= scheduledEndTime || time + duration <= scheduledStartTime);
            });

            if (hasConflict) {
                return;
            }

            Event newEvent = new() {
                Name = name,
                Date = date,
                Time = time,
                Duration = duration
            };

            user.Schedule.Add(newEvent);
        }
    }
}