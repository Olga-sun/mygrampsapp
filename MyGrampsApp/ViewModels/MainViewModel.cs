using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input; // Додаємо для ICommand
using MyGrampsApp.Models;
using MyGrampsApp.Services;
using System.Windows;

namespace MyGrampsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        // Колекція, до якої прив'язаний DataGrid
        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();

        // Команда для кнопки "Оновити"
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            // Ініціалізуємо команду
            RefreshCommand = new RelayCommand(obj => LoadPeopleData());

            // Завантажуємо дані при створенні ViewModel
            LoadPeopleData();
        }

        public void LoadPeopleData()
        {
            try
            {
                // Обов'язково очищуємо існуючу колекцію, а не створюємо нову
                People.Clear();
                var data = _dbService.GetAllPeople(App.CurrentUserId);

                foreach (var person in data)
                {
                    People.Add(person);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Помилка завантаження через сервіс: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Проста реалізація RelayCommand всередині того ж файлу (або винесіть у папку Services/Commands)
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
}