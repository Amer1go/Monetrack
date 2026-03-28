using System;
using System.Collections.Generic;
using Monetrack.Shared.Models;

namespace Monetrack.Mobile
{
    public partial class AddTransactionPage : ContentPage
    {
        public class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private List<CategoryItem> _expenseCategories;
        private List<CategoryItem> _incomeCategories;

        public AddTransactionPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            _expenseCategories = new List<CategoryItem>
            {
                new CategoryItem { Id = 1, Name = "Продукти" },
                new CategoryItem { Id = 2, Name = "Кафе та ресторани" },
                new CategoryItem { Id = 3, Name = "Авто та транспорт" },
                new CategoryItem { Id = 4, Name = "Домашні тварини" },
                new CategoryItem { Id = 5, Name = "Здоров'я та ліки" },
                new CategoryItem { Id = 6, Name = "Дім та комуналка" },
                new CategoryItem { Id = 7, Name = "Одяг та взуття" },
                new CategoryItem { Id = 8, Name = "Освіта (КПІ, курси)" },
                new CategoryItem { Id = 9, Name = "Електроніка та ПК" },
                new CategoryItem { Id = 10, Name = "Хобі (Лего, ігри)" },
                new CategoryItem { Id = 11, Name = "Спорт" },
                new CategoryItem { Id = 12, Name = "Подорожі" },
                new CategoryItem { Id = 13, Name = "Підписки та сервіси" },
                new CategoryItem { Id = 14, Name = "Різне" }
            };

            _incomeCategories = new List<CategoryItem>
            {
                new CategoryItem { Id = 15, Name = "Зарплата" },
                new CategoryItem { Id = 16, Name = "Фріланс / Підробіток" },
                new CategoryItem { Id = 17, Name = "Стипендія" }
            };

            TypePicker.SelectedIndex = 0;
        }

        private void OnTypeChanged(object sender, EventArgs e)
        {
            if (TypePicker.SelectedIndex == 0) 
            {
                CategoryPicker.ItemsSource = _expenseCategories;
            }
            else 
            {
                CategoryPicker.ItemsSource = _incomeCategories;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (!decimal.TryParse(AmountEntry.Text, out decimal amount) || amount <= 0)
            {
                await DisplayAlert("Помилка", "Введіть коректну суму більше нуля!", "ОК");
                return;
            }

            string note = NoteEntry.Text;
            if (string.IsNullOrWhiteSpace(note))
            {
                await DisplayAlert("Помилка", "Введіть опис транзакції!", "ОК");
                return;
            }

            var selectedCategory = (CategoryItem)CategoryPicker.SelectedItem;
            if (selectedCategory == null)
            {
                await DisplayAlert("Помилка", "Оберіть категорію!", "ОК");
                return;
            }

            if (TypePicker.SelectedIndex == 0)
            {
                amount = amount * -1;
            }

            string userId = Preferences.Default.Get("UserId", string.Empty);

            var newTx = new Transaction
            {
                UserId = userId,
                AccountId = 1,
                CategoryId = selectedCategory.Id, 
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Note = note,
                IsSynced = true
            };

            var api = new ApiService();
            bool isSuccess = await api.AddTransactionAsync(newTx);

            if (isSuccess)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Помилка", "Не вдалося зберегти транзакцію.", "ОК");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}