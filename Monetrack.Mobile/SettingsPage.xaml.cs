using System;

namespace Monetrack.Mobile
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            ThemeSwitch.IsToggled = Application.Current.RequestedTheme == AppTheme.Dark;

            ChartLegendSwitch.IsToggled = Preferences.Default.Get("ShowChartLegend", true);
        }

        private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
                Application.Current.UserAppTheme = AppTheme.Dark;
            else
                Application.Current.UserAppTheme = AppTheme.Light;
        }

        private void OnChartLegendSwitchToggled(object sender, ToggledEventArgs e)
        {
            Preferences.Default.Set("ShowChartLegend", e.Value);
        }
    }
}