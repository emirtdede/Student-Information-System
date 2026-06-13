# Project Memory: OKÜ OBS

**Son Güncelleme:** 2026-06-13

## Proje Durumu
Proje yayına hazır durumdadır. Tüm testler (derleme, kimlik doğrulama, şifreleme, yerelleştirme, arayüz uyumluluğu) başarıyla tamamlanmıştır.

## Son Yapılan İşler
- `SecurityHelper` eklenerek şifreler için PBKDF2 tabanlı güvenli karma (hashing) yapısı kuruldu.
- `AuthController`, `AdvisorAuthController` ve `SettingsController` şifre doğrulama ve saklama akışları güvenli hale getirildi.
- Geriye dönük uyumluluk (düz metin şifreler için geçiş desteği) sağlandı.
- Test senaryoları `TESTING_STATUS.md` dosyasına eklenerek tamamı doğrulandı.

## Sıradaki Adımlar
- Canlı sunucu ortamında (IIS, Docker veya Cloud) dağıtım (deployment) adımlarının planlanması.
- Öğretim görevlileri ve yöneticiler için gelişmiş yönetim araçlarının eklenmesi.
