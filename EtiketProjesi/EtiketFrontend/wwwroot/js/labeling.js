// Canvas etiketleme için JavaScript fonksiyonları
// Bu dosya wwwroot/js/labeling.js olarak kaydedilmeli

let canvas, ctx, img;
let isDrawingMode = false;
let startX, startY, endX, endY;
let originalWidth, originalHeight;

// Canvas'ı başlat
window.initCanvas = function () {
    canvas = document.getElementById('labelCanvas');
    img = document.getElementById('labelImage');
    
    if (!canvas || !img) {
        console.error('Canvas veya image elementi bulunamadı');
        return;
    }

    ctx = canvas.getContext('2d');
    
    // Resim yüklendikten sonra canvas boyutunu ayarla
    img.onload = function() {
        canvas.width = img.width;
        canvas.height = img.height;
        originalWidth = img.naturalWidth;
        originalHeight = img.naturalHeight;
        console.log('Canvas hazır:', canvas.width, 'x', canvas.height);
    };
    
    // Eğer resim zaten yüklenmişse
    if (img.complete) {
        canvas.width = img.width;
        canvas.height = img.height;
        originalWidth = img.naturalWidth;
        originalHeight = img.naturalHeight;
    }
};

// Çizim modunu başlat
window.startDrawing = function () {
    return new Promise((resolve) => {
        if (!canvas || !ctx) {
            console.error('Canvas başlatılmamış');
            resolve(null);
            return;
        }

        isDrawingMode = true;
        let isDrawing = false;

        // Mouse down - Çizime başla
        const handleMouseDown = (e) => {
            if (!isDrawingMode) return;
            
            isDrawing = true;
            const rect = canvas.getBoundingClientRect();
            startX = e.clientX - rect.left;
            startY = e.clientY - rect.top;
        };

        // Mouse move - Dikdörtgen çiz
        const handleMouseMove = (e) => {
            if (!isDrawing || !isDrawingMode) return;
            
            const rect = canvas.getBoundingClientRect();
            endX = e.clientX - rect.left;
            endY = e.clientY - rect.top;
            
            // Canvas'ı temizle ve yeni dikdörtgeni çiz
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            redrawExistingLabels(); // Mevcut etiketleri yeniden çiz
            
            // Yeni dikdörtgen
            ctx.strokeStyle = '#00FF00';
            ctx.lineWidth = 2;
            ctx.strokeRect(startX, startY, endX - startX, endY - startY);
        };

        // Mouse up - Çizimi bitir
        const handleMouseUp = (e) => {
            if (!isDrawing || !isDrawingMode) return;
            
            isDrawing = false;
            isDrawingMode = false;
            
            const rect = canvas.getBoundingClientRect();
            endX = e.clientX - rect.left;
            endY = e.clientY - rect.top;
            
            // Event listener'ları temizle
            canvas.removeEventListener('mousedown', handleMouseDown);
            canvas.removeEventListener('mousemove', handleMouseMove);
            canvas.removeEventListener('mouseup', handleMouseUp);
            
            // YOLO formatına dönüştür (normalized 0-1 arası)
            const x = Math.min(startX, endX);
            const y = Math.min(startY, endY);
            const width = Math.abs(endX - startX);
            const height = Math.abs(endY - startY);
            
            if (width < 5 || height < 5) {
                console.log('Çok küçük dikdörtgen, iptal edildi');
                resolve(null);
                return;
            }
            
            // Normalize et (0-1 arası)
            const xCenter = (x + width / 2) / canvas.width;
            const yCenter = (y + height / 2) / canvas.height;
            const normWidth = width / canvas.width;
            const normHeight = height / canvas.height;
            
            // YOLO formatında döndür: "x_center,y_center,width,height"
            const result = `${xCenter},${yCenter},${normWidth},${normHeight}`;
            console.log('Etiket koordinatları:', result);
            resolve(result);
        };

        // Event listener'ları ekle
        canvas.addEventListener('mousedown', handleMouseDown);
        canvas.addEventListener('mousemove', handleMouseMove);
        canvas.addEventListener('mouseup', handleMouseUp);
        
        console.log('Çizim modu aktif - Dikdörtgen çizin');
    });
};

// Mevcut etiketleri saklama değişkeni
let existingLabels = [];

// Mevcut etiketleri çiz
window.drawExistingLabels = function (labelsJson) {
    if (!canvas || !ctx) {
        console.error('Canvas başlatılmamış');
        return;
    }
    
    try {
        existingLabels = JSON.parse(labelsJson);
        
        // Canvas'ı temizle
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        
        redrawExistingLabels();
        
        console.log('Etiketler çizildi:', existingLabels.length);
    } catch (error) {
        console.error('Etiket çizme hatası:', error);
    }
};

// Mevcut etiketleri yeniden çiz (helper fonksiyon)
function redrawExistingLabels() {
    if (!existingLabels || existingLabels.length === 0) return;
    
    existingLabels.forEach((label, index) => {
        // YOLO formatından pixel koordinatlarına dönüştür
        const xCenter = label.x_center * canvas.width;
        const yCenter = label.y_center * canvas.height;
        const width = label.width * canvas.width;
        const height = label.height * canvas.height;
        
        const x = xCenter - width / 2;
        const y = yCenter - height / 2;
        
        // Renk paleti (her etiket farklı renk)
        const colors = ['#FF0000', '#00FF00', '#0000FF', '#FFFF00', '#FF00FF', '#00FFFF'];
        const color = colors[index % colors.length];
        
        // Dikdörtgen çiz
        ctx.strokeStyle = color;
        ctx.lineWidth = 2;
        ctx.strokeRect(x, y, width, height);
        
        // Etiket adını yaz
        ctx.fillStyle = color;
        ctx.font = '14px Arial';
        ctx.fillText(label.etiket, x, y - 5);
    });
}

// Canvas'ı temizle
window.clearCanvas = function () {
    if (!canvas || !ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
};

console.log('Labeling.js yüklendi');
