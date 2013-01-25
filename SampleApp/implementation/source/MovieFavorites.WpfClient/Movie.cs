using System;
using System.Windows.Media.Imaging;

using MovieFavorites.Contract;

namespace MovieFavorites.WpfClient
{
  
        public class Movie : MovieDto
        {
            public BitmapImage CoverImage { get; set; }

            public BitmapImage FanArtImage { get; set; }

            public static Movie CreateFromDto(MovieDto dto)
            {
                return new Movie()
                    {
                        Cover = dto.Cover,
                        Fanart = dto.Fanart,
                        Teaser = dto.Teaser,
                        Title = dto.Title,
                        TmdbId = dto.TmdbId,
                        Year = dto.Year,
                        FanArtImage = new BitmapImage(new Uri(dto.Fanart)),
                        CoverImage = new BitmapImage(new Uri(dto.Cover)),
                    };
            }
        } 
}