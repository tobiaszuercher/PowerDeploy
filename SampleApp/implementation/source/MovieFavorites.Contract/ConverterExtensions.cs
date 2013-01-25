using MovieFavorites.Entities;

namespace MovieFavorites.Contract
{
    public static class ConverterExtensions
    {
        public static MovieDto ConvertToDto(this Movie target)
        {
            return new MovieDto()
                {
                    Teaser = target.Teaser,
                    Title =  target.Title,
                    Year = target.Year,
                    TmdbId = target.TmdbId,
                    Cover = target.Cover,
                    Fanart = target.Backdrop,
                };
        }
    }
}