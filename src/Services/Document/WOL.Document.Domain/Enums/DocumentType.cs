namespace WOL.Document.Domain.Enums;

public enum DocumentType
{
    Istemara = 1,              // Vehicle Registration
    MVPI = 2,                  // Motor Vehicle Periodic Inspection
    VehicleInsurance = 3,      // Vehicle Insurance
    VehiclePhoto = 4,          // Vehicle Photo with Number Plate
    
    DriverLicense = 5,         // Driver's License
    DriverIqama = 6,           // Driver's Iqama (Residence Permit)
    
    CommercialLicense = 7,     // Commercial Registration/License
    VATCertificate = 8,        // VAT Registration Certificate
    
    CargoPhoto = 9,            // Photo of Packed Goods
    EmptyLoadPhoto = 10,       // Photo of Empty Load (for cancellation)
    DeliveryProof = 11,        // Proof of Delivery
    
    Other = 99
}
