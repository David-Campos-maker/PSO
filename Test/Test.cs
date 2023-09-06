using PSO.Classes;
using PSOAlias = PSO.Classes;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

public class Test {
    public static void Main() {
        List<User> users = new List<User>();
        List<Event> events = new List<Event>();

        string connectionString = "Data Source=NITRO5DAVID;Initial Catalog=PSO_testing;Integrated Security=true;User ID=Tester;Password=tester";
        SqlConnection connection = new SqlConnection(connectionString);

        connection.Open();

        string getUsersQuery = "SELECT DISTINCT Users.User_ID, Users.User_Name FROM Schedule JOIN Users ON Schedule.Owner = Users.User_ID;";
        using (SqlCommand query1 = new SqlCommand(getUsersQuery , connection))
        using (SqlDataReader usersReader = query1.ExecuteReader()) {
            while (usersReader.Read()) {
                User user = new();
                int userID = usersReader.GetInt32(usersReader.GetOrdinal("User_ID"));
                string userName = usersReader.GetString(usersReader.GetOrdinal("User_Name"));

                user.ID = userID;
                user.Name = userName;
                user.Schedule = new List<Event>();

                users.Add(user);
            }
        }

        string getUserScheduleQuery = "SELECT Events.* FROM Schedule JOIN Events ON Schedule.Scheduled_Event = Events.Event_ID WHERE Schedule.Owner = @UserID;";
        using (SqlCommand query2 = new SqlCommand(getUserScheduleQuery , connection)) {
            query2.Parameters.Add(new SqlParameter("UserID" , SqlDbType.Int));

            foreach (User user in users) {
                query2.Parameters["UserID"].Value = user.ID;

                using SqlDataReader scheduleReader = query2.ExecuteReader();
                while (scheduleReader.Read()) {
                    int scheduledEventID = scheduleReader.GetInt32(scheduleReader.GetOrdinal("Event_ID"));
                    string scheduledEventName = scheduleReader.GetString(scheduleReader.GetOrdinal("Event_Name"));
                    DateTime scheduledEventDate = scheduleReader.GetDateTime(scheduleReader.GetOrdinal("Event_Date"));
                    TimeSpan scheduledEventTime = scheduleReader.GetTimeSpan(scheduleReader.GetOrdinal("Event_Time"));
                    TimeSpan scheduleEventDuration = scheduleReader.GetTimeSpan(scheduleReader.GetOrdinal("Duration"));
                    bool scheduledEventPriority = scheduleReader.GetBoolean(scheduleReader.GetOrdinal("Priority"));

                    Event e = new();
                    e.ID = scheduledEventID;
                    e.Name = scheduledEventName;
                    e.Date = scheduledEventDate;
                    e.Time = scheduledEventTime;
                    e.Duration = scheduleEventDuration;
                    e.Priority = scheduledEventPriority;

                    user.Schedule.Add(e);
                }
            }
        }

        string getEventsQuery = "SELECT Events.* FROM Event_Participant JOIN Events ON Event_Participant.Event_ID = Events.Event_ID WHERE Event_Participant.Participant_ID = @UserID AND NOT EXISTS (SELECT * FROM Schedule WHERE Schedule.Scheduled_Event = Events.Event_ID AND Schedule.Owner = @UserID);";
        using (SqlCommand query3 = new SqlCommand(getEventsQuery , connection)) {
            query3.Parameters.Add(new SqlParameter("UserID" , SqlDbType.Int));

            foreach (User user in users) {
                query3.Parameters["UserID"].Value = user.ID;

                using SqlDataReader eventsReader = query3.ExecuteReader();

                while (eventsReader.Read()) {
                    int scheduledEventID = eventsReader.GetInt32(eventsReader.GetOrdinal("Event_ID"));
                    bool isEventAlreadyAdded = false;

                    foreach (Event e in events) {
                        if (e.ID == scheduledEventID) {
                            isEventAlreadyAdded = true;
                            break;
                        }
                    }

                    if (!isEventAlreadyAdded) {
                        string scheduledEventName = eventsReader.GetString(eventsReader.GetOrdinal("Event_Name"));
                        DateTime scheduledEventDate = eventsReader.GetDateTime(eventsReader.GetOrdinal("Event_Date"));
                        TimeSpan scheduledEventTime = eventsReader.GetTimeSpan(eventsReader.GetOrdinal("Event_Time"));
                        TimeSpan scheduleEventDuration = eventsReader.GetTimeSpan(eventsReader.GetOrdinal("Duration"));
                        bool scheduledEventPriority = eventsReader.GetBoolean(eventsReader.GetOrdinal("Priority"));

                        Event e = new Event();
                        e.ID = scheduledEventID;
                        e.Name = scheduledEventName;
                        e.Date = scheduledEventDate;
                        e.Time = scheduledEventTime;
                        e.Duration = scheduleEventDuration;
                        e.Priority = scheduledEventPriority;
                        e.Participants = new List<User>();

                        e.Participants.Add(user);

                        events.Add(e);
                    } else {
                        Event existingEvent = events.Find(e => e.ID == scheduledEventID);
                        existingEvent.Participants.Add(user);
                    }
                }
            } 
        }

        PSOAlias.PSO pso = new PSOAlias.PSO(1000 , 100 , 0.5 , 1.0 , 1.0 , events.Count); 

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        pso.Run(events);
        stopwatch.Stop();

        foreach (var currentEvent in events) {
            if (currentEvent.Participants != null) {
                foreach (User user in currentEvent.Participants) {
                    Console.WriteLine(user.Name + "'s Schedule");

                    if (user.Schedule != null) {
                        foreach (Event e in user.Schedule.OrderBy(evt => evt.Date)) {
                            Console.WriteLine(" - " + e.Name + ": " + e.Date.ToShortDateString() + " " + e.Time + " - Duration: " + e.Duration);
                        }
                    }
                }
            }
        }

        double elapsedTimeInSeconds = stopwatch.ElapsedMilliseconds / 1000.0;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Elapsed running time: " + elapsedTimeInSeconds + " seconds");
        Console.ResetColor();
    }
}