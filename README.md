# Personnel Tayin Talep Sistemi

## 📌 Proje Hakkında

**Personnel Tayin Talep Sistemi**, kurum içi personel tayin taleplerinin dijital ortamda yönetilmesini sağlayan bir web uygulamasıdır. Bu sistem, personel tayin süreçlerini daha şeffaf, izlenebilir ve verimli hale getirmeyi amaçlamaktadır.

## 🧰 Kullanılan Teknolojiler

- **Backend:** ASP.NET Core 8
- **Frontend:** HTML, CSS, JavaScript ve jQuery
- **Veritabanı:** MSSQL Server
- **ORM:** Entity Framework Core 8
- **Diğer:** Identity, Serilog, LINQ, Razor Pages

## 📁 Proje Yapısı

```
PersonnelTransferRequest/
├── PersonnelTransferRequest.Common/   # Ortak yardımcı sınıflar ve sabitler
├── PersonnelTransferRequest.Entities/ # Veritabanı modelleri Enum ve entity sınıfları
├── PersonnelTransferRequest.Web/      # Web uygulaması (UI, Services, Middleware controller'lar, view'lar)
├── PersonnelTransferRequest.sln       # Çözüm dosyası
├── .gitignore
└── README.md
```

## 🚀 Kurulum

1. **Projeyi Klonlayın:**

   ```bash
   git clone https://github.com/RasulSonmez/PersonnelTransferRequest.git
   cd PersonnelTransferRequest
   ```

2. **Veritabanını Yapılandırın:**

   - `appsettings.json` dosyasındaki bağlantı dizesini kendi SQL Server ayarlarınıza göre güncelleyin.

   ````json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=aspnet-PersonnelTransferRequestAppDB;Trusted_Connection=True;MultipleActiveResultSets=true"


   - Aşağıdaki komutla veritabanı migrasyonlarını uygulayın:

     ```bash
     dotnet ef database update
   ````

3. **Projeyi Çalıştırın:**

   ```bash
   dotnet run --project PersonnelTransferRequest.Web
   ```

   Uygulama varsayılan olarak `https://localhost:7299` adresinde çalışacaktır.

## 🧪 Özellikler

- 📝 Personel tayin taleplerinin oluşturulması ve takibi
- 🔐 Rol tabanlı kullanıcı yetkilendirme (personel, yönetici)
- 📄 Tayin taleplerinin onay/ret süreçleri
- 📊 Tayin geçmişi ve istatistikler
- 📬 E-posta bildirim sistemi (isteğe bağlı)
