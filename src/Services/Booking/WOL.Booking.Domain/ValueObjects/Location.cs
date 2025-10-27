using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.ValueObjects;

public class Location : ValueObject
{
    public string Address { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public string City { get; private set; }

    private Location() 
    {
        Address = string.Empty;
        City = string.Empty;
    }

    public Location(string address, decimal latitude, decimal longitude, string city)
    {
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
        City = city;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Address;
        yield return Latitude;
        yield return Longitude;
        yield return City;
    }
}
