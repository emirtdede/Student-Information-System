# 🎓 OKÜ Student Information System (Öğrenci Bilgi Sistemi - OBS)

[![.NET Core](https://img.shields.io/badge/.NET_Core-9.0-blue.svg?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-green.svg?style=flat-square&logo=sqlite)](https://www.sqlite.org/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.4-cyan.svg?style=flat-square&logo=tailwindcss)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-brightgreen.svg?style=flat-square)](LICENSE)

OKÜ Student Information System (OBS), bilgisayar mühendisliği öğrencileri ve akademik danışmanları için özel olarak tasarlanmış, yüksek bilgi yoğunluğuna sahip, modern ve responsive bir akademik portaldır. 

Bu proje, modern web tasarım trendlerine (bento grid yerleşimi, cam morfolojisi/glassmorphism, koyu/aydınlık tema entegrasyonu, yumuşak geçiş efektleri) ve endüstriyel standartta güvenli arka uç (back-end) yapılarına (PBKDF2 şifreleme, cookie tabanlı yerelleştirme senkronizasyonu) uygun olarak geliştirilmiştir.

---

## ✨ Temel Özellikler (Key Features)

### 👨‍🎓 Öğrenci Modülleri (Student Features)
- **Bento Grid Gösterge Paneli:** Akademik durum, GNO, AKTS, harç ve yemekhane bakiye durumlarını, yaklaşan ders programını ve duyuruları tek ekranda sunar.
- **Ders Kayıt Sistemi:** Anadal ve Çift Anadal (ÇAP) programları için çakışma, ön koşul kontrolü yapan gelişmiş seçim ve danışman onayına gönderme akışı.
- **Transkript ve Not Görüntüleme:** Dönemsel harf notları, anadal ve ÇAP transkript çıktıları ve not itiraz modülü.
- **İntibak ve Muafiyet Talepleri:** Kaldırılan veya başarısız olunan derslerin yerine güncel müfredattaki diğer dersleri saydırma talebi gönderimi.
- **Evrak Talepleri:** Öğrenci Belgesi ve Transkript gibi belgelerin kopya sayısı ve gerekçe girilerek talep edilmesi ve durum takibi.
- **Akademik İletişim:** Öğrenci ile danışman akademisyen arasında anlık mesajlaşma arayüzü.

### ⚙️ Tercihler ve Güvenlik (Preferences & Security)
- **Flicker-Free Koyu/Aydınlık Tema:** Sistem genelinde çerez (cookie) ve veritabanı senkronizasyonlu tema yönetimi. Sayfa yüklenirken ekran titremesi yaşanmaz.
- **Çok Dilli Altyapı (TR/EN):** Hem arayüz elemanları hem de sunucudan dönen hata/uyarı mesajları çerezlerden okunan dil tercihine göre dinamik olarak yerelleştirilir.
- **Kriptografik Parola Güvenliği:** Parolalar veritabanında düz metin yerine **PBKDF2** (HMAC-SHA256) algoritmalarıyla tuzlanarak (salted-hash) saklanır.

---

## 🛠️ Teknoloji Yığını (Tech Stack)

- **Framework:** ASP.NET Core 9.0 (MVC)
- **Database:** SQLite (`obs.db`)
- **ORM:** Entity Framework Core (EF Migrations)
- **Security:** ASP.NET Core Cookie Authentication & `PasswordHasher<ApplicationUser>` (PBKDF2)
- **Styling:** Tailwind CSS (loaded via CDN) & Custom CSS layer controls
- **Font & Icons:** Google Fonts (Inter) & Google Material Symbols

---

## 📂 Dosya Yapısı (File Structure)

```
Student-Information-System/
├── Controllers/                 # MVC Denetleyiciler (Rotalar ve API uçları)
├── Data/                        # DB Context, Veri Seeding ve Localization Helper
├── Migrations/                  # EF Core Veritabanı Göç Geçmişi
├── Models/                      # Model Sınıfları (Student, Advisor, Course vb.)
├── project_docs/                # Proje Teknik Dokümantasyonu (Vibe Docs)
├── Views/                       # Razor Görünümleri (.cshtml şablonları)
├── wwwroot/                     # Statik dosyalar (Görseller, özel stiller vb.)
├── obs.db                       # Yerel SQLite Veritabanı dosyası
├── Program.cs                   # Uygulama ayağa kalkış ve middleware yapılandırması
└── Student-Information-System.csproj
```

---

## 🚀 Kurulum ve Çalıştırma (Installation & Setup)

### Gereksinimler (Prerequisites)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) yüklü olmalıdır.

### Adımlar (Steps)

1. Projeyi klonlayın veya indirin:
   ```bash
   git clone https://github.com/emirtdede/Student-Information-System.git
   cd Student-Information-System
   ```

2. Gerekli bağımlılıkları yükleyin ve projeyi derleyin:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Veritabanı tablolarını göç geçmişine göre oluşturun (Eğer `obs.db` mevcut değilse):
   ```bash
   dotnet ef database update
   ```

4. Uygulamayı yerel olarak başlatın:
   ```bash
   dotnet run
   ```

5. Tarayıcınızdan konsolda yazan adrese (örneğin `http://localhost:5177`) giderek giriş yapın:
   - **Öğrenci Girişi:** Öğrenci No: `20240101001` | Şifre: `password`
   - **Danışman Girişi:** T.C. Kimlik No: `12345678900` | Şifre: `password`

---

## 📜 Lisans (License)
Bu proje **MIT** lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına göz atabilirsiniz.
