using Microsoft.Data.SqlClient;
using MyGrampsApp.Models;
using Mysqlx.Crud;
using System.Data;
using System.Windows;

namespace MyGrampsApp.Services
{
    public class DatabaseService
    {
        private readonly string _connString = "Server=localhost;Database=new_database;Integrated Security=True;TrustServerCertificate=True;";

        public List<Person> GetAllPeople(int userId)
        {
            var people = new List<Person>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                conn.Open();
                // Додаємо JOIN з таблицею place
                string sql = @"
    SELECT p.id, p.first_name, p.last_name, p.patronymic, p.sex, 
           p.birth_date, p.death_date, -- БЕЗ КОНВЕРТАЦІЇ У VARCHAR
           p.birth_place_id, pl.name AS birth_place_name
    FROM person p
    LEFT JOIN place pl ON p.birth_place_id = pl.id
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
                                Patronymic = reader["patronymic"].ToString(),
                                Sex = reader["sex"].ToString(),
                                BirthDate = reader["birth_date"] as DateTime?, // Пряме приведення до DateTime?
                                DeathDate = reader["death_date"] as DateTime?,
                                // Додаємо нові поля:
                                BirthPlaceId = reader["birth_place_id"] as int?,
                                BirthPlaceName = reader["birth_place_name"]?.ToString() ?? "Не вказано"
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
                string sql = @"SELECT p.id, p.first_name, p.last_name, p.patronymic, p.sex, 
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
                                Patronymic = reader["patronymic"].ToString(),
                                Sex = reader["sex"].ToString(),
                                BirthDate = reader["birth_date"] as DateTime?,
                                DeathDate = reader["death_date"] as DateTime?
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
        
        public int? EnsurePlaceExists(string placeName)
        {
            // Якщо поле порожнє, повертаємо null (місце не вказано)
            if (string.IsNullOrWhiteSpace(placeName)) return null;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    //  Перевіряємо, чи вже існує місто з такою назвою
                    string checkSql = "SELECT id FROM place WHERE name = @name";
                    using (SqlCommand cmd = new SqlCommand(checkSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", placeName.Trim());
                        var result = cmd.ExecuteScalar();

                        // Якщо знайшли — повертаємо його ID
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }

                        // 2. Якщо такого міста немає — створюємо новий запис
                        // OUTPUT INSERTED.id дозволяє одразу отримати ID нового рядка
                        string insertSql = "INSERT INTO place (name) OUTPUT INSERTED.id VALUES (@name)";
                        using (SqlCommand insCmd = new SqlCommand(insertSql, conn))
                        {
                            insCmd.Parameters.AddWithValue("@name", placeName.Trim());
                            return (int)insCmd.ExecuteScalar();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при роботі з містами: " + ex.Message);
                    return null;
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
        public bool AddPerson(Person person, string newPlaceName)
        {
            int? placeId = EnsurePlaceExists(newPlaceName);
            if (placeId != null) person.BirthPlaceId = placeId;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO person (sex, birth_date, death_date, notes, last_name, first_name, patronymic, maiden_name, user_id, birth_place_id) 
                           VALUES (@sex, @bd, @dd, @notes, @ln, @fn, @pat, @mn, @uid, @bpid)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sex", person.Sex);

                        // Передаємо об'єкт DateTime? напряму, обробляючи NULL
                        cmd.Parameters.Add("@bd", SqlDbType.Date).Value = (object)person.BirthDate ?? DBNull.Value;
                        cmd.Parameters.Add("@dd", SqlDbType.Date).Value = (object)person.DeathDate ?? DBNull.Value;

                        cmd.Parameters.AddWithValue("@bpid", (object)person.BirthPlaceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@notes", (object)person.Notes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ln", person.LastName);
                        cmd.Parameters.AddWithValue("@fn", person.FirstName);
                        cmd.Parameters.AddWithValue("@pat", person.Patronymic ?? (object)DBNull.Value);
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

        public bool UpdatePerson(Person person, string newPlaceName)
        {
            int? placeId = EnsurePlaceExists(newPlaceName);
            if (placeId != null) person.BirthPlaceId = placeId;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE person SET first_name=@fn, last_name=@ln, patronymic=@pat, sex=@sex, 
                           birth_date=@bd, death_date=@dd, notes=@notes, birth_place_id=@bpid 
                           WHERE id=@id AND user_id=@uid";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@fn", person.FirstName);
                        cmd.Parameters.AddWithValue("@ln", person.LastName);
                        cmd.Parameters.AddWithValue("@pat", person.Patronymic ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sex", person.Sex);

                        // Передаємо об'єкт DateTime? напряму
                        cmd.Parameters.Add("@bd", SqlDbType.Date).Value = (object)person.BirthDate ?? DBNull.Value;
                        cmd.Parameters.Add("@dd", SqlDbType.Date).Value = (object)person.DeathDate ?? DBNull.Value;

                        cmd.Parameters.AddWithValue("@notes", (object)person.Notes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@bpid", (object)person.BirthPlaceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", person.Id);
                        cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка оновлення особи: " + ex.Message);
                    return false;
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

