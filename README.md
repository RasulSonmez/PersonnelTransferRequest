# PersonnelTayinRequest

## 📌 Proje Hakkında

**PersonnelTayinRequest**, kurum içi personel tayin taleplerinin dijital ortamda yönetilmesini sağlayan bir web uygulamasıdır. Bu sistem, personel tayin süreçlerini daha şeffaf, izlenebilir ve verimli hale getirmeyi amaçlamaktadır.

## 🧰 Kullanılan Teknolojiler

- **Backend:** ASP.NET Core
- **Frontend:** HTML, CSS, JavaScript
- **Veritabanı:** SQL Server
- **ORM:** Entity Framework Core
- **Diğer:** .NET 6+, LINQ, Razor Pages

## 📁 Proje Yapısı

```
PersonnelTransferRequest/
├── PersonnelTransferRequest.Common/   # Ortak yardımcı sınıflar ve sabitler
├── PersonnelTransferRequest.Entities/ # Veritabanı modelleri ve entity sınıfları
├── PersonnelTransferRequest.Web/      # Web uygulaması (UI, controller'lar, view'lar)
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
   - Aşağıdaki komutla veritabanı migrasyonlarını uygulayın:

     ```bash
     dotnet ef database update
     ```

3. **Projeyi Çalıştırın:**

   ```bash
   dotnet run --project PersonnelTransferRequest.Web
   ```

   Uygulama varsayılan olarak `https://localhost:5001` adresinde çalışacaktır.

## 🧪 Özellikler

- 📝 Personel tayin taleplerinin oluşturulması ve takibi
- 🔐 Rol tabanlı kullanıcı yetkilendirme (personel, yönetici, İK)
- 📄 Tayin taleplerinin onay/ret süreçleri
- 📊 Tayin geçmişi ve istatistikler
- 📬 E-posta bildirim sistemi (isteğe bağlı)

## 🧑‍💻 Katkıda Bulunma

Katkılarınızı memnuniyetle karşılıyoruz! Lütfen aşağıdaki adımları izleyin:

1. Bu repoyu fork'layın.
2. Yeni bir feature branch oluşturun:
   ```bash
   git checkout -b yeni-ozellik
   ```
3. Değişikliklerinizi commit'leyin:
   ```bash
   git commit -m "Yeni özellik eklendi"
   ```
4. Branch'inizi push'layın:
   ```bash
   git push origin yeni-ozellik
   ```
5. Bir Pull Request oluşturun.

## 📄 Lisans

Bu proje MIT Lisansı ile lisanslanmıştır. Daha fazla bilgi için [LICENSE](LICENSE) dosyasını inceleyebilirsiniz.

## 📬 İletişim

Herhangi bir soru veya öneriniz için lütfen [GitHub Issues](https://github.com/RasulSonmez/PersonnelTransferRequest/issues) üzerinden bizimle iletişime geçin.
