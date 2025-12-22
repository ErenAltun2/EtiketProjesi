# 🏷️ Etiket - Image Labeling Platform

Modern ve kullanıcı dostu bir görsel etiketleme (image annotation) platformu. YOLO formatında veri seti oluşturmak için geliştirilmiştir.

## 📋 Özellikler

- ✅ **Kullanıcı Yönetimi**: Kayıt olma ve giriş yapma sistemi
- ✅ **Klasör Oluşturma**: Her proje için özel klasör oluşturma
- ✅ **Resim Yükleme**: Çoklu resim yükleme desteği
- ✅ **Paylaşım Kodu**: 6 haneli kod ile proje paylaşımı
- ✅ **Görsel Etiketleme**: Bounding box ile nesne etiketleme
- ✅ **YOLO Export**: Etiketlenmiş veriyi YOLO formatında indirme
- ✅ **Real-time Collaboration**: Ekip arkadaşlarınla aynı veri seti üzerinde çalışma

## 🛠️ Teknolojiler

### Backend
- **ASP.NET Core Web API** (.NET 8)
- **Entity Framework Core** (Code First)
- **SQL Server** / PostgreSQL / MySQL

### Frontend
- **Blazor Web App** (Interactive Server Mode)
- **C#** (Razor Components)
- **Bootstrap** / Tailwind CSS

### Shared
- **Class Library** (DTO'lar ve ortak modeller)

## 📁 Proje Yapısı

```
EtiketAPI/
├── Controllers/           # API endpoint'leri
│   ├── UserController.cs
│   ├── ImageController.cs
│   ├── EtiketController.cs
│   └── ExportController.cs
├── Services/             # Business logic
├── Data/                 # DbContext ve migrations
└── Program.cs

EtiketBlazor/
├── Services/             # API çağrıları için servisler
│   ├── AuthService.cs
│   ├── ImageService.cs
│   ├── EtiketService.cs
│   └── ExportService.cs
├── Pages/                # Razor sayfaları
├── Components/           # Yeniden kullanılabilir bileşenler
└── Program.cs

Shared/ClassLibrary/
├── DTOs/                 # Veri transfer objeleri
└── Models/               # Ortak modeller
```

## 🚀 Kurulum

### 1. Gereksinimler
- .NET 8 SDK
- SQL Server / PostgreSQL / MySQL
- Visual Studio 2022 veya VS Code

### 2. Veritabanı Kurulumu

```bash
# API projesine git
cd EtiketAPI

# Connection string'i appsettings.json'da düzenle
# Migrations oluştur
dotnet ef migrations add InitialCreate

# Veritabanını oluştur
dotnet ef database update
```

### 3. API'yi Çalıştır

```bash
cd EtiketAPI
dotnet run
```

API varsayılan olarak `https://localhost:7020` adresinde çalışacaktır.

### 4. Blazor Frontend'i Çalıştır

```bash
cd EtiketBlazor
dotnet run
```

Frontend varsayılan olarak `https://localhost:5001` adresinde çalışacaktır.

## 📖 Kullanım

### 1. Kayıt Ol ve Giriş Yap
- Sisteme kayıt olun
- Email ve şifrenizle giriş yapın

### 2. Klasör Oluştur
- "Yeni Klasör Oluştur" butonuna tıklayın
- Sistem size 6 haneli bir paylaşım kodu verecek
- Bu kodu not edin!

### 3. Resim Yükle
- Oluşturduğunuz klasöre birden fazla resim yükleyin
- Desteklenen formatlar: JPG, JPEG, PNG, GIF, BMP

### 4. Etiketle
- Resimlerin üzerine bounding box çizin
- Her nesneye etiket (label) verin
- Örnek: "drone", "uçak", "araba"

### 5. Paylaş
- Paylaşım kodunu arkadaşlarınıza gönderin
- Onlar da aynı veri seti üzerinde etiketleme yapabilir

### 6. Export Et
- "YOLO Export" butonuna tıklayın
- Etiketlenmiş veri setinizi ZIP olarak indirin
- Direkt olarak YOLOv5/YOLOv8 ile kullanabilirsiniz

## 🔧 API Endpoints

### User
```
POST   /api/user/register     # Kayıt ol
POST   /api/user/login        # Giriş yap
GET    /api/user/all          # Tüm kullanıcılar
DELETE /api/user/{id}         # Kullanıcı sil
```

### Image
```
POST   /api/image/create-set              # Klasör oluştur
POST   /api/image/upload                  # Resim yükle
GET    /api/image/kod/{paylasmaKodu}      # Kod ile resimleri getir
GET    /api/image/{imageId}               # ID ile resim getir
GET    /api/image/set-info/{paylasimKodu} # Klasör bilgisi
```

### Etiket
```
POST   /api/etiket/ekle              # Etiket ekle
GET    /api/etiket/image/{imageId}   # Resme ait etiketler
GET    /api/etiket/code/{kod}        # Koda ait etiketler
DELETE /api/etiket/{etiketId}        # Etiket sil
```

### Export
```
GET    /api/export/yolo/{paylasimKodu}  # YOLO formatında indir
```

## 🎯 YOLO Format

Export edilen ZIP dosyası şu yapıya sahiptir:

```
dataset_ABC123.zip
├── images/
│   ├── image1.jpg
│   ├── image2.jpg
│   └── ...
├── labels/
│   ├── image1.txt
│   ├── image2.txt
│   └── ...
├── data.yaml
└── classes.txt
```

Her `.txt` dosyası YOLO formatında:
```
0 0.5 0.5 0.3 0.2
1 0.7 0.3 0.15 0.1
```
Format: `class_id x_center y_center width height` (0-1 normalize edilmiş)

## 🤝 Katkıda Bulunma

1. Bu repo'yu fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👨‍💻 Geliştirici

**[Senin Adın]**
- GitHub: [@username](https://github.com/ErenAltun2)
- Email: eren.alltun@gmail.com

## 🙏 Teşekkürler

- YOLOv5 ve YOLOv8 topluluğuna
- Microsoft Blazor ekibine
- Tüm katkıda bulunanlara

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!
