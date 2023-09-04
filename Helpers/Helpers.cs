using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using PSO.Classes;

namespace PSO.Helpers {
     public static class Helper {
        public static bool IsEventWithinSchedule(DateTime date, TimeSpan eventStart, TimeSpan eventEnd, List<Event> schedule) {
            if (schedule == null)
                return true;

            return !schedule.Any(scheduledEvent => {
                if (scheduledEvent == null)
                    return false;

                DateTime scheduledDate = scheduledEvent.Date.Date;
                TimeSpan scheduledStartTime = scheduledEvent.Time;
                TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                return scheduledDate == date && !(eventEnd <= scheduledStartTime || eventStart >= scheduledEndTime);
            });
        }

        public static double ObjectiveFunction(double[] position, List<Event> events) {
            double quality = 0.0;

            DateTime currentDate = DateTime.Now.Date;
            DateTime maxFutureDate = currentDate.AddMonths(1).Date;

            // Calculate the quality of each event
            for (int i = 0; i < events.Count; i++) {
                Event e = events[i];

                if (e == null) continue;

                // Calculate the quality of the current event
                double eventQuality = 0.0;
                DateTime eventDate = DateTime.FromOADate(position[i * 2]);
                TimeSpan time = TimeSpan.FromHours(position[i * 2 + 1]);

                if (eventDate < currentDate) {
                    eventQuality += 10000.0; // Penalize invalid dates
                }

                if (e.Participants != null) {
                    foreach (User user in e.Participants.Where(user => user != null && user.Schedule != null)) {
                        // Check if the current event overlaps with any scheduled event
                        foreach (Event scheduledEvent in user.Schedule) {
                            DateTime scheduledDate = scheduledEvent.Date.Date;
                            TimeSpan scheduledStartTime = scheduledEvent.Time;
                            TimeSpan scheduledEndTime = scheduledStartTime.Add(scheduledEvent.Duration);

                            if (eventDate == scheduledDate && !(time >= scheduledEndTime || time + e.Duration <= scheduledStartTime)) {
                                eventQuality += 10000.0; // Apply penalty for overlapping event
                            }
                        }

                        // Check if the current event is within the user's schedule
                        if (!IsEventWithinSchedule(eventDate, time, time.Add(e.Duration), user.Schedule)) {
                            eventQuality += 10000.0; // Apply a large penalty for event not being within the schedule
                        }

                        // Check if the current event fits within the user's work day
                        TimeSpan workDayStart = new(7, 0, 0);
                        TimeSpan workDayEnd = new(18, 0, 0);
                        if (time + e.Duration > workDayEnd || time + e.Duration < workDayStart) {
                            eventQuality += 10000.0; // Apply a large penalty for event not fitting within the user's work day
                        }
                    }
                }

                // Add the quality of the current event to the total quality
                quality += eventQuality;
            }

            return quality;
        }

        public static DateTime GetMaxDate(Event e) {
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

        public static double GenerateValidTime(List<Event> events, int eventIndex, Random random) {
            Event e = events[eventIndex];
            double minTime = Math.Max(e.Time.TotalHours + e.Duration.TotalHours, 7);
            double maxTime = 18;
            double time = GenerateRandomTime(minTime, maxTime, random);

            return time;
        }

        private static double GenerateRandomTime(double minTime, double maxTime, Random random) {
            double time = minTime + (maxTime - minTime) * random.NextDouble();
            return Math.Round(time / 0.25) * 0.25;
        }

        public static void AddEventToSchedule(string name, DateTime date, TimeSpan time, TimeSpan duration, User user) {
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