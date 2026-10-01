namespace Byd.AuthManagement.Consts;

/// <summary>
/// Kaydin sabit degerlerini tanimlar.
/// </summary>
// islevi: Kayitla acilan tenant'in sahip rolunun adini ve tenant adinin azami uzunlugunu tek yerde tutar.
// sistemdeki gorevi: Tenant acan kayitta rol bu adla kurulur; validator tenant adini kolona carpmadan reddeder.
public static class RegistrationConsts
{
    // Kayitla acilan tenant'ta kullaniciya verilen, tum izinleri tasiyan rolun adi.
    public const string OwnerRoleName = "admin";

    // Tenant adinin azami uzunlugu; cercevenin tenant adi kolonunun uzunlugudur.
    public const int MaxTenantNameLength = 64;
}
