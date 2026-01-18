using Microsoft.Data.SqlClient;
using MyGrampsApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Windows;

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

        public bool AddKinship(int parentId, int childId, string relationType)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO kinship (parent_id, child_id, relation_type, user_id) 
                           VALUES (@pid, @cid, @type, @uid)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", parentId);
                        cmd.Parameters.AddWithValue("@cid", childId);
                        cmd.Parameters.AddWithValue("@type", relationType);
                        cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                        // Виконуємо запит. Якщо тригер викине RAISERROR, ми перейдемо в блок catch.
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    // Виводимо повідомлення про дублікат або іншу помилку БД
                    MessageBox.Show(ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка додавання: " + ex.Message);
                    return false;
                }
            }
        }
        public bool AddPerson(Person person)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    // Запит включає всі 10 колонок з вашої структури (крім id)
                    string sql = @"INSERT INTO person (sex, birth_date, death_date, birth_place_id, notes, last_name, first_name, patronymic, maiden_name, user_id) 
                           VALUES (@sex, @bd, @dd, @bpid, @notes, @ln, @fn, @pat, @mn, @uid)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sex", person.Sex);
                        cmd.Parameters.AddWithValue("@bd", person.BirthDate);
                        cmd.Parameters.AddWithValue("@dd", (object)person.DeathDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@bpid", (object)person.BirthPlaceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@notes", (object)person.Notes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ln", person.LastName);
                        cmd.Parameters.AddWithValue("@fn", person.FirstName);
                        cmd.Parameters.AddWithValue("@pat", person.Patronymic);
                        cmd.Parameters.AddWithValue("@mn", (object)person.MaidenName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка додавання особи: " + ex.Message);
                    return false;
                }
            }
        }
        public DataTable GetPlaces()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT id, name FROM place";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка завантаження місць: " + ex.Message);
                    return new DataTable(); // Повертаємо порожню таблицю, щоб уникнути NullReference
                }
            }
        }
    }
}
