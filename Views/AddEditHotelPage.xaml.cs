using System.Windows.Input;

namespace KostenkoLaba3
{
    public partial class AddEdithousePage : ContentPage
    {
        public house house { get; set; }
        public string PageTitle { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private Action<house> _onSaveCallback;

        // Конструктор для AddEdithousePage, який ініціалізує компоненти та встановлює початкові значення.
        public AddEdithousePage(house house, string title, Action<house> onSaveCallback)
        {
            InitializeComponent();
            this.house = house;
            PageTitle = title;
            _onSaveCallback = onSaveCallback;

            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);

            BindingContext = this; // Встановлення контексту прив’язки для UI.
        }

        private async void Save()
        {
            var validationResult = ValidatehouseData(); // Валідація даних об'єкта house.
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Error", validationResult.ErrorMessage, "OK");
                return;
            }

            _onSaveCallback?.Invoke(house); // Виклик зворотного виклику для збереження.
            await Navigation.PopAsync();
        }

        private ValidationResult ValidatehouseData()
        {
            if (string.IsNullOrWhiteSpace(house.Name))
                return ValidationResult.Failed("House name cannot be empty.");

            if (string.IsNullOrWhiteSpace(house.Location))
                return ValidationResult.Failed("Location cannot be empty.");

            if (house.CountRooms <= 0)
                return ValidationResult.Failed("Number of available rooms must be greater than 0.");

            if (house.Price <= 0)
                return ValidationResult.Failed("Price per night must be greater than 0.");

            return ValidationResult.Success();
        }

        private async void Cancel()
        {
            await Navigation.PopAsync();
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        private ValidationResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        // Метод для створення успішного результату валідації.
        public static ValidationResult Success()
        {
            return new ValidationResult(true, string.Empty);
        }

        // Метод для створення результату валідації з помилкою.
        public static ValidationResult Failed(string errorMessage)
        {
            return new ValidationResult(false, errorMessage);
        }
    }
}
