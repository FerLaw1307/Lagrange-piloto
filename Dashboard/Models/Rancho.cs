namespace Dashboard.Models;

public class Rancho
{
    public string IdRancho { get; set; } = string.Empty;
    public string UuidSistema { get; set; } = string.Empty;
    public string IdEmpresa { get; set; } = string.Empty;
    public string NombreRancho { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
