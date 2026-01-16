using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyGrampsApp
{
    /// <summary>
    /// Interaction logic for AddPersonWindow.xaml
    /// </summary>
    public partial class AddPersonWindow : Window
    {
        string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        public AddPersonWindow()
        {
            InitializeComponent();
            LoadPlaces();
        }

        private void LoadPlaces()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = "SELECT id, name FROM place";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cbBirthPlace.ItemsSource = dt.DefaultView;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO person (first_name, last_name, patronymic, maiden_name, sex, birth_date, birth_place_id, notes, user_id) 
                       VALUES (@fn, @ln, @patr, @mn, @sex, @bd, @bpid, @notes, @uid)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                cmd.Parameters.AddWithValue("@patr", txtPatronymic.Text);

                cmd.Parameters.AddWithValue("@mn", string.IsNullOrWhiteSpace(txtMaidenName.Text) ? DBNull.Value : txtMaidenName.Text);

                object sexValue = (cbSex.SelectedItem as ComboBoxItem)?.Content!;
                cmd.Parameters.AddWithValue("@sex", sexValue ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@bd", (object?)dpBirthDate.SelectedDate! ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@bpid", cbBirthPlace.SelectedValue ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? DBNull.Value : txtNotes.Text);

                cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Дані успішно збережено!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при збереженні: " + ex.Message);
                }
            }
        }
    }
        
    }

