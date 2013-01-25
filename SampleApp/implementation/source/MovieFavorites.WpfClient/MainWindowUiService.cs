using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

using MovieFavorites.Contract;

using ServiceStack.ServiceClient.Web;

namespace MovieFavorites.WpfClient
{
    public class MainWindowUiService
    {
        public ObservableCollection<MovieDto> Movies { get; private set; }

        public MainWindowUiService()
        {
            var restClient = new JsonServiceClient(ConfigurationManager.AppSettings["MovieServiceUrl"]);
            var movies = restClient.Get<List<MovieDto>>("movies");

            var moviesConverted = movies.Select(Movie.CreateFromDto).ToList();

            Movies = new ObservableCollection<MovieDto>(moviesConverted);
        } 
    }
}