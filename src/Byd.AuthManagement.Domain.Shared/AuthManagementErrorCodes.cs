namespace Byd.AuthManagement;

/// <summary>
/// Modulun is hatasi kod grameri.
/// </summary>
// islevi: Kok namespace'i ve ozellik ailesi basina numara grubunu tanimlar.
// sistemdeki gorevi: Her kod MapCodeNamespace ile kaydedilen kokle baslar; aile numarada tasinir ("AuthManagement:01").
public static class AuthManagementErrorCodes
{
    // Modulun Domain.Shared modulunde MapCodeNamespace ile kaydedilen kok namespace.
    public const string RootNamespace = "AuthManagement";

    // Organizasyon ailesinin numara grubu.
    public const string OrganizationGroup = RootNamespace + ":01";

    // Login ailesinin numara grubu.
    public const string LoginGroup = RootNamespace + ":02";

    // Kayit ailesinin numara grubu.
    public const string RegistrationGroup = RootNamespace + ":03";

    // Sifre ailesinin numara grubu.
    public const string PasswordGroup = RootNamespace + ":04";

    // Kullanici daveti ailesinin numara grubu.
    public const string InvitationGroup = RootNamespace + ":05";
}
