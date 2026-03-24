namespace Monetrack.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string login = LoginEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Помилка", "Введіть логін та пароль!", "Ок");
                return;
            }

            var api = new ApiService();

            string loggedInUserId = await api.LoginAsync(login, password);

            if (!string.IsNullOrEmpty(loggedInUserId))
            {
                Preferences.Default.Set("UserId", loggedInUserId);

                App.Current.MainPage = new NavigationPage(new DashboardPage());
            }
            else
            {
                await DisplayAlert("Помилка", "Невірний логін або пароль!", "Ок");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string login = LoginEntry.Text;
            string password = PasswordEntry.Text;
            string email = login + "@test.com";

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Помилка", "Не всі поля заповнені!", "Ок");
                return;
            }

            var api = new ApiService();
            string result = await api.RegisterAsync(login, email, password);

            await DisplayAlert("Результат", result, "Ок");
        }
    }
}