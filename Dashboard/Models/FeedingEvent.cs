using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models;

public class FeedingEvent
{
    public string IdEvent { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string Arete { get; set; } = string.Empty; // linked animal arete
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string TipoAlimento { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string Notas { get; set; } = string.Empty;
}
