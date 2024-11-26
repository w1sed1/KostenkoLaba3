using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KostenkoLaba3
{
    public partial class SearchHousesPage : ContentPage
    {
        public ObservableCollection<house> Houses { get; set; }

        public ObservableCollection<house> FilteredHouses { get; set; }

        // Властивість SearchQuery для зберігання тексту пошукового запиту.
        public string SearchQuery { get; set; }

        // Команда для виконання пошуку.
        public ICommand SearchCommand { get; }

        public SearchHousesPage(ObservableCollection<house> Houses)
        {
            InitializeComponent(); 

            Houses = Houses; 
            FilteredHouses = new ObservableCollection<house>(Houses); 

            SearchCommand = new Command(ExecuteSearch); 

            BindingContext = this; // Встановлюємо контекст прив'язки для інтерфейсу.
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Якщо запит порожній, показуємо всі готелі
                FilteredHouses.Clear(); 
                foreach (var house in Houses)
                    FilteredHouses.Add(house); 
            }
            else
            {
                var query = SearchQuery.ToLower(); // Приводимо пошуковий запит до нижнього регістру.
                var results = Houses.Where(h =>
                    h.Name.ToLower().Contains(query) || // Пошук по назві будинку.
                    h.Location.ToLower().Contains(query) // Пошук по місцезнаходженню будинку.
                ).ToList();

                // Очищаємо поточний список і додаємо нові результати
                FilteredHouses.Clear(); 
                foreach (var house in results)
                    FilteredHouses.Add(house); 
            }
        }
    }
}
