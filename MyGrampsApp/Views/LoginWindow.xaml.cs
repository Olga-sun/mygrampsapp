using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace MyGrampsApp.Views
{
    public partial class LoginWindow : Window
    {
        string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        // МЕТОД ДЛЯ ХЕШУВАННЯ 
        private string ComputeHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введіть логін та пароль!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id FROM app_users WHERE username=@user AND password_hash=@pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    // Порівнюємо з ХЕШЕМ введеного пароля
                    cmd.Parameters.AddWithValue("@pass", ComputeHash(txtPassword.Password));

                    object userId = cmd.ExecuteScalar();

                    if (userId != null)
                    {
                        App.CurrentUserId = Convert.ToInt32(userId);
                        new MainWindow().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль!");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // ВАЛІДАЦІЯ: не дозволяємо порожні або короткі дані
            if (txtUsername.Text.Length < 3 || txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Логін має бути від 3 символів, пароль — від 6!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO app_users (username, password_hash) VALUES (@user, @pass)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    // ЗБЕРІГАЄМО ХЕШ ПАРОЛЯ
                    cmd.Parameters.AddWithValue("@pass", ComputeHash(txtPassword.Password));

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Реєстрація успішна! Тепер можете увійти.");
                }
                catch (SqlException)
                {
                    MessageBox.Show("Такий логін вже зайнятий або помилка БД!");
                }
                catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
            }
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
