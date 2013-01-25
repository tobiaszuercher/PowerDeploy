using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MovieFavorites.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _nextTimer;
        private DispatcherTimer _prevTimer;
        public MainWindow()
        {
            InitializeComponent();
            _nextTimer = new DispatcherTimer{ Interval = TimeSpan.FromMilliseconds(500)};
            _nextTimer.Tick += (s, e) =>
                {
                    ViewModel.Movies.MoveCurrentToNext();
                   
                    if (ViewModel.Movies.IsCurrentAfterLast)
                    {
                        ViewModel.Movies.MoveCurrentToFirst();
                    }
                    
                    _nextTimer.Stop();
                };
            _prevTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _prevTimer.Tick += (s, e) =>
                {
                    ViewModel.Movies.MoveCurrentToPrevious();    
                    
                    if (ViewModel.Movies.IsCurrentBeforeFirst)
                    {
                        ViewModel.Movies.MoveCurrentToLast();
                    }
                    
                    _prevTimer.Stop();
                };
        }

        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void LayoutRoot_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _prevTimer.Start();
            }

            if (e.Key == Key.Right)
            {
                _nextTimer.Start();
            }
        }
    }
}