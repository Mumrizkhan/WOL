using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.ValueObjects;

public class ContactInfo : ValueObject
{
    public string Name { get; private set; }
    public string Mobile { get; private set; }
    public string? Email { get; private set; }

    private ContactInfo() 
    {
        Name = string.Empty;
        Mobile = string.Empty;
    }

    public ContactInfo(string name, string mobile, string? email)
    {
        Name = name;
        Mobile = mobile;
        Email = email;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Mobile;
        yield return Email;
    }
}
