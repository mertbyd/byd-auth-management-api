namespace Byd.AuthManagement.Dtos.Organizations;

/// <summary>
/// Yeni organizasyon olusturma istegini tanimlar.
/// </summary>
// islevi: Organizasyon create endpointinin public girdi tipidir.
// sistemdeki gorevi: Kimlik, kod, kapsam ve audit alanlarini sunucuda birakip degistirilebilir alanlari kabul eder.
public sealed class CreateOrganizationDto : OrganizationInputDto
{
    public string AdminUserName { get; set; } = default!;

    public string AdminEmail { get; set; } = default!;

    public string AdminPassword { get; set; } = default!;
}
