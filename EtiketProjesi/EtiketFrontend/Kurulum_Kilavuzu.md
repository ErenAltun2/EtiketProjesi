# 🎯 Etiket Projesi - Tüm Sayfalar Kurulum Kılavuzu

## 📦 Hazırlanan Sayfalar

### ✅ Tamamlanan Sayfalar:

1. **Home.razor** - Ana sayfa (giriş yapmış/yapmamış versiyonları)
2. **Login.razor** - Giriş sayfası ✅ (Zaten var)
3. **Register.razor** - Kayıt sayfası ✅ (Zaten var)
4. **ProjeOlustur.razor** - Yeni proje/ImageSet oluşturma
5. **ResimYukle.razor** - Resimleri yükleme
6. **KodIleAc.razor** - Paylaşım kodu ile projeye erişim
7. **Resimler.razor** - Projedeki tüm resimleri görüntüleme
8. **Etiketleme.razor** - Ana etiketleme sayfası (Canvas ile bounding box çizimi)
9. **Export.razor** - YOLO dataset ZIP indirme

### 📁 JavaScript Dosyaları:

1. **labeling.js** - Canvas etiketleme fonksiyonları
2. **download.js** - Dosya indirme fonksiyonu

---

## 🚀 Kurulum Adımları

### 1️⃣ Sayfaları Kopyala

Tüm `.razor` dosyalarını projenize kopyalayın:

```
EtiketFrontend/
└── Components/
    └── Pages/
        ├── Home.razor (YENİ - mevcut olanı değiştir)
        ├── Login.razor (ZATEN VAR)
        ├── Register.razor (ZATEN VAR)
        ├── ProjeOlustur.razor (YENİ)
        ├── ResimYukle.razor (YENİ)
        ├── KodIleAc.razor (YENİ)
        ├── Resimler.razor (YENİ)
        ├── Etiketleme.razor (YENİ)
        └── Export.razor (YENİ)
```

### 2️⃣ JavaScript Dosyalarını Ekle

JavaScript dosyalarını `wwwroot/js/` klasörüne kopyalayın:

```
EtiketFrontend/
└── wwwroot/
    └── js/
        ├── labeling.js (YENİ)
        └── download.js (YENİ)
```

Eğer `wwwroot/js/` klasörü yoksa oluşturun:

```bash
mkdir -p wwwroot/js
```

### 3️⃣ JavaScript Dosyalarını App.razor'a Ekle

`Components/App.razor` dosyasını açın ve `</body>` etiketinden **önce** şunları ekleyin:

```html
<!DOCTYPE html>
<html lang="tr">
<head>
    <!-- ... mevcut head içeriği ... -->
</head>
<body>
    <CascadingAuthenticationState>
        <Routes />
    </CascadingAuthenticationState>
    
    <script src="_framework/blazor.web.js"></script>
    
    <!-- ⭐ BUNLARI EKLEYİN ⭐ -->
    <script src="js/labeling.js"></script>
    <script src="js/download.js"></script>
</body>
</html>
```

---

## 🎯 Sayfa Açıklamaları ve Kullanım

### 📄 **Home.razor** - Ana Sayfa
**Route:** `/`
**Koruma:** Yok (herkese açık)

**Özellikler:**
- Giriş yapmış kullanıcılar için: Hızlı erişim kartları (Proje Oluştur, Kod ile Aç, Export)
- Giriş yapmamış kullanıcılar için: Tanıtım sayfası

---

### 📄 **ProjeOlustur.razor** - Yeni Proje
**Route:** `/proje-olustur`
**Koruma:** ✅ `@attribute [Authorize]` (sadece giriş yapanlar)

**Ne Yapar:**
1. Kullanıcı "Proje Oluştur" butonuna basar
2. API'ye `CreateImageSet` isteği gönderilir
3. Benzersiz 6 haneli paylaşım kodu oluşturulur
4. Kullanıcıya paylaşım kodu gösterilir
5. "Resim Yüklemeye Geç" butonu ile sonraki adıma geçiş

**API Çağrısı:**
```csharp
POST /api/Image/create-set
Body: { "userId": 5 }
Response: { "imageSetId": 10, "paylasmaKodu": "123456", "olusturmaTarihi": "..." }
```

---

### 📄 **ResimYukle.razor** - Resim Yükleme
**Route:** `/resim-yukle?imageSetId=10&kod=123456`
**Koruma:** ✅ `@attribute [Authorize]`

**Ne Yapar:**
1. Kullanıcı birden fazla resim seçer
2. Dosya boyutu ve format kontrolü yapılır
3. "Resimleri Yükle" butonuna basılır
4. Resimler API'ye gönderilir
5. Başarılı olursa "Etiketlemeye Geç" butonu aktif olur

**API Çağrısı:**
```csharp
POST /api/Image/upload
FormData: imageSetId=10, images=[file1, file2, ...]
```

---

### 📄 **KodIleAc.razor** - Kod ile Projeye Erişim
**Route:** `/kod-ile-ac`
**Koruma:** ✅ `@attribute [Authorize]`

**Ne Yapar:**
1. Kullanıcı 6 haneli paylaşım kodunu girer
2. API'den proje bilgileri çekilir
3. Proje bulunursa:
   - "Resimleri Görüntüle" → `/resimler?kod=123456`
   - "Etiketleme Sayfasına Git" → `/etiketleme?kod=123456`

**API Çağrısı:**
```csharp
GET /api/Image/set-info/123456
Response: { "id": 10, "paylasmKodu": "123456", "resimSayisi": 5, ... }
```

---

### 📄 **Resimler.razor** - Resim Galerisi
**Route:** `/resimler?kod=123456`
**Koruma:** ✅ `@attribute [Authorize]`

**Ne Yapar:**
1. Paylaşım koduna ait tüm resimleri gösterir
2. Her resim için thumbnail (küçük resim) gösterir
3. Resme tıklandığında etiketleme sayfasına yönlendirir

**API Çağrısı:**
```csharp
GET /api/Image/kod/123456
Response: { "resimler": [{ "id": 1, "name": "...", "imageBase64": "..." }, ...] }
```

---

### 📄 **Etiketleme.razor** - Ana Etiketleme Sayfası
**Route:** `/etiketleme?kod=123456&imageId=1`
**Koruma:** ✅ `@attribute [Authorize]`

**Ne Yapar:**
1. **Sol Panel:** Tüm resimlerin thumbnail'leri (resim seçimi için)
2. **Orta Panel:** Seçili resim üzerinde canvas ile bounding box çizimi
3. **Sağ Panel:** O resme ait mevcut etiketler listesi

**Etiketleme İşlemi:**
1. Etiket adı gir (örn: "drone")
2. "Çizmeye Başla" butonuna bas
3. Mouse ile dikdörtgen çiz
4. Otomatik olarak YOLO formatında kaydedilir

**API Çağrıları:**
```csharp
// Resimleri al
GET /api/Image/kod/123456

// Etiketleri al
GET /api/Etiket/image/1

// Etiket ekle
POST /api/Etiket/ekle
Body: { "userId": 5, "imageId": 1, "etiket": "drone", "xCenter": 0.5, ... }

// Etiket sil
DELETE /api/Etiket/5
```

---

### 📄 **Export.razor** - Dataset İndirme
**Route:** `/export`
**Koruma:** ✅ `@attribute [Authorize]`

**Ne Yapar:**
1. Kullanıcı paylaşım kodunu girer
2. "ZIP Dosyası İndir" butonuna basar
3. API'den YOLO formatında dataset indirilir

**ZIP İçeriği:**
```
dataset_123456.zip
├── images/
│   ├── image1.jpg
│   └── image2.jpg
├── labels/
│   ├── image1.txt (YOLO format)
│   └── image2.txt
├── classes.txt (sınıf isimleri)
└── data.yaml (YOLO config)
```

**API Çağrısı:**
```csharp
GET /api/Export/yolo/123456
Response: ZIP file (byte[])
```

---

## 🔗 Navigasyon Akışı

```
Ana Sayfa (/)
    ↓
    ├─→ Yeni Proje Oluştur (/proje-olustur)
    │       ↓
    │   Resim Yükle (/resim-yukle?imageSetId=10&kod=123456)
    │       ↓
    │   Etiketleme (/etiketleme?kod=123456)
    │       ↓
    │   Export (/export) → ZIP İndir
    │
    ├─→ Kod ile Aç (/kod-ile-ac)
    │       ↓
    │   Resimler Görüntüle (/resimler?kod=123456)
    │       ↓
    │   Etiketleme (/etiketleme?kod=123456&imageId=1)
    │
    └─→ Export (/export) → ZIP İndir
```

---

## ✅ Kontrol Listesi

Kurulumu tamamladıktan sonra şunları kontrol edin:

- [ ] Tüm `.razor` dosyaları `Components/Pages/` klasöründe
- [ ] `labeling.js` ve `download.js` dosyaları `wwwroot/js/` klasöründe
- [ ] `App.razor`'da JavaScript dosyaları yükleniyor
- [ ] `Program.cs`'de authentication servisleri kayıtlı
- [ ] API çalışıyor (`https://localhost:7267`)
- [ ] Frontend çalışıyor

---

## 🧪 Test Adımları

### 1. Proje Oluşturma Testi
```
1. /login → Giriş yap
2. / → Ana sayfa → "Yeni Proje Oluştur"
3. /proje-olustur → "Proje Oluştur" butonu
4. Paylaşım kodunu not et (örn: 123456)
```

### 2. Resim Yükleme Testi
```
1. "Resim Yüklemeye Geç" butonu
2. /resim-yukle → Resimleri seç
3. "Resimleri Yükle" butonu
4. Başarılı mesajı gelmeli
```

### 3. Etiketleme Testi
```
1. "Etiketlemeye Geç" butonu
2. /etiketleme → Sol panelden resim seç
3. Etiket adı gir (örn: "drone")
4. "Çizmeye Başla" → Mouse ile dikdörtgen çiz
5. Sağ panelde etiketin görünmesi gerekir
```

### 4. Kod ile Açma Testi
```
1. Yeni sekme aç → /kod-ile-ac
2. Paylaşım kodunu gir (123456)
3. "Projeyi Bul" butonu 
4. Proje bilgileri görünmeli
5. "Resimleri Görüntüle" veya "Etiketlemeye Git"
```

### 5. Export Testi
```
1. /export
2. Paylaşım kodunu gir (123456)
3. "ZIP Dosyası İndir" butonu
4. dataset_123456.zip indirilmeli
5. ZIP'i aç → images/, labels/, classes.txt, data.yaml olmalı
```

---

## 🐛 Sık Karşılaşılan Sorunlar

### Sorun 1: "Canvas başlatılmamış" hatası
**Çözüm:** `labeling.js` dosyasının `App.razor`'da yüklendiğinden emin olun.

### Sorun 2: Resimler görünmüyor
**Çözüm:** API'nin çalıştığından ve CORS ayarlarının doğru olduğundan emin olun.

### Sorun 3: Etiket kaydedilmiyor
**Çözüm:** Kullanıcı ID'nin doğru alındığından emin olun (AuthStateProvider).

### Sorun 4: ZIP indirilmiyor
**Çözüm:** `download.js` dosyasının yüklendiğini kontrol edin.

---

## 🎉 Tebrikler!

Artık tam fonksiyonel bir YOLO etiketleme sisteminiz var! 

**Özellikler:**
✅ Kullanıcı girişi ve kayıt
✅ Proje oluşturma
✅ Toplu resim yükleme
✅ Canvas ile bounding box çizimi
✅ YOLO formatında etiketleme
✅ Paylaşım kodu ile işbirliği
✅ YOLO dataset ZIP export

**Sonraki Adımlar:**
- Etiketlenen datasetleri YOLOv8/YOLOv5 ile eğitin
- Model performansını test edin
- Daha fazla resim ekleyin ve etiketleyin
