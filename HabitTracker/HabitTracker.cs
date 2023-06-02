using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    public static class HabitTrackerClass
    {

        private static string connectionString = @"Data Source=habit-Tracker.db";

        public static void CreateTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS habits (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static void GetUserInput()
        {
            Console.Clear();

            bool closeApp = false;
            while (closeApp == false)
            {

                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("\nType 1 to View All Records.");
                Console.WriteLine("\nType 2 to Insert Record.");
                Console.WriteLine("\nType 3 to Delete Record.");
                Console.WriteLine("\nType 4 to Update Record.");
                Console.WriteLine("\n---------------------------------------\n");

                string? commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye\n");
                        closeApp = true;
                        break;

                    case "1":
                        GetAllRecords();
                        break;

                    case "2":
                        Insert();
                        break;

                    case "3":
                        Delete();
                        break;

                    case "4":
                        Update();
                        break;

                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;

                }


            }
        }

        static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE from habits WHERE Id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} not found");
                    Delete();
                }

            }

            Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");

            GetUserInput();

        }

        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM habits ";

                List<Habit> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new Habit
                        {
                            Id = reader.GetInt32(0),
                            HabitDetails = reader.GetString(1),
                            Quantity = reader.GetInt32(2)
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                Console.WriteLine("------------------------------------------\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.HabitDetails} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("------------------------------------------\n");
            }
        }

        private static void Insert()
        {

            string? habit = GetHabitInput();

            int quantity = GetNumberInput("\n\nPlease insert number of times habit was completed (no decimals allowed)\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                           $"INSERT INTO habits(date, quantity) VALUES('{habit}', {quantity})";

                Console.WriteLine($"Executing command: {tableCmd.CommandText.ToString()}");

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static string? GetHabitInput()
        {
            Console.WriteLine("\n\nPlease insert the habit. Type 0 to return to main menu\n\n");
            string? habit = Console.ReadLine();
            return habit;
        }

        static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string? numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            if (!String.IsNullOrEmpty(numberInput))
            {
                return Convert.ToInt32(numberInput);
            }

            else return -1;
        }

        static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type Id of the habit you would like to update. Type 0 to return to main manu.\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {

                connection.Open();
                var checkCmd = connection.CreateCommand();

                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM habits WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                    connection.Close();
                    Update();
                }

                string? habit = GetHabitInput();

                int quantity = GetNumberInput("\n\nPlease insert number of times you have completed this habit (no decimals allowed)\n\n");


                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"UPDATE habits SET date = '{habit}', quantity = '{quantity}' WHERE Id = '{recordId}'";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }

        }

    }
}
