namespace Application.DTOs;

public class ClientDTO
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Tel { get; set; }
    public string Email { get; set; } = null!;
    public string? Adresse { get; set; }
    public string? Image { get; set; }
}