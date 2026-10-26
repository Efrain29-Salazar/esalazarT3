using System.Globalization;
using System.Text;

namespace esalazarT3.Views.Models;

public class Contacto
{
    // Aporte personal al IESS (trabajador en relación de dependencia, sector privado)
    public const decimal TasaAporteIess = 0.0945m;

    public string TipoIdentificacion { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Correo { get; set; } = string.Empty;
    public decimal Salario { get; set; }

    public decimal AporteIess =>
        Math.Round(Salario * TasaAporteIess, 2, MidpointRounding.AwayFromZero);

    public decimal SalarioNeto => Salario - AporteIess;

    public string ATexto()
    {
        var ci = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.AppendLine("=== DATOS DEL CONTACTO ===");
        sb.AppendLine($"Tipo de identificación : {TipoIdentificacion}");
        sb.AppendLine($"Identificación         : {Identificacion}");
        sb.AppendLine($"Nombres                : {Nombres}");
        sb.AppendLine($"Apellidos              : {Apellidos}");
        sb.AppendLine($"Fecha de nacimiento    : {Fecha.ToString("dd/MM/yyyy", ci)}");
        sb.AppendLine($"Correo                 : {Correo}");
        sb.AppendLine($"Salario                : ${Salario.ToString("F2", ci)}");
        sb.AppendLine($"Aporte IESS (9,45%)    : ${AporteIess.ToString("F2", ci)}");
        sb.AppendLine($"Salario neto           : ${SalarioNeto.ToString("F2", ci)}");
        sb.AppendLine($"Generado el            : {DateTime.Now.ToString("dd/MM/yyyy HH:mm", ci)}");
        return sb.ToString();
    }
}
