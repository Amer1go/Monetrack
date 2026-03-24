using Monetrack.Shared.Models;

namespace Monetrack.Mobile;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = Preferences.Default.Get("UserId", string.Empty);

        if (string.IsNullOrEmpty(userId))
            return;

        try
        {
            var api = new ApiService();
            var transactions = await api.GetTransactionsAsync(userId);

            TransactionsList.ItemsSource = transactions;
            decimal totalBalance = transactions.Sum(t => t.Amount);
            BalanceLabel.Text = $"{totalBalance:F2} UAH";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка связи с API", ex.Message, "ОК");
        }
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        Preferences.Default.Remove("UserId");
        App.Current.MainPage = new MainPage();
    }
    private async void OnAddTestTransactionClicked(object sender, EventArgs e)
    {
        string userId = Preferences.Default.Get("UserId", string.Empty);

        var newTx = new Transaction
        {
            UserId = userId,
            AccountId = 1, 
            CategoryId = 1, 
            Amount = 1000.00m,
            TransactionDate = DateTime.UtcNow,
            Note = "Зарплата (Тест)",
            IsSynced = true
        };

        var api = new ApiService();
        bool isSuccess = await api.AddTransactionAsync(newTx);

        if (isSuccess)
        {
            OnAppearing();
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось сохранить транзакцию", "ОК");
        }
    }
}