namespace Dashboard.Models;

public class Corral
{
    public string IdCorral { get; set; } = string.Empty;
    public string UuidSistema { get; set; } = string.Empty;
    public string IdRancho { get; set; } = string.Empty;
    public string NombreCorral { get; set; } = string.Empty;
    public string Capacidad { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
