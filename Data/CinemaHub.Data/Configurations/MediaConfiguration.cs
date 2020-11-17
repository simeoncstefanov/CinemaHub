namespace CinemaHub.Data.Configurations
{
    using CinemaHub.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> media)
        {
            media
                .ToTable("Media")
                .HasDiscriminator<string>("MediaType")
                .HasValue<Movie>("Movie")
                .HasValue<Show>("Show");
        }
    }
}
