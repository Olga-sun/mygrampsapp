using Microsoft.Data.SqlClient;
using Microsoft.Win32;
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
namespace MyGrampsApp
{
    /// <summary>
    /// Interaction logic for AddDocumentWindow.xaml
    /// </summary>
    public partial class AddDocumentWindow : Window
    {
        string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";
        public AddDocumentWindow()
        {
            InitializeComponent();
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Фільтр для зображень та PDF
            openFileDialog.Filter = "Зображення (*.jpg;*.png)|*.jpg;*.png|Документи PDF (*.pdf)|*.pdf|Всі файли (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                txtFileUri.Text = openFileDialog.FileName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            byte[]? fileData = null;

            // 1. Зчитуємо файл у пам'ять, якщо шлях вказано
            if (!string.IsNullOrEmpty(txtFileUri.Text) && System.IO.File.Exists(txtFileUri.Text))
            {
                fileData = System.IO.File.ReadAllBytes(txtFileUri.Text); // Перетворюємо файл у байти
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // 2. Додаємо @data у наш SQL запит
                string sql = @"INSERT INTO document (kind, title, created_dt, file_uri, text, user_id, file_data) 
                       VALUES (@kind, @title, @dt, @uri, @text, @uid, @data)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kind", cbKind.Text);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                cmd.Parameters.AddWithValue("@dt", (object?)dpCreatedDate.SelectedDate ?? DBNull.Value);

                // Зберігаємо лише назву файлу, а не шлях до вашого ноута
                cmd.Parameters.AddWithValue("@uri", System.IO.Path.GetFileName(txtFileUri.Text));

                cmd.Parameters.AddWithValue("@text", txtDocumentText.Text);
                cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                // 3. Відправляємо байти файлу в базу
                cmd.Parameters.AddWithValue("@data", (object?)fileData ?? DBNull.Value);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Документ збережено в базі! Тепер він доступний з будь-якого пристрою.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }
    }
}
