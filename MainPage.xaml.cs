using System.Collections.ObjectModel;

namespace KostenkoLaba3
{
    public partial class MainPage : ContentPage
    {
        private houseList houseList;

        private string filePath;

        // Default file path
        private readonly string defaultFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Houses.json");

        public List<house> Houses { get; set; }

        public MainPage()
        {
            InitializeComponent();
            filePath = defaultFilePath;
            LoadHouses();

            BindingContext = this; // Setting the binding context for the page
        }

        private void LoadHouses()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    // Load houses from the file
                    var Houses = JsonFileHandler.LoadHouses(filePath);
                    houseList = new houseList(Houses);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                    houseList = new houseList();
                }
            }
            else
            {
                houseList = new houseList();
            }

            Houses = houseList.GetAllHouses();
        }

        private void SaveHousesToFile()
        {
            try
            {
                JsonFileHandler.SaveHouses(filePath, houseList.GetAllHouses());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to save data: {ex.Message}", "OK");
            }
        }

        // Event handler for selecting a JSON file
        private async void OnChooseFileClicked(object sender, EventArgs e)
        {
            try
            {
                var jsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { "json" } }
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a JSON file",
                    FileTypes = jsonFileType
                });

                if (result != null)
                {
                    // Read the content of the selected file
                    string fileContent = await File.ReadAllTextAsync(result.FullPath);

                    // Deserialize JSON into objects
                    var Houses = JsonFileHandler.DeserializeHouses(fileContent);

                    if (Houses != null)
                    {
                        filePath = result.FullPath;
                        houseList = new houseList(Houses);
                        RefreshHouses();
                        await DisplayAlert("Success", "Data loaded successfully!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "The JSON file contains invalid data.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load the file: {ex.Message}", "OK");
            }
        }

        private void RefreshHouses()
        {
            Houses = houseList.GetAllHouses();
            BindingContext = null;
            BindingContext = this;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                SaveHousesToFile();
                await DisplayAlert("Success", "Changes saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while saving: {ex.Message}", "OK");
            }
        }

        private async void OnRemovehouseClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Remove House", "Enter the ID of the house to remove:");
                if (!int.TryParse(idStr, out int id) || id <= 0)
                {
                    await DisplayAlert("Error", "ID must be a positive numeric value.", "OK");
                    return;
                }

                var house = Houses.FirstOrDefault(h => h.Id == id);
                if (house != null)
                {
                    bool confirm = await DisplayAlert("Confirmation", $"Do you really want to remove the house \"{house.Name}\"?", "Yes", "No");
                    if (!confirm) return;

                    houseList.Removehouse(house.Id);
                    SaveHousesToFile();
                    await DisplayAlert("Success", $"House \"{house.Name}\" was successfully removed.", "OK");
                    RefreshHouses();
                }
                else
                {
                    await DisplayAlert("Error", "House with this ID not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnSearchByPriceRangeClicked(object sender, EventArgs e)
        {
            try
            {
                string minPriceStr = await DisplayPromptAsync("Search by Price", "Enter the minimum price:");
                if (!decimal.TryParse(minPriceStr, out decimal minPrice) || minPrice < 0)
                {
                    await DisplayAlert("Error", "Minimum price must be a positive number.", "OK");
                    return;
                }

                string maxPriceStr = await DisplayPromptAsync("Search by Price", "Enter the maximum price:");
                if (!decimal.TryParse(maxPriceStr, out decimal maxPrice) || maxPrice < minPrice)
                {
                    await DisplayAlert("Error", "Maximum price must be a number greater than the minimum price.", "OK");
                    return;
                }

                var searchResults = houseList.SearchByPriceRange(minPrice, maxPrice);

                if (searchResults.Any())
                {
                    await Navigation.PushAsync(new SearchHousesPage(new ObservableCollection<house>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Search Results", "No houses found in the specified price range.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private async void OnSearchByLocationClicked(object sender, EventArgs e)
        {
            try
            {
                string location = await DisplayPromptAsync("Search by Location", "Enter the location:");
                if (string.IsNullOrWhiteSpace(location)) return;

                var searchResults = houseList.SearchByLocation(location);

                if (searchResults.Any())
                {
                    await Navigation.PushAsync(new SearchHousesPage(new ObservableCollection<house>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Search Results", "No houses found for the specified location.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnSearchByAvailabilityClicked(object sender, EventArgs e)
        {
            try
            {
                string roomsStr = await DisplayPromptAsync("Search by Rooms", "Enter the minimum number of rooms:");
                if (!int.TryParse(roomsStr, out int minRooms) || minRooms < 0)
                {
                    await DisplayAlert("Error", "Number of rooms must be a positive number.", "OK");
                    return;
                }

                var searchResults = houseList.SearchByAvailability(minRooms);

                if (searchResults.Any())
                {
                    await Navigation.PushAsync(new SearchHousesPage(new ObservableCollection<house>(searchResults)));
                }
                else
                {
                    await DisplayAlert("Search Results", "No houses found with the specified number of rooms.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnAddhouseClicked(object sender, EventArgs e)
        {
            var newhouse = new house
            {
                Id = Houses.Count + 1
            };

            await Navigation.PushAsync(new AddEdithousePage(newhouse, "Add House", house =>
            {
                houseList.Addhouse(house);
                SaveHousesToFile();
                RefreshHouses();
            }));
        }

        private async void OnEdithouseClicked(object sender, EventArgs e)
        {
            try
            {
                string idStr = await DisplayPromptAsync("Edit House", "Enter the ID of the house:");
                if (!int.TryParse(idStr, out int id))
                {
                    await DisplayAlert("Error", "ID must be a numeric value.", "OK");
                    return;
                }

                var house = Houses.FirstOrDefault(h => h.Id == id);
                if (house == null)
                {
                    await DisplayAlert("Error", "House with this ID not found.", "OK");
                    return;
                }

                await Navigation.PushAsync(new AddEdithousePage(house, "Edit House", updatedhouse =>
                {
                    houseList.Edithouse(updatedhouse.Id, updatedhouse);
                    SaveHousesToFile();
                    RefreshHouses();
                }));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnAdvancedSearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.AdvancedSearchPage(houseList));
        }
    }
}
