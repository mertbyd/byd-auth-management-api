namespace Byd.AuthManagement.Consts;

/// <summary>
/// Organizasyonun sabit degerlerini tanimlar.
/// </summary>
// islevi: Organizasyon adinin kabul edilen azami uzunlugunu tek yerde tutar.
// sistemdeki gorevi: Cercevenin display_name kolonunun uzunlugudur; validator istegi kolona carpmadan reddeder.
public static class OrganizationConsts
{
    // Organizasyon adinin azami uzunlugu.
    public const int MaxDisplayNameLength = 128;
}
