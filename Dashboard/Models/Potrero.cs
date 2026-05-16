namespace Dashboard.Models;

public class Potrero
{
    public string IdPotrero { get; set; } = string.Empty;
    public string UuidSistema { get; set; } = string.Empty;
    public string IdRancho { get; set; } = string.Empty;
    public string NombrePotrero { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
