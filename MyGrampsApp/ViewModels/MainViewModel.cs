using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using MyGrampsApp.Models;
using System.Windows;

namespace MyGrampsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        // Список людей, який побачить DataGrid
        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();

        public void LoadPeopleData()
        {
            People.Clear();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string sql = @"
                        SELECT p.id, p.first_name, p.last_name, p.sex, 
                               CONVERT(VARCHAR, p.birth_date, 104) AS birth_date, 
                               CONVERT(VARCHAR, p.death_date, 104) AS death_date
                        FROM person p
                        WHERE p.user_id = @uid";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            People.Add(new Person
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
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка завантаження: " + ex.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}