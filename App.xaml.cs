using Microsoft.Extensions.DependencyInjection;

namespace esalazarT3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // La vista 1 es la raíz de la navegación (necesario para PushAsync)
            return new Window(new NavigationPage(new Views.vContacto())
            {
                BarBackgroundColor = Color.FromArgb("#1F3A5F"),
                BarTextColor = Colors.White
            });
        }
    }
}
