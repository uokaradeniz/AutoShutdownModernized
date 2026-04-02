# Auto Shutdown Modernized (Windows 11)

Bu uygulama, Windows 11 tasarım dili ile hazırlanmış, kullanıcıların bilgisayarlarının ne zaman otomatik kapatılacağını planlamalarını sağlayan ve aktif olan kapatma işleminin kalan süresini bir widget üzerinden canlı olarak gösteren WPF tabanlı bir araçtır.

## Özellikler

- **Zamanlanmış Kapatma**: Bilgisayarı belirlediğiniz dakika sonrasında kapatma (`shutdown /s /t <seconds>` yardımıyla).
- **Kapatma İşlemini İptal Etme**: Aktif kapatma işlemini sadece bir tıkla iptal etme.
- **Canlı Geri Sayım**: O an aktif olan planlanmış kapatma işleminin kalan süresini widget üzerinden anlık olarak takip edebilme.
- **Widget Modu**: "Always-on-top" (Her Zaman Üstte), çerçevesiz, temiz, yumuşatılmış köşelere sahip, kolayca konumlandırılabilen modern arayüz.

## Kullanılan Teknolojiler
- .NET 10 & C# 13 (Modern C# syntax, records, async processing)
- WPF (Windows Presentation Foundation)
- MVVM (CommunityToolkit.Mvvm)
- Minimal Hosting Pattern (`Microsoft.Extensions.Hosting`)
- Dependency Injection & Logging

## Sistem Gereksinimleri
- Windows 10/11 İşletim Sistemi
- .NET 10 SDK / Runtime

## Kurulum ve Çalıştırma

Projenin derlenmesi ve çalıştırılması için gerekli adımlar:

### PowerShell Script'ini Kullanarak (Tavsiye Edilir)

Aşağıdaki script aracılığıyla projeyi Release modunda derleyip çalıştırabilirsiniz:

```powershell
# Sadece Derleme (Build):
.\build.ps1

# Derleyip Çalıştırma (Build & Run):
.\build.ps1 -Run
```

### Manuel Yöntem (.NET CLI ile)

```sh
# Projeyi Derleyin
dotnet build

# Projeyı Çalıştırın
dotnet run
```

## Mimari
- **MVVM Pattern**: View ve logic kesin şekilde ayrılmıştır.
- **Singleton Services**: Uygulama durumu `IShutdownTrackerService` ile background service aracılığıyla tutulmaktadır. UI, `ShutdownTrackerService` üzerinden 1 saniyelik (PeriodicTimer) döngüler ile güncellenir.
- **Dependency Injection**: Minimal Hosting Pattern sayesinde tüm gerekli componentler (.NET Core stili) birbirine bağlanmıştır.

## Lisans
Bu proje açık kaynaklı olup, herhangi bir ticari amaç taşımamaktadır.


