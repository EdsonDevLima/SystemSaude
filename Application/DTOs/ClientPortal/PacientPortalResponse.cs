namespace SystemSaude.Application.DTOs.ClientPortal;

public class PacientPortalResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public AddressRequest Address { get; set; } = new();
}
