using MyGrampsApp.Models;
using MyGrampsApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyGrampsApp.Views
{
    public partial class FamilyTreeWindow : Window
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public FamilyTreeWindow()
        {
            InitializeComponent();
            LoadTree(); // ВИКЛИКАЄМО ЗАВАНТАЖЕННЯ ПРИ ВІДКРИТТІ
        }

        private void LoadTree()
        {
            try
            {
                int userId = App.CurrentUserId; // Наш користувач №5

                // 1. Отримуємо людей та зв'язки
                var people = _dbService.GetAllPeople(userId);
                DataTable links = _dbService.GetFamilyLinks(userId);

                if (people == null || people.Count == 0) return;

                // 2. Створюємо словник нод (FamilyMemberNode)
                var nodes = people.ToDictionary(p => p.Id, p => new FamilyMemberNode
                {
                    Id = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}"
                });

                var childrenIds = new HashSet<int>();

                // 3. Будуємо ієрархію
                foreach (DataRow row in links.Rows)
                {
                    int parentId = (int)row["parent_id"];
                    int childId = (int)row["child_id"];

                    if (nodes.ContainsKey(parentId) && nodes.ContainsKey(childId))
                    {
                        nodes[parentId].Children.Add(nodes[childId]);
                        childrenIds.Add(childId);
                    }
                }

                // 4. Знаходимо "верхівку" дерева (ті, хто не є дітьми)
                var roots = nodes.Values.Where(n => !childrenIds.Contains(n.Id)).ToList();

                // 5. Прив'язуємо до TreeView у XAML
                tvFamilyTree.ItemsSource = roots;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка побудови дерева: " + ex.Message);
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}