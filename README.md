# 📦 Personel Tayin Talep Sistemi

## 📌 Proje Hakkında

**Personel Tayin Talep Sistemi**, kurum içi personel tayin taleplerinin dijital ortamda yönetilmesini sağlayan bir web uygulamasıdır. Bu sistem, personel tayin süreçlerini daha şeffaf, izlenebilir ve verimli hale getirmeyi amaçlamaktadır.

## 🧰 Kullanılan Teknolojiler

- ⚙️ **Backend:** ASP.NET Core 8
- 🎨 **Frontend:** HTML, CSS, Bootstrap, JavaScript ve jQuery
- 🗄️ **Veritabanı:** MSSQL Server
- 🔄 **ORM:** Entity Framework Core 8
- 🧩 **Diğer:** Identity, Serilog, LINQ, Razor Pages

## 📁 Proje Yapısı

```
PersonnelTransferRequest/
├── PersonnelTransferRequest.Common/   # Ortak yardımcı sınıflar ve sabitler
├── PersonnelTransferRequest.Entities/ # Veritabanı modelleri Enum ve entity sınıfları
├── PersonnelTransferRequest.Web/      # Web uygulaması (UI, Services, Middleware ve controller'lar, view'lar)
├── PersonnelTransferRequest.sln       # Çözüm dosyası
├── .gitignore
└── README.md
```

## 🚀 Kurulum

1. 🧬 **Projeyi Klonlayın:**

   ```bash
   git clone https://github.com/RasulSonmez/PersonnelTransferRequest.git
   cd PersonnelTransferRequest
   ```

2. 🛠️ **Veritabanını Yapılandırın:**

   - `appsettings.json` dosyasındaki ConnectionStrings dizesini kendi SQL Server ayarlarınıza göre güncelleyin:

     ```json
     "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=aspnet-PersonnelTransferRequestAppDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     ```

   - Aşağıdaki komutla veritabanı migrasyonlarını uygulayın:

     ```bash
     dotnet ef database update
     ```

3. ▶️ **Projeyi Çalıştırın:**

   ```bash
   dotnet run --project PersonnelTransferRequest.Web
   ```

   Uygulama varsayılan olarak `https://localhost:7299` adresinde çalışacaktır.

## 🧪 Özellikler

### 👨‍💼 Admin Tarafı

- 🔐 İlk admin [`/adminRegister`](https://localhost:7299/adminRegister) sayfasından kayıt olur, admin rolü verilir ve sayfa kapatılır.
- 🧭 [`/admin`](https://localhost:7299/admin) üzerinden admin paneline giriş yapılır.
- 📊 Dashboard’da özet bilgiler görüntülenir.
- 👥 **Tüm Personeller:** Kayıtlı personel listesinin yönetimi.
- 📁 **Tayin Başvuruları:**
  - Taleplerin açılıp kapatılması (dönemsel yapı)
  - Başvuran personelin tercihleriyle birlikte tüm bilgileri
  - Taleplerin onaylanması veya reddedilmesi
- 🏷️ **Unvan Yönetimi:**
  - Kurumdaki unvanlar tanımlanır.
  - Kayıt sırasında sadece tanımlı unvanların seçilmesi sağlanır.
- 🔑 **Şifre Değiştirme:** Admin şifresini değiştirebilir.
- 💬 **Destek Kayıtları:**
  - Personelin destek talepleri
  - Durum takibi ve cevap yazılması

### 👤 Personel Tarafı

- 🔐 [`/giris-yap`](https://localhost:7299/giris-yap): Sisteme sicil ve şifre ile giriş.
- 📝 [`/kayit-ol`](https://localhost:7299/kayit-ol): Sicil, isim-soyisim, unvan, TCKN, telefon, görev yeri, e-posta, şifre ve fotoğraf ile kayıt.
- 🪪 Giriş sonrası kişisel bilgilerin olduğu profil kartı.
- 📄 **Tayin Taleplerim:** Geçmiş başvurular ve durum bilgileri.
- 📨 **Tayin Talebi Oluşturma:** Tayin Nedeni, Açıklama + 1-5 tercih arası il seçimi.
- 🆘 **Destek:** Sağ üst profilden destek kaydı açma ve takip.
- 🔒 **Şifre Değiştirme:** Profilden şifre güncelleme.

---

## ➕ Ek Özellikler

- 🔐 **ASP.NET Core Identity** ile şifre yönetimi, "Şifremi Unuttum" ve "Şifre Sıfırlama" modülleri.
- 📧 **Email Helper**: `appsettings.json` içine SMTP bilgileri girildiğinde otomatik e-posta gönderimi.
- 📊 **DataTable**: Admin tarafındaki listeleme sayfaları için dinamik server-side DataTable servisi.
- 📋 **Serilog Loglama**:
  - Personel tarafında action bazlı loglama.
  - Admin tarafı için özel Middleware ile isteklerin loglanması.
  - 14 günde bir logların temizlenmesi.

## 🔐 Güvenlik Önlemleri

- **Form Validasyonu:**  
  Tüm formlarda hem istemci tarafında (HTML5 `required`, `type`, `maxlength` vb.) hem sunucu tarafında model doğrulama (`[Required]`, `[StringLength]`, `[EmailAddress]` vb.) ile veri güvenliği sağlanmıştır.

- **Anti-Forgery Token:**  
  Cross-Site Request Forgery (CSRF) saldırılarına karşı ASP.NET Core’un yerleşik `@Html.AntiForgeryToken()` özelliği kullanılarak formlarda güvenlik artırılmıştır.

- **Harici Kaynak Yönetimi:**  
  Uygulamada CDN kullanımı tercih edilmemiş, tüm kütüphaneler yerel olarak host edilmiştir. Bu sayede dış kaynaklardan doğabilecek güvenlik açıkları önlenmiştir.

- **Kimlik Doğrulama ve Yetkilendirme:**  
  ASP.NET Core Identity kullanılarak kullanıcı kimlik doğrulaması sağlanmış, roller bazında (örn. `Admin`) yetkilendirme uygulanmıştır.

- **Şifre Politikası:**  
  Kullanıcı şifreleri için aşağıdaki kurallar zorunlu kılınmıştır:

  - En az bir büyük harf
  - En az bir küçük harf
  - En az bir rakam
  - En az bir özel karakter
  - Minimum şifre uzunluğu: 6 karakter

- **Hassas Verilerin Korunması:**  
  Bağlantı stringleri ve SMTP gibi hassas bilgiler `appsettings.json` içinde tutulmakta, dışa açık commit'lerde paylaşılmamaktadır.

- **Loglama:**  
  Serilog kullanılarak hata ve kullanıcı işlemleri loglanmaktadır.

  - Personel tarafında: Action bazlı loglama
  - Admin tarafında: Custom Middleware ile kapsamlı log kaydı tutulmaktadır.

- **Kapsamlı Rol Yönetimi:**  
  Admin kullanıcıları yalnızca özel olarak atanmakta ve sistem üzerinde yeni admin oluşturulması sınırlandırılmıştır.

---

Not: Proje içerisindeki tüm önemli sınıflara ve metotlara açıklayıcı yorum satırları (summary etiketleri) eklenmiştir. Bu sayede kod okunabilirliği ve geliştirici deneyimi artırılmıştır.

## 🔑 Örnek Giriş Bilgileri

### 👤 Yönetici (Admin)

- **E-posta:** `admin@example.com`
- **Şifre:** `Pa$$w0rd!`

### 👨‍💼 Personel

- **Sicil No:** `188187`
- **Şifre:** `Adalet34.`
