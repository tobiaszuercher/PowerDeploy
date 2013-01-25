using System.Windows;

namespace MovieFavorites.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            var mainWindow = new MainWindow();
            var mainWindowUiService = new MainWindowUiService();

            mainWindow.ViewModel = new MainWindowViewModel(mainWindowUiService);

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}