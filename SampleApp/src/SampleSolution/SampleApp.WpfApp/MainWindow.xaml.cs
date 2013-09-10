using System.Configuration;
using System.Windows;

namespace SampleApp.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            label.Content = string.Format("Value of config is {0}", ConfigurationManager.AppSettings["Setting"]);
        }
    }
}
