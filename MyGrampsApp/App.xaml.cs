using System.Configuration;
using System.Data;
using System.Windows;

namespace MyGrampsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int CurrentUserId { get; set; } // Тут буде ID користувача
    }

}
