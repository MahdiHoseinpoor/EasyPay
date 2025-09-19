using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{
    public class CurrencyService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private const string CurrencyKey = "currencyPreference";

        public CurrencyDisplay CurrentCurrency { get; private set; } = CurrencyDisplay.Toman;

        public event Action OnCurrencyChange;

        public CurrencyService(ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task InitializeAsync()
        {
            var savedPreference = await _localStorage.GetItemAsync<string>(CurrencyKey);
            if (Enum.TryParse<CurrencyDisplay>(savedPreference, out var preference))
            {
                CurrentCurrency = preference;
            }
            else
            {
                CurrentCurrency = CurrencyDisplay.Toman; 
            }
        }

        public async Task ToggleCurrencyAsync()
        {
            CurrentCurrency = CurrentCurrency == CurrencyDisplay.Rial ? CurrencyDisplay.Toman : CurrencyDisplay.Rial;
            await _localStorage.SetItemAsStringAsync(CurrencyKey, CurrentCurrency.ToString());
            OnCurrencyChange?.Invoke();
        }

        public string Format(decimal rialAmount)
        {
            if (CurrentCurrency == CurrencyDisplay.Toman)
            {
                return (rialAmount / 10).ToString("N0", CultureInfo.InvariantCulture);
            }
            return rialAmount.ToString("N0", CultureInfo.InvariantCulture);
        }

        public string GetCurrencySymbol() => CurrentCurrency.ToString();
    }
}