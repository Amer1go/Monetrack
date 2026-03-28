using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using Monetrack.Shared.Models;

namespace Monetrack.Mobile
{
    internal class ApiService
    {
        private readonly HttpClient _http = new HttpClient() { BaseAddress = new Uri("https://localhost:7270/") };

        public async Task<string> RegisterAsync(string username, string email, string password)
        {
            try
            {
                string url = $"api/Auth/register?username={username}&email={email}&password={password}";
                var response = await _http.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    return "user add complete!";
                }
                else
                {
                    return $"Server misstake: {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                return $"Connection lost: {ex.Message}";

            }
        }
        public async Task<string> LoginAsync(string login, string password)
        {
            try
            {
                string url = $"api/Auth/login?login={login}&password={password}";
                var response = await _http.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(json);
                    return doc.RootElement.GetProperty("userId").GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<Transaction>> GetTransactionsAsync(string userId)
        {
            string url = $"api/Transactions/user/{userId}";
            var transactions = await _http.GetFromJsonAsync<List<Transaction>>(url);
            return transactions ?? new List<Transaction>();
        }
        public async Task<bool> AddTransactionAsync(Transaction transaction)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Transactions", transaction);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteTransactionAsync(string transactionId)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/Transactions/{transactionId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

    }
}
