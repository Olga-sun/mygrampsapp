using Microsoft.Data.SqlClient;
using MyGrampsApp.Models;
using System.Collections.Generic;

namespace MyGrampsApp.Services
{
    public class DatabaseService
    {
        // Тимчасово залишаємо тут, але пізніше винесемо в конфіг
        private readonly string _connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        public List<Person> GetAllPeople(int userId)
        {
            var people = new List<Person>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = @"
                    SELECT p.id, p.first_name, p.last_name, p.sex, 
                           CONVERT(VARCHAR, p.birth_date, 104) AS birth_date, 
                           CONVERT(VARCHAR, p.death_date, 104) AS death_date
                    FROM person p
                    WHERE p.user_id = @uid";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            people.Add(new Person
                            {
                                Id = (int)reader["id"],
                                FirstName = reader["first_name"].ToString(),
                                LastName = reader["last_name"].ToString(),
                                Sex = reader["sex"].ToString(),
                                BirthDate = reader["birth_date"].ToString(),
                                DeathDate = reader["death_date"].ToString()
                            });
                        }
                    }
                }
            }
            return people;
        }
    }
}