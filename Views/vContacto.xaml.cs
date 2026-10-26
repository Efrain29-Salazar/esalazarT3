using System.Globalization;
using System.Text.RegularExpressions;
using esalazarT3.Views.Models;

namespace esalazarT3.Views;

public partial class vContacto : ContentPage
{
    private const int EdadMinima = 18;

    private static readonly Regex RegexNombre = new(@"^\p{L}+([ '\-]\p{L}+)*$");
    private static readonly Regex RegexCorreo = new(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");

    public vContacto()
    {
        InitializeComponent();

        pkrTipo.ItemsSource = new List<string> { "CI", "RUC", "Pasaporte" };

        dpFecha.MinimumDate = new DateTime(1900, 1, 1);
        dpFecha.MaximumDate = DateTime.Today.AddYears(-EdadMinima);
        dpFecha.Date = DateTime.Today.AddYears(-EdadMinima);
    }

    private static int CalcularEdad(DateTime nacimiento)
    {
        DateTime hoy = DateTime.Today;
        int edad = hoy.Year - nacimiento.Year;
        if (nacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }

    // Al cambiar el tipo se ajusta el teclado, el largo máximo y el texto de ayuda
    private void pkrTipo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        txtIdentificacion.Text = string.Empty;
        txtIdentificacion.IsEnabled = true;

        switch (pkrTipo.SelectedItem as string)
        {
            case "CI":
                txtIdentificacion.Keyboard = Keyboard.Numeric;
                txtIdentificacion.MaxLength = 10;
                txtIdentificacion.Placeholder = "10 números";
                break;
            case "RUC":
                txtIdentificacion.Keyboard = Keyboard.Numeric;
                txtIdentificacion.MaxLength = 13;
                txtIdentificacion.Placeholder = "13 números";
                break;
            default: // Pasaporte: formato libre
                txtIdentificacion.Keyboard = Keyboard.Default;
                txtIdentificacion.MaxLength = 20;
                txtIdentificacion.Placeholder = "Número de pasaporte";
                break;
        }
    }

    // En CI y RUC solo se permiten dígitos (también en PC, donde el teclado numérico no restringe)
    private void txtIdentificacion_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string tipo = pkrTipo.SelectedItem as string ?? string.Empty;
        if (tipo is not ("CI" or "RUC")) return;

        string actual = e.NewTextValue ?? string.Empty;
        string soloNumeros = new(actual.Where(c => c is >= '0' and <= '9').ToArray());
        if (soloNumeros != actual)
            txtIdentificacion.Text = soloNumeros;
    }

    private async void btnGuardar_Clicked(object? sender, EventArgs e)
    {
        try
        {
            var errores = new List<string>();

            string tipo = pkrTipo.SelectedItem as string ?? string.Empty;
            string id = txtIdentificacion.Text?.Trim() ?? string.Empty;
            string nombres = txtNombres.Text?.Trim() ?? string.Empty;
            string apellidos = txtApellidos.Text?.Trim() ?? string.Empty;
            string correo = txtCorreo.Text?.Trim() ?? string.Empty;
            string textoSalario = (txtSalario.Text ?? string.Empty).Trim().Replace(',', '.');

            // Identificación
            if (tipo == string.Empty)
                errores.Add("• Seleccione el tipo de identificación.");
            else if (tipo == "CI" && !Regex.IsMatch(id, "^[0-9]{10}$"))
                errores.Add("• La CI debe tener exactamente 10 números.");
            else if (tipo == "RUC" && !Regex.IsMatch(id, "^[0-9]{13}$"))
                errores.Add("• El RUC debe tener exactamente 13 números.");
            else if (tipo == "Pasaporte" && id.Length == 0)
                errores.Add("• Ingrese el número de pasaporte.");

            // Nombres y apellidos
            if (!RegexNombre.IsMatch(nombres))
                errores.Add("• Nombres: solo letras y espacios (no puede estar vacío).");
            if (!RegexNombre.IsMatch(apellidos))
                errores.Add("• Apellidos: solo letras y espacios (no puede estar vacío).");

            // Fecha de nacimiento
            if (dpFecha.Date is null)
                errores.Add("• Seleccione la fecha de nacimiento.");
            else if (dpFecha.Date > DateTime.Today)
                errores.Add("• La fecha no puede ser futura.");
            else if (CalcularEdad(dpFecha.Date.Value) < EdadMinima)
                errores.Add($"• Debe ser mayor de {EdadMinima} años.");

            // Correo
            if (!RegexCorreo.IsMatch(correo))
                errores.Add("• Ingrese un correo válido (ej: usuario@correo.com).");

            // Salario
            if (!decimal.TryParse(textoSalario, NumberStyles.AllowDecimalPoint,
                                  CultureInfo.InvariantCulture, out decimal salario) || salario <= 0)
                errores.Add("• El salario debe ser un número mayor que 0 (use punto para decimales).");

            if (errores.Count > 0)
            {
                lblErrores.Text = string.Join("\n", errores);
                lblErrores.IsVisible = true;
                return;
            }

            lblErrores.IsVisible = false;

            var contacto = new Contacto
            {
                TipoIdentificacion = tipo,
                Identificacion = id,
                Nombres = nombres,
                Apellidos = apellidos,
                Fecha = dpFecha.Date ?? DateTime.Today,
                Correo = correo,
                Salario = salario
            };

            await Navigation.PushAsync(new vResumen(contacto));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "Cerrar");
        }
    }

    private void btnLimpiar_Clicked(object? sender, EventArgs e)
    {
        pkrTipo.SelectedIndex = -1;
        txtIdentificacion.Text = string.Empty;
        txtIdentificacion.IsEnabled = false;
        txtIdentificacion.Placeholder = "Primero elija el tipo";
        txtNombres.Text = string.Empty;
        txtApellidos.Text = string.Empty;
        dpFecha.Date = DateTime.Today.AddYears(-EdadMinima);
        txtCorreo.Text = string.Empty;
        txtSalario.Text = string.Empty;
        lblErrores.IsVisible = false;
    }
}