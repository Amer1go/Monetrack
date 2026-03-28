using System;
using System.Collections.Generic;
using System.Linq;
using Monetrack.Shared.Models;
using Microcharts;
using SkiaSharp;
using System.Globalization;

namespace Monetrack.Mobile
{
    public partial class DashboardPage : ContentPage
    {
        private bool _isGraphicalView = false;

        private DateTime _currentDate = DateTime.Now;
        private List<Transaction> _allTransactions = new List<Transaction>();

        public DashboardPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_allTransactions != null && _allTransactions.Any())
            {
                UpdateUIForCurrentMonth();
            }

            var api = new ApiService();
            string userId = Preferences.Default.Get("UserId", string.Empty);

            if (string.IsNullOrEmpty(userId)) return;

            var transactions = await api.GetTransactionsAsync(userId);

            if (transactions != null)
            {
                _allTransactions = transactions;
                UpdateUIForCurrentMonth();
            }
        }

        private async void LoadDataAsync()
        {
            var api = new ApiService();
            string userId = Preferences.Default.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            var transactions = await api.GetTransactionsAsync(userId);
            if (transactions != null)
            {
                _allTransactions = transactions;
                UpdateUIForCurrentMonth();
            }
        }

        private void UpdateUIForCurrentMonth()
        {
            string monthName = _currentDate.ToString("MMMM yyyy", new CultureInfo("uk-UA"));
            MonthLabel.Text = char.ToUpper(monthName[0]) + monthName.Substring(1);

            var currentMonthTransactions = _allTransactions
                .Where(t => t.TransactionDate.Year == _currentDate.Year && t.TransactionDate.Month == _currentDate.Month)
                .ToList();

            TransactionsList.ItemsSource = currentMonthTransactions;

            decimal balance = currentMonthTransactions.Sum(t => t.Amount);
            BalanceLabel.Text = $"{balance:F2} ₴";
            BalanceLabel.TextColor = balance >= 0 ? Colors.Green : Colors.Red;

            UpdateChart(currentMonthTransactions);
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            UpdateUIForCurrentMonth();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            UpdateUIForCurrentMonth();
        }

        private async void OnAddTransactionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTransactionPage());
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var transaction = button?.CommandParameter as Transaction;

            if (transaction == null) return;

            bool isConfirmed = await DisplayAlert("Підтвердження", $"Точно видалити запис '{transaction.Note}'?", "Видалити", "Скасувати");

            if (!isConfirmed) return;

            var api = new ApiService();
            bool isSuccess = await api.DeleteTransactionAsync(transaction.Id.ToString());

            if (isSuccess)
            {
                OnAppearing();
            }
            else
            {
                await DisplayAlert("Помилка", "Не вдалося видалити транзакцію.", "ОК");
            }
        }

        private void OnToggleViewClicked(object sender, EventArgs e)
        {
            _isGraphicalView = !_isGraphicalView;

            if (_isGraphicalView)
            {
                ViewToggleButton.Text = "📄 Простий";
                ViewToggleButton.BackgroundColor = Color.FromArgb("#2980B9");
                GraphPlaceholder.IsVisible = true;
                BalanceTitleLabel.Text = "Залишилось:";
            }
            else
            {
                ViewToggleButton.Text = "🍩 Графік";
                ViewToggleButton.BackgroundColor = Color.FromArgb("#F39C12");
                GraphPlaceholder.IsVisible = false;
                BalanceTitleLabel.Text = "Твій баланс:";
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void UpdateChart(List<Transaction> transactions)
        {
            var groupedExpenses = transactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    TotalAmount = Math.Abs(g.Sum(t => t.Amount))
                })
                .ToList();

            if (!groupedExpenses.Any())
            {
                DonutChartView.Chart = null;
                return;
            }

            // РАХУЄМО ЗАГАЛЬНУ СУМУ для відсотків
            decimal totalExpenses = groupedExpenses.Sum(g => g.TotalAmount);

            // ПЕРЕВІРЯЄМО НАЛАШТУВАННЯ (чи показувати текст)
            bool showLegend = Preferences.Default.Get("ShowChartLegend", true);

            var entries = new List<ChartEntry>();

            foreach (var group in groupedExpenses)
            {
                string colorHex = "#9B59B6";
                string labelName = "Витрата";

                switch (group.CategoryId)
                {
                    case 1: colorHex = "#E74C3C"; labelName = "Продукти"; break;
                    case 2: colorHex = "#D35400"; labelName = "Кафе"; break;
                    case 3: colorHex = "#F39C12"; labelName = "Авто"; break;
                    case 4: colorHex = "#8E44AD"; labelName = "Тварини"; break;
                    case 5: colorHex = "#16A085"; labelName = "Здоров'я"; break;
                    case 6: colorHex = "#2C3E50"; labelName = "Дім"; break;
                    case 7: colorHex = "#F1C40F"; labelName = "Одяг"; break;
                    case 8: colorHex = "#2980B9"; labelName = "Освіта"; break;
                    case 9: colorHex = "#34495E"; labelName = "ПК"; break;
                    case 10: colorHex = "#9B59B6"; labelName = "Хобі"; break;
                    case 11: colorHex = "#E67E22"; labelName = "Спорт"; break;
                    case 12: colorHex = "#1ABC9C"; labelName = "Подорожі"; break;
                    case 13: colorHex = "#7F8C8D"; labelName = "Підписки"; break;
                    case 14: colorHex = "#BDC3C7"; labelName = "Різне"; break;
                }

                decimal percentage = 0;
                if (totalExpenses > 0)
                {
                    percentage = (group.TotalAmount / totalExpenses) * 100;
                }

                string finalLabel = showLegend ? labelName : "";
                string finalValueLabel = showLegend ? $"{group.TotalAmount:N0} ₴ ({percentage:F1}%)" : "";

                entries.Add(new ChartEntry((float)group.TotalAmount)
                {
                    Label = finalLabel,
                    ValueLabel = finalValueLabel,
                    Color = SKColor.Parse(colorHex),
                    ValueLabelColor = SKColor.Parse(colorHex),
                    TextColor = SKColors.Black
                });
            }

            DonutChartView.Chart = new DonutChart
            {
                Entries = entries.ToArray(),
                LabelTextSize = 22f,
                Margin = 20,
                HoleRadius = 0.5f,
                BackgroundColor = SKColors.Transparent
            };
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Default.Remove("UserId");
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}