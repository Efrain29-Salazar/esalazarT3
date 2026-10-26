using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using esalazarT3.Views.Models;

namespace esalazarT3.Views;

public partial class vResumen : ContentPage
{
    private readonly Contacto contacto;

    public vResumen(Contacto contacto)
    {
        InitializeComponent();
        this.contacto = contacto;

        var ci = CultureInfo.InvariantCulture;
        lblTipo.Text = contacto.TipoIdentificacion;
        lblIdentificacion.Text = contacto.Identificacion;
        lblNombres.Text = contacto.Nombres;
        lblApellidos.Text = contacto.Apellidos;
        lblFecha.Text = contacto.Fecha.ToString("dd/MM/yyyy", ci);
        lblCorreo.Text = contacto.Correo;
        lblSalario.Text = $"${contacto.Salario.ToString("F2", ci)}";
        lblAporte.Text = $"${contacto.AporteIess.ToString("F2", ci)}";
        lblNeto.Text = $"${contacto.SalarioNeto.ToString("F2", ci)}";
    }

    private async void btnExportar_Clicked(object? sender, EventArgs e)
    {
        try
        {
            string nombreArchivo = $"contacto_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string ruta = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);

            await File.WriteAllTextAsync(ruta, contacto.ATexto(), Encoding.UTF8);

            bool compartir = await DisplayAlertAsync(
                "Archivo exportado",
                $"Se guardó en:\n{ruta}",
                "Compartir", "Cerrar");

            if (compartir)
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Datos del contacto",
                    File = new ShareFile(ruta)
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "Cerrar");
        }
    }

    private async void btnVolver_Clicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();
}
