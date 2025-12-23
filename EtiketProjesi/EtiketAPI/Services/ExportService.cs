using EtiketAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;

namespace EtiketAPI.Services
{
    public class ExportService
    {
        private readonly EtiketDbContext _context;

        public ExportService(EtiketDbContext context)
        {
            _context = context;
        }

        // Class isimlerini ID'ye çevirmek için
        private Dictionary<string, int> classMapping = new Dictionary<string, int>();

        // ZIP dosyası oluştur (YOLO formatında)
        public async Task<byte[]?> CreateYoloZip(string paylasmKodu)
        {
            // 1. ImageSet'i bul
            var imageSet = await _context.ImageSets
                .FirstOrDefaultAsync(s => s.paylasmKodu == paylasmKodu);

            if (imageSet == null)
                return null;

            // 2. Resimleri getir
            var images = await _context.Images
                .Where(i => i.ImageSetId == imageSet.Id)
                .ToListAsync();

            if (!images.Any())  //hiç resim var mı dıye bakıyor yoksa  soruyu soyle soruyor imagesden hiç var mı yok dıyıp false donduruyor basına koydugum ! ile de true yapıp null donduruyoruz
                return null;

            // 3. Tüm etiket isimlerini topla ve ID ver
            var tumEtiketler = await _context.EtiketlenenImages
                .Where(e => images.Select(i => i.Id).Contains(e.ImageId))
                .Select(e => e.etiket)
                .Distinct()
                .ToListAsync();

            classMapping.Clear();
            for (int i = 0; i < tumEtiketler.Count; i++)
            {
                classMapping[tumEtiketler[i]] = i;
            }

            // 4. ZIP oluştur
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // images/ klasörü
                    foreach (var img in images)
                    {
                        // Resim dosyasını ekle
                        var imageEntry = archive.CreateEntry($"images/{img.Name}");
                        using (var entryStream = imageEntry.Open())
                        {
                            await entryStream.WriteAsync(img.image, 0, img.image.Length);
                        }

                        // Bu resmin etiketlerini getir
                        var etiketler = await _context.EtiketlenenImages
                            .Where(e => e.ImageId == img.Id)
                            .ToListAsync();

                        // YOLO formatında TXT oluştur
                        var txtContent = new StringBuilder();
                        foreach (var etiket in etiketler)
                        {
                            int classId = classMapping[etiket.etiket];

                            // YOLO format: class_id x_center y_center width height
                            txtContent.AppendLine(
                                $"{classId} {etiket.x_center:F6} {etiket.y_center:F6} {etiket.width:F6} {etiket.height:F6}"
                            );
                        }

                        // TXT dosyasını ekle
                        var txtFileName = Path.GetFileNameWithoutExtension(img.Name) + ".txt";
                        var txtEntry = archive.CreateEntry($"labels/{txtFileName}");
                        using (var entryStream = txtEntry.Open())
                        using (var writer = new StreamWriter(entryStream))
                        {
                            await writer.WriteAsync(txtContent.ToString());
                        }
                    }

                    // classes.txt dosyası oluştur (YOLO için class mapping)
                    var classesEntry = archive.CreateEntry("classes.txt");
                    using (var entryStream = classesEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        foreach (var kvp in classMapping.OrderBy(x => x.Value))
                        {
                            await writer.WriteLineAsync(kvp.Key);
                        }
                    }

                    // data.yaml dosyası oluştur (YOLO config)
                    var yamlEntry = archive.CreateEntry("data.yaml");
                    using (var entryStream = yamlEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        await writer.WriteLineAsync("# YOLO Dataset Config");
                        await writer.WriteLineAsync($"path: ../datasets/{paylasmKodu}");
                        await writer.WriteLineAsync("train: images");
                        await writer.WriteLineAsync("val: images");
                        await writer.WriteLineAsync();
                        await writer.WriteLineAsync($"nc: {classMapping.Count}");
                        await writer.WriteLineAsync($"names: [{string.Join(", ", classMapping.OrderBy(x => x.Value).Select(x => $"'{x.Key}'"))}]");
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}