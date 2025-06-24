# WebApp_Demo-master

# WebAppDemo_update (.NET Core MVC UI)

Bu proje, oluşturmuş olduğum [`JwtToken_Project`](https://github.com/oykugurbuz/JwtToken_Project) (kullanıcı kimlik doğrulama ve yetkilendirme işlemlerini JWT (JSON Web Token) kullanarak gerçekleştiren bir ASP.NET Core Web API uygulaması) ile entegre olarak çalışır.  Kullanıcıların giriş yaparak ürün/kategori CRUD işlemleri yapmasını sağlar ve DevExpress bileşenleri ile zenginleştirilmiştir. Tarih, fiyat ve kategori filtreli raporlama yapar.CRUD işlemleri için kullanıcı bazlı yetkilendirme (okuma, ekleme, güncelleme, silme) yapar. 

##Özellikler

- Kullanıcı kimlik doğrulama  (API üzerinden JWT ile)
- JWT token cookie içerisinde (HTTPOnly flag ile) saklanır
- Ürün ve Kategori CRUD işlemleri
- Kullanıcı bazlı yetkilendirme (okuma, ekleme, güncelleme, silme)
- Kullanıcıya özel **authority level** tanımı
-  Action bazlı `[HasPermission]` attribute ile işlem kontrolü
- Kategori, tarih ve fiyat filtreli gelişmiş raporlama
- PDF rapor önizleme
- Raporların iframe üzerinden sayfa içine gömülmesi, dışa aktarma
- DevExpress bileşenleri ile ile zengin arayüz

##JWT

- Kullanıcı, API aracılığıyla giriş yaptığında sunucudan bir **JWT Token** döner.
- Bu token, **cookie** içerisine (HTTPOnly flag ile) kaydedilir.
- Cookie tarayıcıda saklandığı için token güvenli bir şekilde taşınır; JavaScript erişemez.
- Her istek otomatik olarak bu token ile yapılır; kullanıcı kimliği korunur.

##Yetki Tabanlı İşlem İzni

Kullanıcıların yetkileri, veritabanında tanımlı olan **Module**,**Permission** ve **UserPermission** tabloları üzerinden kontrol edilir.

Özel bir `[HasPermission]` attribute sınıfı ile action seviyesinde erişim denetimi sağlanır.

### Örnek kullanım:
```csharp
[HasPermission("Product.Create")]
public IActionResult CreateProduct() { 
     // yalnızca bu yetkiye sahip kullanıcılar erişebilir
 }
 ```
### Sistem Nasıl Çalışır:

Kullanıcının UserName bilgisi HttpContext.User.Identity.Name ile alınır

Kullanıcının aktif izinleri UserPermissions üzerinden sorgulanır

Yetki "Modül.Action" formatında kontrol edilir

Erişimi olmayan kullanıcıya 403 HTTP hatası döner


## Kullanılan Teknolojiler

-ASP.NET Core MVC (.NET 8)

-Entity Framework Core

-DevExpress JavaScript & Reporting v24.2+

-JWT Authentication (cookie tabanlı)

-Custom Attribute ile Yetkilendirme

-Session bazlı filtreleme & raporlama

-jQuery, AJAX, PartialView mimarisi

### Projeyi Çalıştırmak İçin

   1) Bu repoyu klonlayın: git clone https://github.com/oykugurbuz/WebAppDemo_update
   2) API projesi (JwtToken_Project) çalışır durumda olmalıdır.
   3) Gerekli appsettings.json ayarlarını yapın ve Program.cs içinde API base adresini belirtin: 
    
     services.AddHttpClient("JwtApi", client =>
{
    client.BaseAddress = new Uri(" https://localhost:5269/");
});
   4) Uygulamayı dotnet run ile çalıştırın.