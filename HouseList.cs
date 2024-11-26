public class houseList
{
    private List<house> Houses = new();

    public houseList() { }

    public houseList(List<house> houses) => Houses = houses;

    public void Addhouse(house house) => Houses.Add(house);

    public void Edithouse(int id, house updatedhouse)
    {
        var house = Houses.FirstOrDefault(h => h.Id == id);
        if (house == null) return;

        house.Name = updatedhouse.Name;
        house.Location = updatedhouse.Location;
        house.CountRooms = updatedhouse.CountRooms;
        house.Price = updatedhouse.Price;
    }

    public void Removehouse(int id) =>
        Houses.RemoveAll(h => h.Id == id);

    public List<house> GetAllHouses() => Houses;

    public List<house> SearchByLocation(string location) =>
        Houses.Where(h => h.Location.Contains(location)).ToList();

    public List<house> SearchByPriceRange(decimal minPrice, decimal maxPrice) =>
        Houses.Where(h => h.Price >= minPrice && h.Price <= maxPrice).ToList();

    public List<house> SearchByAvailability(int minRooms) =>
        Houses.Where(h => h.CountRooms >= minRooms).ToList();
}
