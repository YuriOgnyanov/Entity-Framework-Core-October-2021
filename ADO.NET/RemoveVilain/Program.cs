using Microsoft.Data.SqlClient;
using System;

namespace RemoveVilain
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            var connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                int id = int.Parse(Console.ReadLine());
                SqlDataReader reader = SelectNameWithId(connection, id);

                if (reader.HasRows is false)
                {
                    Console.WriteLine("No such villain was found.");
                }
                else
                {
                    var name = string.Empty;
                    using (reader)
                    {
                        reader.Read();
                        name = (string)reader["Name"];
                    }

                    // detele query Minions
                    int count = DeleteMinions(connection, id);


                    //detele villains
                    DeleteVillain(connection, id);

                    Console.WriteLine($"{name} was deleted.");
                    Console.WriteLine($"{count} minions were released.");
                }



            }

        }

        private static void DeleteVillain(SqlConnection connection, int id)
        {
            var deleteVillainQuery = @"DELETE FROM Villains
                                                  WHERE Id = @villainId";
            var deleteVillainCommand = new SqlCommand(deleteVillainQuery, connection);
            deleteVillainCommand.Parameters.AddWithValue("@villainId", id);
        }

        private static int DeleteMinions(SqlConnection connection, int id)
        {
            var deleteQuery = @"DELETE FROM MinionsVillains 
                                           WHERE VillainId = @villainId";

            var deleteCommand = new SqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@villainId", id);
            var count = deleteCommand.ExecuteNonQuery();
            return count;
        }

        private static SqlDataReader SelectNameWithId(SqlConnection connection, int id)
        {
            var selectQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
            var command = new SqlCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@villainId", id);

            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }
    }
}
