using System;
using System.Collections.Generic;
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

            // Divide the list of events into sub-lists, each containing only one event
            List<List<Event>> eventSubLists = events.Select(e => new List<Event> { e }).ToList();

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
                    subListQuality += 10000.0; // Penalize invalid dates
                }

                if (e.Participants != null) {
                    foreach (User user in e.Participants.Where(user => user != null && user.Schedule != null)) {
                        foreach (Event scheduledEvent in user.Schedule.Where(scheduledEvent => scheduledEvent != null)) {
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

        public static DateTime GetMinDate(bool isPriority) {
            return DateTime.Now.Date;
        }

        public static DateTime GetMaxDate(bool isPriority) {
            return isPriority ? DateTime.Now.Date.AddDays(2) : DateTime.Now.Date.AddDays(14);
        }

        public static double GenerateValidTime(List<Event> events, int eventIndex, Random random) {
            Event e = events[eventIndex];

            double minTime = Math.Max(e.Time.TotalHours + e.Duration.TotalHours, 8);
            double maxTime = 19;
            double time = minTime + (maxTime - minTime) * random.NextDouble();
            time = Math.Round(time / 0.25) * 0.25;

            // Continue generating times until a valid time is found
            while (HasConflict(events, eventIndex, time)) {
                time = minTime + (maxTime - minTime) * random.NextDouble();
                time = Math.Round(time / 0.25) * 0.25;
            }

            return time;
        }

        public static bool HasConflict(List<Event> events, int eventIndex, double time) {
            Event e = events[eventIndex];
            double endTime = time + e.Duration.TotalHours;

            // Check if the generated schedule conflicts with the participants' agenda
            if (e.Participants != null) {
                foreach (User participant in e.Participants) {
                    // Check conflict with events already optimized by PSO
                    for (int i = 0; i < eventIndex; i++) {
                        Event scheduledEvent = events[i];
                        double scheduledEndTime = scheduledEvent.Time.TotalHours + scheduledEvent.Duration.TotalHours;
                        if (e.Date == scheduledEvent.Date && ((time >= scheduledEvent.Time.TotalHours && time < scheduledEndTime) || (endTime > scheduledEvent.Time.TotalHours && endTime <= scheduledEndTime) || (time <= scheduledEvent.Time.TotalHours && endTime >= scheduledEndTime))) {
                            return true;
                        }
                    }

                    // Check for conflicts with events in the attendee's agenda
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