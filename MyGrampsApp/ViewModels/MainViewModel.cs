using MyGrampsApp.Models;
using MyGrampsApp.Services;
using MyGrampsApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input; // Додаємо для ICommand

namespace MyGrampsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set { _selectedPerson = value; OnPropertyChanged(nameof(SelectedPerson)); }
        }
        // Команди для вікон введення
        public ICommand RefreshCommand { get; }
        public ICommand OpenKinshipCommand { get; }
        public ICommand OpenAddPersonCommand { get; }
        public ICommand OpenAddDocumentCommand { get; }

        public ICommand OpenSearchCommand { get; }
        public ICommand OpenFilterPlaceCommand { get; }
        public ICommand OpenShowDocsCommand { get; }
        public ICommand OpenBuildTreeCommand { get; }
        public ICommand OpenStatsCommand { get; }
        public ICommand OpenBioCommand { get; }
        public ICommand DeletePersonCommand { get; }

        public MainViewModel()
        {
            // Ініціалізація існуючих команд
            RefreshCommand = new RelayCommand(obj => LoadPeopleData());
            OpenKinshipCommand = new RelayCommand(obj => OpenKinshipWindow());
            OpenAddPersonCommand = new RelayCommand(obj => OpenAddPersonWindow());
            OpenBuildTreeCommand = new RelayCommand(obj => OpenBuildTree());
            // Ініціалізація нових команд та заглушок
            OpenAddDocumentCommand = new RelayCommand(obj => OpenAddDocumentWindow());
            DeletePersonCommand = new RelayCommand(
                execute: obj => ExecuteDeletePerson(obj as Person),
                canExecute: obj => obj is Person // Кнопка активна тільки якщо вибрано людину
            );
            // Заглушки (виводять повідомлення, поки вікна не створені)
            OpenSearchCommand = new RelayCommand(obj => MessageBox.Show("Вікно пошуку розробляється"));
            OpenFilterPlaceCommand = new RelayCommand(obj => MessageBox.Show("Фільтрація розробляється"));
            OpenShowDocsCommand = new RelayCommand(obj => MessageBox.Show("Список документів розробляється"));
            
            OpenStatsCommand = new RelayCommand(obj => MessageBox.Show("Статистика розробляється"));
            OpenBioCommand = new RelayCommand(obj => MessageBox.Show("Біографія розробляється"));

            LoadPeopleData();
        }

        private void ExecuteDeletePerson(Person person)
        {
            if (person == null) return;

            var result = MessageBox.Show($"Ви впевнені, що хочете видалити {person.FirstName} {person.LastName}?",
                                         "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_dbService.DeletePerson(person.Id))
                {
                    MessageBox.Show("Особу видалено");
                    LoadPeopleData(); // Оновлюємо список
                }
            }
        }
        private void OpenBuildTree()
        {
            FamilyTreeWindow treeWin = new FamilyTreeWindow();
            treeWin.Owner = Application.Current.MainWindow; // Щоб вікно було по центру головного
            treeWin.ShowDialog();
        }
        private void OpenAddPersonWindow()
        {
            var win = new AddPersonWindow();
            win.DataContext = new AddPersonViewModel();
            win.ShowDialog();
            LoadPeopleData();
        }

        private void OpenAddDocumentWindow()
        {
            var win = new AddDocumentWindow();
            // win.DataContext = new AddDocumentViewModel(); // Створимо пізніше
            win.ShowDialog();
        }

        private void OpenKinshipWindow()
        {
            var win = new KinshipWindow();
            win.DataContext = new KinshipViewModel(this.People);
            win.ShowDialog();
            LoadPeopleData();
        }

        public void LoadPeopleData()
        {
            try
            {
                People.Clear();
                var data = _dbService.GetAllPeople(App.CurrentUserId);
                foreach (var person in data) People.Add(person);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Помилка завантаження: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        // Проста реалізація RelayCommand всередині того ж файлу (або винесіть у папку Services/Commands)
        
    }
    public class RelayCommand : ICommand
        {
            private readonly System.Action<object> _execute;
            private readonly System.Predicate<object> _canExecute;

            public RelayCommand(System.Action<object> execute, System.Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new System.ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
            public void Execute(object parameter) => _execute(parameter);
            public event System.EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }

        // --- VIEWMODEL ДЛЯ ВІКНА ЗВ'ЯЗКІВ ---
        public class KinshipViewModel : INotifyPropertyChanged
        {
            private readonly DatabaseService _dbService = new DatabaseService();
            public ObservableCollection<Person> People { get; set; }

            private Person _selectedParent;
            public Person SelectedParent
            {
                get => _selectedParent;
                set { _selectedParent = value; OnPropertyChanged(nameof(SelectedParent)); }
            }

            private Person _selectedChild;
            public Person SelectedChild
            {
                get => _selectedChild;
                set { _selectedChild = value; OnPropertyChanged(nameof(SelectedChild)); }
            }

            public string SelectedRelationType { get; set; } = "parent-child";
            public ICommand SaveKinshipCommand { get; }

            public KinshipViewModel(IEnumerable<Person> currentPeople)
            {
                People = new ObservableCollection<Person>(currentPeople);

                SaveKinshipCommand = new RelayCommand(obj =>
                {
                    // Перевірки перед відправкою
                    if (SelectedParent == null || SelectedChild == null)
                    {
                        MessageBox.Show("Виберіть обох осіб!");
                        return;
                    }

                    if (SelectedParent.Id == SelectedChild.Id)
                    {
                        MessageBox.Show("Особа не може бути родичем самій собі!");
                        return;
                    }

                    // Один виклик сервісу. 
                    // Результат true означає успіх. 
                    // Результат false означає, що DatabaseService вже показав текст помилки з БД.
                    if (_dbService.AddKinship(SelectedParent.Id, SelectedChild.Id, SelectedRelationType))
                    {
                        MessageBox.Show("Зв'язок успішно додано до бази!");
                    }
                });
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public class AddPersonViewModel : INotifyPropertyChanged
        {
            private readonly DatabaseService _dbService = new DatabaseService();

            // Об'єкт для нової особи
            public Person NewPerson { get; set; } = new Person();

            // Список місць для ComboBox
            public DataTable Places { get; set; }

            public ICommand SaveCommand { get; }

            public AddPersonViewModel()
            {
                // Завантажуємо місця при ініціалізації ViewModel
                LoadPlaces();

                SaveCommand = new RelayCommand(obj =>
                {
                    if (string.IsNullOrWhiteSpace(NewPerson.FirstName) || string.IsNullOrWhiteSpace(NewPerson.LastName))
                    {
                        MessageBox.Show("Будь ласка, заповніть ім'я та прізвище.");
                        return;
                    }

                    NewPerson.UserId = App.CurrentUserId;

                    if (_dbService.AddPerson(NewPerson))
                    {
                        MessageBox.Show("Особу успішно зареєстровано!");
                        if (obj is Window window) window.Close();
                    }
                });
            }

            private void LoadPlaces()
            {
            Places = _dbService.GetPlaces();

            OnPropertyChanged(nameof(Places));
        }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

}