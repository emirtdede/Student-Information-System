using System.Collections.Generic;

namespace Student_Information_System.Data
{
    public static class LocalizationHelper
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            {
                "tr", new Dictionary<string, string>
                {
                    { "InvalidCredentials", "Geçersiz öğrenci numarası/e-posta veya şifre." },
                    { "UserNotFound", "Kullanıcı bulunamadı!" },
                    { "PasswordUpdated", "Şifreniz başarıyla güncellendi." },
                    { "CurrentPasswordIncorrect", "Mevcut şifreniz yanlış!" },
                    { "PasswordsDoNotMatch", "Yeni şifreler eşleşmiyor!" },
                    { "PasswordTooShort", "Şifre en az 6 karakter olmalıdır!" },
                    { "PreferencesUpdated", "Tercihleriniz başarıyla güncellendi." },
                    { "ErrorOccurred", "Bir hata oluştu. Lütfen tekrar deneyin." },
                    { "StudentIdRequired", "Öğrenci numarası veya e-posta boş bırakılamaz." },
                    { "FillAllFields", "Lütfen tüm alanları doldurunuz." },
                    { "UseInstitutionalEmail", "Lütfen kurumsal e-posta adresinizi giriniz (@ogr.oku.edu.tr ile bitmelidir)." },
                    { "AlreadyRegistered", "Bu öğrenci numarası veya e-posta zaten kayıtlı." }
                }
            },
            {
                "en", new Dictionary<string, string>
                {
                    { "InvalidCredentials", "Invalid student number/email or password." },
                    { "UserNotFound", "User not found!" },
                    { "PasswordUpdated", "Your password has been successfully updated." },
                    { "CurrentPasswordIncorrect", "Current password is incorrect!" },
                    { "PasswordsDoNotMatch", "New passwords do not match!" },
                    { "PasswordTooShort", "Password must be at least 6 characters long!" },
                    { "PreferencesUpdated", "Your preferences have been successfully updated." },
                    { "ErrorOccurred", "An error occurred. Please try again." },
                    { "StudentIdRequired", "Student number or email cannot be empty." },
                    { "FillAllFields", "Please fill in all fields." },
                    { "UseInstitutionalEmail", "Please enter your institutional email address (must end with @ogr.oku.edu.tr)." },
                    { "AlreadyRegistered", "This student number or email is already registered." }
                }
            }
        };

        public static string Get(string key, string lang = "tr")
        {
            var normalizedLang = (lang ?? "tr").ToLower().Substring(0, 2);
            if (normalizedLang != "tr" && normalizedLang != "en")
            {
                normalizedLang = "tr";
            }

            if (Translations.TryGetValue(normalizedLang, out var langDict) && langDict.TryGetValue(key, out var translation))
            {
                return translation;
            }

            // Fallback to Turkish if translation not found
            if (Translations["tr"].TryGetValue(key, out var fallbackTranslation))
            {
                return fallbackTranslation;
            }

            return key;
        }
    }
}
