// Dosya indirme fonksiyonu
// Bu dosya wwwroot/js/download.js olarak kaydedilmeli

window.downloadFile = function (filename, base64Data) {
    // Base64'ü binary'ye çevir
    const binaryString = window.atob(base64Data);
    const bytes = new Uint8Array(binaryString.length);
    
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    
    // Blob oluştur
    const blob = new Blob([bytes], { type: 'application/zip' });
    const url = window.URL.createObjectURL(blob);
    
    // İndirme linki oluştur ve tıkla
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    
    // Temizlik
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
    
    console.log('Dosya indirildi:', filename);
};

console.log('Download.js yüklendi');
