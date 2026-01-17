using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;         
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace MyGrampsApp
{
    public partial class MainWindow : Window
    {
        string connString = "Server=localhost;Database=new_database;User Id=sa;Password=2026777;TrustServerCertificate=True;";

        public MainWindow()
        {
            InitializeComponent();
            LoadPeopleData();
        }

        // --- СЕКЦІЯ 1: ВВЕДЕННЯ ІНФОРМАЦІЇ ---

        private void btnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            AddPersonWindow addPersonWin = new AddPersonWindow();
            addPersonWin.Owner = this;
            addPersonWin.ShowDialog();
        }

        private void btnAddDocument_Click(object sender, RoutedEventArgs e)
        {
            AddDocumentWindow addDocWin = new AddDocumentWindow();
            addDocWin.Owner = this;
            addDocWin.ShowDialog();
        }

        private void btnAddKinship_Click(object sender, RoutedEventArgs e)
        {
            // Тут ми створимо вікно для зв'язків пізніше
            MessageBox.Show("Вікно встановлення зв'язків у розробці.");
        }

        // --- СЕКЦІЯ 2: ПОШУК ТА ФІЛЬТРАЦІЯ ---

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Тут ми створимо вікно пошуку з таблицею
            MessageBox.Show("Тут буде вікно пошуку родичів.");
        }

        private void btnFilterPlace_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут буде фільтрація за містом.");
        }

        private void btnShowDocs_Click(object sender, RoutedEventArgs e)
        {
            // Це кнопка "Переглянути документи"
            MessageBox.Show("Тут буде список ваших файлів для перегляду.");
        }

        // --- СЕКЦІЯ 3: ГЕНЕРАЦІЯ ЗВІТІВ ---

        private void btnBuildTree_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функція побудови дерева готується.");
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут буде статистика тривалості життя.");
        }

        private void btnBio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Генерація біографічної картки.");
        }

        // Метод для оновлення списку (якщо він вам ще потрібен)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Видалили звернення до myDataGrid, щоб не було помилки CS0103
            MessageBox.Show("Дані оновлено!");
        }
        // Дозволяє перетягувати вікно мишкою за нову панель
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) btnMaximize_Click(sender, e);
            else this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        // ВИПРАВЛЕНО: прибираємо 'this.' перед Application
        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void LoadPeopleData()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Використовуємо прості імена стовпців для стабільності
                    string sql = @"
                SELECT 
                    p.first_name AS [Ім'я], 
                    p.last_name AS [Прізвище], 
                    p.sex AS [Стать], 
                    CONVERT(VARCHAR, p.birth_date, 104) AS [Дата нар], 
                    CONVERT(VARCHAR, p.death_date, 104) AS [Дата см],
                    (SELECT TOP 1 occupation 
                     FROM person_occupation 
                     WHERE person_id = p.id AND user_id = @uid 
                     ORDER BY from_dt DESC) AS [Професія]
                FROM person p
                WHERE p.user_id = @uid";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", App.CurrentUserId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgPeople.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка завантаження: " + ex.Message);
                }
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPeopleData(); 
        }
    }
}