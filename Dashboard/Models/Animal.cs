using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models;

public class Animal
{
    public string IdAnimal { get; set; } = Guid.NewGuid().ToString();
    public string UuidSistema { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Arete { get; set; } = string.Empty; // Identificador único (SINIIGA/ID)

    [Required]
    public string Raza { get; set; } = string.Empty; // Ej: Cebú, Angus, F1

    [Required]
    public string Sexo { get; set; } = string.Empty; // Macho, Hembra

    // Condición (p. ej. Entero, Castrado) — mostrar solo si Sexo == "Macho"
    public int? Condicion { get; set; }

    // Opcional: asignación a Corral
    public string IdCorral { get; set; } = string.Empty;

    // Opcional: asignación a Potrero/Lote
    public string IdPotrero { get; set; } = string.Empty;
}
