using Microsoft.EntityFrameworkCore;
namespace EtiketAPI.Models
{
    public class EtiketDbContext:DbContext
    {
        public EtiketDbContext(DbContextOptions<EtiketDbContext>options):base(options) { }  

        public DbSet<User>Users { get; set; }

        public DbSet<Image>Images { get; set; }

        public DbSet<EtiketlenenImage> EtiketlenenImages { get;set; }

        public DbSet<ImageSet> ImageSets { get; set; }


    }
}
