
INSERT INTO vehicle_types (id, name, category, length_meters, width_meters, height_meters, capacity_kg, description, is_active, created_at)
VALUES
    (gen_random_uuid(), 'Light Truck - Box', 'DryVan', 4.5, 2.0, 2.2, 1500, 'Light truck with enclosed box body, suitable for general cargo', true, NOW()),
    (gen_random_uuid(), 'Light Truck - Flatbed', 'Flatbed', 4.5, 2.0, 0.5, 1500, 'Light truck with open flatbed, suitable for construction materials', true, NOW()),
    (gen_random_uuid(), 'Light Truck - Reefer', 'Refrigerated', 4.5, 2.0, 2.2, 1200, 'Light truck with refrigeration unit for perishable goods', true, NOW()),
    (gen_random_uuid(), 'Light Truck - Canopy', 'Canopy', 4.5, 2.0, 2.0, 1500, 'Light truck with canvas canopy cover', true, NOW()),
    
    (gen_random_uuid(), 'Box Truck - Rigid', 'DryVan', 7.0, 2.4, 2.6, 5000, 'Medium rigid box truck for general cargo', true, NOW()),
    (gen_random_uuid(), 'Semi-Trailer - Dry Van', 'DryVan', 13.6, 2.5, 2.7, 24000, 'Standard semi-trailer with enclosed dry van body', true, NOW()),
    
    (gen_random_uuid(), 'Curtain-Side Trailer', 'Curtainside', 13.6, 2.5, 2.7, 24000, 'Semi-trailer with retractable curtain sides for easy loading', true, NOW()),
    
    (gen_random_uuid(), 'Flatbed Trailer', 'Flatbed', 13.6, 2.5, 0.5, 26000, 'Open flatbed trailer for oversized or construction cargo', true, NOW()),
    
    (gen_random_uuid(), 'Low Bed Trailer', 'Lowbed', 12.0, 2.5, 0.8, 40000, 'Low bed trailer for heavy machinery and equipment', true, NOW()),
    
    (gen_random_uuid(), 'Refrigerated Trailer', 'Refrigerated', 13.6, 2.5, 2.6, 22000, 'Temperature-controlled trailer for perishable goods (-25°C to +25°C)', true, NOW()),
    
    (gen_random_uuid(), 'Water Tanker Truck', 'Tanker', 8.0, 2.4, 3.0, 20000, 'Potable water tanker truck with pump system', true, NOW()),
    
    (gen_random_uuid(), 'Tow Truck - Flatbed', 'Recovery', 6.0, 2.3, 1.0, 3000, 'Flatbed tow truck for vehicle recovery and transport', true, NOW()),
    (gen_random_uuid(), 'Recovery Truck - Heavy Duty', 'Recovery', 8.0, 2.5, 2.0, 10000, 'Heavy-duty recovery truck with crane for large vehicles', true, NOW())
ON CONFLICT (id) DO NOTHING;

CREATE INDEX IF NOT EXISTS idx_vehicle_types_category ON vehicle_types(category);
CREATE INDEX IF NOT EXISTS idx_vehicle_types_is_active ON vehicle_types(is_active);
CREATE INDEX IF NOT EXISTS idx_vehicle_types_capacity ON vehicle_types(capacity_kg);

COMMENT ON TABLE vehicle_types IS 'Vehicle types with standard dimensions and capacities for World of Logistics KSA';
COMMENT ON COLUMN vehicle_types.category IS 'Vehicle category: DryVan, Flatbed, Refrigerated, Canopy, Curtainside, Lowbed, Tanker, Recovery';
COMMENT ON COLUMN vehicle_types.length_meters IS 'Standard cargo area length in meters';
COMMENT ON COLUMN vehicle_types.width_meters IS 'Standard cargo area width in meters';
COMMENT ON COLUMN vehicle_types.height_meters IS 'Standard cargo area height in meters (0.5 for open flatbeds)';
COMMENT ON COLUMN vehicle_types.capacity_kg IS 'Maximum payload capacity in kilograms';
