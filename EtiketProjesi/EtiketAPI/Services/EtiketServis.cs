using EtiketAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EtiketAPI.Services
{
    public class EtiketServis
    {
        private readonly EtiketDbContext _context;

        public EtiketServis(EtiketDbContext context)
        {
            _context = context;
        }

        public async Task<EtiketlenenImage> AddEtiket(
                   int userId,
                   int imageId,
                   string etiket,
                   float xCenter,
                   float yCenter,
                   float width,
                   float height)
        {
            var yeniEtiket = new EtiketlenenImage
            {
                UserId = userId,
                ImageId = imageId,
                etiket = etiket,
                x_center = xCenter,
                y_center = yCenter,
                width = width,
                height = height,
                etiketlenmeTarihi = DateTime.Now
            };

            _context.EtiketlenenImages.Add(yeniEtiket);
            await _context.SaveChangesAsync();

            return yeniEtiket;
        }

        public async Task<List<EtiketlenenImage>> GetEtiketlerByImageId(int imageId)
        {
            return await _context.EtiketlenenImages
                .Where(e => e.ImageId == imageId)
                .ToListAsync();
        }

        public async Task<List<EtiketlenenImage>?> GetEtiketlerByCode(string paylasmKodu)
        {
            var imageSet = await _context.ImageSets
                .FirstOrDefaultAsync(i => i.paylasmKodu == paylasmKodu);

            if (imageSet == null)
                return null;

            var imageIds = await _context.Images
                .Where(i => i.ImageSetId == imageSet.Id)
                .Select(i => i.Id)
                .ToListAsync();

            return await _context.EtiketlenenImages
                .Where(e => imageIds.Contains(e.ImageId))
                .ToListAsync();
        }

        public async Task<bool> DeleteEtiket(int etiketId)
        {
            var etiket = await _context.EtiketlenenImages.FindAsync(etiketId);

            if (etiket == null)
                return false;

            _context.EtiketlenenImages.Remove(etiket);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
