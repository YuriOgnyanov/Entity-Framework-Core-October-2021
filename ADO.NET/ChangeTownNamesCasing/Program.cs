using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace ChangeTownNamesCasing
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

                //You will receive one line of input with the name of the country.
                string country = Console.ReadLine();

                var updateQuery = @"UPDATE Towns
                                           SET Name = UPPER(Name)
                                           WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

                var commandUpdate = new SqlCommand(updateQuery, connection);
                commandUpdate.Parameters.AddWithValue("@countryName", country);
                var result = commandUpdate.ExecuteNonQuery();

                if (result == 0)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    Console.WriteLine($"{result} town names were affected.");
                }

                var selectQuery = @"SELECT t.Name 
                                           FROM Towns as t
                                           JOIN Countries AS c ON c.Id = t.CountryCode
                                           WHERE c.Name = @countryName";

                var commandSelect = new SqlCommand(selectQuery, connection);
                commandSelect.Parameters.AddWithValue("@countryName", country);

                SqlDataReader reader = commandSelect.ExecuteReader();

                StringBuilder sb = new StringBuilder();

                using (reader)
                {

                    while (reader.Read())
                    {
                        sb.Append($"{reader["Name"]} ");

                    }
                    Console.WriteLine(sb.ToString().TrimEnd());
                }

            }
        }
    }
}
