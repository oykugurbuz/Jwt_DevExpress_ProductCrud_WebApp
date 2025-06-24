# WebApp_Demo-master

# WebAppDemo_update (.NET Core MVC UI)

Bu proje, oluşturmuş olduğum JwtToken_Project (kullanıcı kimlik doğrulama ve yetkilendirme işlemlerini JWT (JSON Web Token) kullanarak gerçekleştiren bir ASP.NET Core Web API uygulaması) ile entegre olarak çalışır. Kullanıcıların giriş yaparak ürün/kategori CRUD işlemleri yapmasını sağlar ve DevExpress bileşenleri ile zenginleştirilmiştir. Tarih, fiyat ve kategori filtreli raporlama yapar.CRUD işlemleri için kullanıcı bazlı yetkilendirme (okuma, ekleme, güncelleme, silme) yapar.

#Özellikler

- Kullanıcı kimlik doğrulama işlemleri (API üzerinden JWT ile)
- JWT token cookie olarak saklanır (HTTPOnly güvenliği ile)
- Ürün ve Kategori CRUD işlemleri
- Yetki tabanlı işlem izni (okuma, ekleme, güncelleme, silme kontrolü)
- Kullanıcıya özel authority level
- Action bazlı HasPermission attribute yapısı
- Kategori, fiyat ve tarih aralığına göre gelişmiş filtreleme,raporlama ve raporların iframe ile sayfa içine gömülmesi
- DevExpress bileşenleri ile ile zengin arayüz

#JWT

- Kullanıcı, API aracılığıyla giriş yaptığında sunucudan bir JWT Token döner.
- Bu token, cookie içerisine (HTTPOnly flag ile) kaydedilir.
- Cookie tarayıcıda saklandığı için token güvenli bir şekilde taşınır; JavaScript erişemez.
- Her istek otomatik olarak bu token ile yapılır; kullanıcı kimliği korunur.
- #Yetki Tabanlı İşlem İzni

Kullanıcıların hangi işlemleri yapabileceği, yetki (permission) tablosu ve kullanıcıya bağlı izinlerle kontrol edilir.
Bunun için özel bir [HasPermission] attribute sınıfı tanımlanmıştır.

### Örnek kullanım:
```csharp
[HasPermission("Product.Create")]
public IActionResult CreateProduct() { 
     // yalnızca bu yetkiye sahip kullanıcılar erişebilir
 }
 ```

Kullanılan Teknolojiler
- ASP.NET Core MVC (.NET 8)

- DevExpress JavaScript ve Reporting v24.2+

- Entity Framework Core

- Cookie tabanlı JWT saklama

# Uygulama Arayüzü

## Giriş Ekranı görünümü: 

![Giriş ekranı görseli](screenshots/login_page.png) 

## Ürünler

### Ürün listesi görünümü: 

![Ana sayfa görseli](screenshots/home_page.png)

### Ürün Ekleme PopUp görünümü: 

![Ürün Ekleme PopUp görseli](screenshots/product_create_popup.png)

### Ürün Düzenleme PopUp görünümü: 

![Ürün Ekleme PopUp görseli](screenshots/product_update_popup.png)

### Ürün Silme PopUp görünümü: 

![Ürün Silme PopUp görseli](screenshots/product_delete_uinotify.png)

##Kategoriler

![Kategori görseli](screenshots/categorypage.png)

##Rapor
### Filtreleme:

![Rapor Filtreleme görseli](screenshots/report_filter.png)

### Rapor Pdf

![Rapor Pdf görseli](screenshots/report_pdf.png)

### Yetkilendirme Sayfası 

![Yetkilendirme sayfası görseli](screenshots/permission.png)

### Yetkili olmayan kullanıcıya geribildirim

![Yetkili olmayan kullanıcıya geri bildirim görseli](screenshots/statuscode_403.png)
![Yetkili olmayan Kullanıcıya geri bildirim görseli](screenshots/statuscode_403(2).png)