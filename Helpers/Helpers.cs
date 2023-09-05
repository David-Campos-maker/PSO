using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using PSO.Classes;

namespace PSO.Helpers {
     public static class Helper {
        private static bool CanEventFitInDay(Event newEvent, List<Event> scheduledEvents) {
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
            if (newEvent.Time >= scheduledEvents[scheduledEvents.Count - 1].Time + scheduledEvents[scheduledEvents.Count - 1].Duration) {
                return true;
            }

            // The new event does not fit on the day
            return false;
        }

        public static double ObjectiveFunction(double[] position, List<Event> events) {
            double quality = 0.0;

            // Calculate the quality of each event
            for (int i = 0; i < events.Count; i++) {
                Event e = events[i];

                if (e == null) continue;

                // Calculate the quality of the current event
                double eventQuality = 0.0;
                DateTime eventDate = DateTime.FromOADate(position[i * 2]);
                TimeSpan time = TimeSpan.FromHours(position[i * 2 + 1]);

                // Check if the current event overlaps with any other event
                for (int j = 0; j < events.Count; j++) {
                    if (i == j) continue; // Skip the current event

                    Event otherEvent = events[j];
                    if (otherEvent == null) continue;

                    DateTime otherEventDate = DateTime.FromOADate(position[j * 2]);
                    TimeSpan otherEventStartTime = TimeSpan.FromHours(position[j * 2 + 1]);
                    TimeSpan otherEventEndTime = otherEventStartTime.Add(otherEvent.Duration);

                    if (eventDate == otherEventDate && !(time >= otherEventEndTime || time + e.Duration <= otherEventStartTime)) {
                        eventQuality += 10000.0; // Apply penalty for overlapping event
                    }
                }

                // Check if the current event fits within the day
                List<Event> eventsOnSameDay = events.Where(ev => ev != null && DateTime.FromOADate(position[events.IndexOf(ev) * 2]) == eventDate).ToList();
                if (!CanEventFitInDay(e, eventsOnSameDay)) {
                    eventQuality += 10000.0; // Apply penalty for event not fitting within the day
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