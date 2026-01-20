using Microsoft.Data.SqlClient;
using MyGrampsApp.Models;
using Mysqlx.Crud;
using System.Data;
using System.Windows;

namespace MyGrampsApp.Services
{
    public class DatabaseService
    {

        private readonly string _connString = "Server=localhost;Database=new_database;User Id=UserTreeApp;Password=StrongPassword2026;TrustServerCertificate=True;";

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

        public List<Person> SearchPeopleByLastName(string lastName, int userId)
        {
            var people = new List<Person>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                // Використовуємо LIKE для пошуку за частиною прізвища
                string sql = @"SELECT p.id, p.first_name, p.last_name, p.sex, 
                              CONVERT(VARCHAR, p.birth_date, 104) AS birth_date, 
                              CONVERT(VARCHAR, p.death_date, 104) AS death_date
                       FROM person p 
                       WHERE p.user_id = @uid AND p.last_name LIKE @ln";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@ln", "%" + lastName + "%");

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

                        
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                  
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
                    return new DataTable(); 
                }
            }
        }
        public DataTable GetFamilyLinks(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT parent_id, child_id FROM kinship WHERE user_id = @uid";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@uid", userId);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public bool DeletePerson(int personId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                
                string sql = "DELETE FROM person WHERE id = @pid AND user_id = @uid";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", personId);
                    cmd.Parameters.AddWithValue("@uid", App.CurrentUserId); 

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public bool UpdatePerson(Person person)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = @"UPDATE person SET first_name=@fn, last_name=@ln, sex=@sex, 
                       birth_date=@bd, death_date=@dd, notes=@notes 
                       WHERE id=@id AND user_id=@uid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fn", person.FirstName);
                    cmd.Parameters.AddWithValue("@ln", person.LastName);
                    cmd.Parameters.AddWithValue("@sex", person.Sex);
                    cmd.Parameters.AddWithValue("@bd", person.BirthDate);
                    cmd.Parameters.AddWithValue("@dd", (object)person.DeathDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@notes", (object)person.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", person.Id);
                    cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteKinship(int parentId, int childId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                string sql = "DELETE FROM kinship WHERE parent_id=@pid AND child_id=@cid AND user_id=@uid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", parentId);
                    cmd.Parameters.AddWithValue("@cid", childId);
                    cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

    }
}

