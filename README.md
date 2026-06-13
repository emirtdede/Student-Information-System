# 🎓 OKÜ Student Information System (Öğrenci Bilgi Sistemi - OBS)

[![.NET Core](https://img.shields.io/badge/.NET_Core-9.0-blue.svg?style=flat-square&logo=.net)](https://dotnet.microsoft.com/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-green.svg?style=flat-square&logo=sqlite)](https://www.sqlite.org/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.4-cyan.svg?style=flat-square&logo=tailwindcss)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-brightgreen.svg?style=flat-square)](LICENSE)

---

## 🇺🇸 English Version

OKÜ Student Information System (OBS) is a high-density, modern, and fully responsive academic portal designed for computer engineering students and academic advisors. 

This project is built following modern web design trends (bento grid layout, glassmorphism, native dark/light theme integration, smooth micro-interactions) and industry-standard secure back-end patterns (PBKDF2 security hashing, cookie-based localization synchronization).

---

### ✨ Key Features

#### 👨‍🎓 Student Modules
- **Bento Grid Dashboard:** Displays academic status, GPA, ECTS, tuition fees, dining hall balance, upcoming course schedules, and recent announcements in a single clean layout.
- **Course Registration:** An advanced system for Major and Double Major (CAP) registration that validates schedule conflicts and course prerequisites.
- **Transcripts & Objections:** Allows viewing of semester letter grades, transcripts for major/CAP, and filing objections to exam grades.
- **Exemptions & Substitutions:** Request mapping of old/failed courses to new curriculum courses.
- **Document Requests:** File requests for official papers like Student Certificates or Transcripts with copy counts.
- **Messaging Portal:** Direct messaging inbox between students and advisors.

#### ⚙️ Preferences & Security
- **Flicker-Free Theme Toggling:** Syncs dark/light mode dynamically through database properties and HTTP cookies to eliminate loading flickers.
- **Bilingual Interface (TR/EN):** Localizes both UI components and server validation error messages based on active cookies.
- **Cryptographic Passwords:** Passwords are securely stored in the SQLite database using **PBKDF2** (HMAC-SHA256) salted hashes.

---

### kur 🛠️ Tech Stack
- **Framework:** ASP.NET Core 9.0 (MVC)
- **Database:** SQLite (`obs.db`)
- **ORM:** Entity Framework Core (EF Migrations)
- **Security:** ASP.NET Core Cookie Authentication & `PasswordHasher<ApplicationUser>` (PBKDF2)
- **Styling:** Tailwind CSS (via CDN) & custom CSS utility variables
- **Font & Icons:** Google Fonts (Inter) & Google Material Symbols

---

### 🚀 Setup & Execution

#### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) must be installed.

#### Steps

1. Clone or download the repository:
   ```bash
   git clone https://github.com/emirtdede/Student-Information-System.git
   cd Student-Information-System
   ```

2. Restore dependencies and build:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Update database schema (if `obs.db` is not present):
   ```bash
   dotnet ef database update
   ```

4. Start the application locally:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to the address shown in the console (e.g., `http://localhost:5177`):
   - **Student Access:** Student No: `20240101001` | Password: `password`
   - **Advisor Access:** T.C. ID No: `12345678900` | Password: `password`

---

## 🇹🇷 Türkçe Versiyon

OKÜ Öğrenci Bilgi Sistemi (OBS), bilgisayar mühendisliği öğrencileri ve akademik danışmanları için özel olarak tasarlanmış, yüksek bilgi yoğunluğuna sahip, modern ve responsive bir akademik portaldır. 

Bu proje, modern web tasarım trendlerine (bento grid yerleşimi, cam morfolojisi/glassmorphism, koyu/aydınlık tema entegrasyonu, yumuşak geçiş efektleri) ve endüstriyel standartta güvenli arka uç (back-end) yapılarına (PBKDF2 şifreleme, cookie tabanlı yerelleştirme senkronizasyonu) uygun olarak geliştirilmiştir.

---

### ✨ Temel Özellikler

#### 👨‍🎓 Öğrenci Modülleri
- **Bento Grid Gösterge Paneli:** Akademik durum, GNO, AKTS, harç ve yemekhane bakiye durumlarını, yaklaşan ders programını ve duyuruları tek ekranda sunar.
- **Ders Kayıt Sistemi:** Anadal ve Çift Anadal (ÇAP) programları için çakışma, ön koşul kontrolü yapan gelişmiş seçim ve danışman onayına gönderme akışı.
- **Transkript ve Not Görüntüleme:** Dönemsel harf notları, anadal ve ÇAP transkript çıktıları ve not itiraz modülü.
- **İntibak ve Muafiyet Talepleri:** Kaldırılan veya başarısız olunan derslerin yerine güncel müfredattaki diğer dersleri saydırma talebi gönderimi.
- **Evrak Talepleri:** Öğrenci Belgesi ve Transkript gibi belgelerin kopya sayısı ve gerekçe girilerek talep edilmesi ve durum takibi.
- **Akademik İletişim:** Öğrenci ile danışman akademisyen arasında anlık mesajlaşma arayüzü.

#### ⚙️ Tercihler ve Güvenlik
- **Flicker-Free Koyu/Aydınlık Tema:** Sistem genelinde çerez (cookie) ve veritabanı senkronizasyonlu tema yönetimi. Sayfa yüklenirken ekran titremesi yaşanmaz.
- **Çok Dilli Altyapı (TR/EN):** Hem arayüz elemanları hem de sunucudan dönen hata/uyarı mesajları çerezlerden okunan dil tercihine göre dinamik olarak yerelleştirilir.
- **Kriptografik Parola Güvenliği:** Parolalar veritabanında düz metin yerine **PBKDF2** (HMAC-SHA256) algoritmalarıyla tuzlanarak (salted-hash) saklanır.

---

### 🛠️ Teknoloji Yığını
- **Framework:** ASP.NET Core 9.0 (MVC)
- **Database:** SQLite (`obs.db`)
- **ORM:** Entity Framework Core (EF Migrations)
- **Security:** ASP.NET Core Cookie Authentication & `PasswordHasher<ApplicationUser>` (PBKDF2)
- **Styling:** Tailwind CSS (CDN üzerinden) & Özel CSS katman kontrolleri
- **Font & Icons:** Google Fonts (Inter) & Google Material Symbols

---

### 🚀 Kurulum ve Çalıştırma

#### Gereksinimler
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) yüklü olmalıdır.

#### Adımlar

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
