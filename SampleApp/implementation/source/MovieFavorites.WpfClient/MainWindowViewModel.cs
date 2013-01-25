using System.ComponentModel;
using System.Windows.Data;

using MovieFavorites.Contract;

namespace MovieFavorites.WpfClient
{
    public class MainWindowViewModel
    {
        private readonly MainWindowUiService _mainWindowUiService;
        private readonly CollectionViewSource _movieCollectionViewSource;

        public MainWindowViewModel(MainWindowUiService mainWindowUiService)
        {
            _mainWindowUiService = mainWindowUiService;
            _movieCollectionViewSource = new CollectionViewSource { Source = mainWindowUiService.Movies };
        }

        public ICollectionView Movies
        {
            get { return _movieCollectionViewSource.View; }
        }

    }
}