using System.Windows;
using System.Windows.Input;
using MyGrampsApp.ViewModels;

namespace MyGrampsApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Єдиний важливий рядок — встановлення зв'язку з логікою
            this.DataContext = new MainViewModel();
        }

        // Залишаємо тільки системні функції вікна
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) btnMaximize_Click(sender, e);
            else this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
        private void btnMaximize_Click(object sender, RoutedEventArgs e) =>
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}