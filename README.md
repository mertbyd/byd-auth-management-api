# Byd.AuthManagement

Login, kapsam secimi, kayit, davet, sifre islemleri, organizasyon ve organizasyon feature yonetimini tasiyan ABP DDD
modulu. Token sunucusu OpenIddict'tir; tablolar `auth_management` semasinda durur.

## Paketler

Tuketen projede her katman ayni adli katman paketini referans alir ve o katmanin modulunu `[DependsOn]` ile ekler:

| Paket | Modul |
|---|---|
| `Byd.AuthManagement.Domain.Shared` | `AuthManagementDomainSharedModule` |
| `Byd.AuthManagement.Domain` | `AuthManagementDomainModule` |
| `Byd.AuthManagement.Application.Contracts` | `AuthManagementApplicationContractsModule` |
| `Byd.AuthManagement.Application` | `AuthManagementApplicationModule` |
| `Byd.AuthManagement.EntityFrameworkCore` | `AuthManagementEntityFrameworkCoreModule` |
| `Byd.AuthManagement.HttpApi` | `AuthManagementHttpApiModule` |
| `Byd.AuthManagement.HttpApi.Client` | `AuthManagementHttpApiClientModule` |

Domain.Shared, Domain, Application.Contracts, Application ve EntityFrameworkCore katmanlari ayni adli
`Byd.LogManagement` katmanina baglidir; log da file ve core paketlerine baglidir.
Auth'u alan proje log, file ve core'u onunla birlikte alir; ucunun host sozlesmesi de gecerlidir.

## Token ve kapsam

- Token'a secilen kapsam yazilir: `tenant_id`, `tenant_name`, `organization_id`, `organization_name`,
  `parent_organization_id`, `parent_organization_name` ve `scope_level`. Roller secilen kapsama indirgenir.
- Davet maili core'un e-posta sablonlariyla gider. Davet olayi (`AuthManagement.UserInvitation`) core'un olay
  tablosuna, varsayilan tr/en davet sablonu host kapsaminda seed edilir.

## Host baglantisi

- Auth tablolarinin baglanti dizesi adi `AuthManagement`'tir; tanimli degilse `Default`'a duser. Auth paketini
  kullanan modul host'u bu adla auth veritabanini, `Default` ile kendi veritabanini gosterir; izinler ve feature
  degerleri auth veritabanindan okunur. `dotnet ef` komutlari bu adi okudugu icin ayarda tanimli olmalidir.
- Host `AuthManagementModuleOptions.Authority`'yi token sunucusunun adresiyle doldurur; login servisi token'i
  bu adresteki token ucundan ister.
- Sifre sifirlama ve davet baglantisi `Web` adli uygulamanin `account/reset-password` sayfasina gider; host bu
  uygulamanin kok adresini `AppUrlOptions` ile verir.
- SMTP ayarlari core'un `Abp.Mailing.*` sozlesmesindedir.
- Pipeline'da `app.UseAuthentication()` ve `app.UseAbpOpenIddictValidation()` cagrilarindan sonra
  `app.UseCoreExtensions()` ve `app.UseFileManagement()`, ardindan `app.UseAuthorization()` ve
  `app.UseAuditing()` gelir.
