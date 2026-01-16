using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
namespace MyGrampsApp
{

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        // Рядок підключення (той самий, що ми перевіряли)
        string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        // Кнопка Входу
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    // Перевіряємо, чи є користувач з таким логіном і паролем
                    string query = "SELECT id FROM app_users WHERE username=@user AND password_hash=@pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Password);

                    object userId = cmd.ExecuteScalar();

                    if (userId != null)
                    {
                        // Якщо знайшли — зберігаємо ID та відкриваємо головне вікно
                        App.CurrentUserId = Convert.ToInt32(userId);
                        MainWindow main = new MainWindow();
                        main.Show();
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

        // Кнопка Реєстрації
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO app_users (username, password_hash) VALUES (@user, @pass)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Password);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Реєстрація успішна! Тепер можете увійти.");
                }
                catch (Exception) { MessageBox.Show("Такий логін вже зайнятий!"); }
            }
        }
    }
}
