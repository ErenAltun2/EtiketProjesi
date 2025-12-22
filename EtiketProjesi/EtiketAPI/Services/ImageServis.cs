using EtiketAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassLibrary;

namespace EtiketAPI.Services
{
    public class ImageServis
    {
        private readonly EtiketDbContext _context;

        public ImageServis(EtiketDbContext context)
        {
            _context = context;
        }

        private async Task<string> BenzersizKod()
        {
            string kod;
            bool KodVarmı;

            do
            {
                Random random = new Random();
                kod = random.Next(10000, 999999).ToString();   //6 hanelı kod uretecegım burada
                                                               // bu kod daha once kullanılmıs mı kontrol edecegiz 
                KodVarmı = await _context.ImageSets.AnyAsync(x => x.paylasmKodu == kod);
            }
            while (KodVarmı);

            return kod;
            
            //burada ılk basta do kullandım cunku method calısır calısmaz bır random kod uretmem gerekıyordu bunu da kontrol etmek fazladan ıf else kullanacagımıza 
            // do whıle ıle cozuyoruz eger anyasync bıze true false donduruyor hıc aynı kod var mı dıye bakıyor true derse bır daha olusturuyor. 

        }

        public async Task<ImageSet>CreateImageSet(int UserId)
        {
            string kod = await BenzersizKod();

            var imageSet = new ImageSet();
            imageSet.UserId = UserId;
            imageSet.olusturmaTarihi=DateTime.Now;
            imageSet.paylasmKodu = kod;

            _context.ImageSets.Add(imageSet);
            await _context.SaveChangesAsync();
            return imageSet;    
        }

        public async Task<bool> SaveImages(int imageSetId, List<ImageUploadDto> images)
        {
            var imageSet = await _context.ImageSets.FindAsync(imageSetId);
            if (imageSet==null)
            {
                return false;
            }
            foreach (var img in images) 
            {
                var image = new Image();
                image.ImageSetId = imageSetId;
                image.Name = img.Name;
                image.yuklenmeTarihi=DateTime.Now;
                image.image = img.ImageData;
                _context.Images.Add(image);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Image>?> ImageGetByCode(string paylasmaKodu)
        {
            var imageSet = await _context.ImageSets
                .FirstOrDefaultAsync(x => x.paylasmKodu == paylasmaKodu);

            if (imageSet == null)
                return null;

            var images = await _context.Images
                .Where(x => x.ImageSetId == imageSet.Id)
                .ToListAsync();

            return images;
        }

        //ekranda resımlerı tek tek gosterıp etıketlemek ıcın gırıyoruz bunu    yanı mehmet kod ıle ekranda resımlerı goruyor bu resımlerden ıd sıne bastıgını ekranda gosteren buyutup bu oluyor 
        public async Task<Image?> GetImageById(int imageId)
        {
            return await _context.Images.FindAsync(imageId);
        }

        // bunu neden yazdık burada bu şunu yapıyor kodu gıren kısı bu klasoru kımın actıgını olusturma tarıhını gorebılecek bu olmasa kodu gıren kısı bunu kımın actıgını ekranda goremeyecek. 
        public async Task<ImageSet?> GetImageSetByCode(string paylasmKodu)
        {
            return await _context.ImageSets
                .FirstOrDefaultAsync(x => x.paylasmKodu == paylasmKodu);
        }

        //kullanıcı kendı olusturdugu klasörleri gormek ıcın 
        public async Task<List<ImageSet>> GetUserImageSets(int userId)
        {
            return await _context.ImageSets
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.olusturmaTarihi)
                .ToListAsync();
        }


        // Bir ImageSet'e ait resim sayısını getir
        public async Task<int> GetImageCountBySetId(int imageSetId)
        {
            return await _context.Images
                .Where(i => i.ImageSetId == imageSetId)
                .CountAsync();
        }

        public async Task<bool> DeleteImageSet(int imageSetId)
        {
            var imageSet = await _context.ImageSets.FindAsync(imageSetId);

            if (imageSet == null)
                return false;

            // Önce bu set'e ait tüm resimleri sil
            var images = await _context.Images
                .Where(i => i.ImageSetId == imageSetId)
                .ToListAsync();

            _context.Images.RemoveRange(images);

            // Sonra etiketleri sil
            var imageIds = images.Select(i => i.Id).ToList();
            var etiketler = await _context.EtiketlenenImages
                .Where(e => imageIds.Contains(e.ImageId))
                .ToListAsync();

            _context.EtiketlenenImages.RemoveRange(etiketler);

            // Son olarak ImageSet'i sil
            _context.ImageSets.Remove(imageSet);

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
