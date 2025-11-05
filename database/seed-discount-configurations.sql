
INSERT INTO discount_configurations (id, discount_type, percentage, description, is_active, created_at)
VALUES
    (gen_random_uuid(), 'BackloadDiscount', 0.15, 'Backload trip discount (15% off)', true, NOW()),
    (gen_random_uuid(), 'FlexibleDateDiscount', 0.05, 'Flexible date discount (5% off)', true, NOW()),
    
    (gen_random_uuid(), 'SharedLoadMinDiscount', 0.10, 'Shared load minimum discount (10% off)', true, NOW()),
    (gen_random_uuid(), 'SharedLoadMaxDiscount', 0.20, 'Shared load maximum discount (20% off at full capacity)', true, NOW()),
    
    (gen_random_uuid(), 'LoyaltyBronzeDiscount', 0.00, 'Bronze tier discount (0% - new customers)', true, NOW()),
    (gen_random_uuid(), 'LoyaltySilverDiscount', 0.05, 'Silver tier discount (5% - 20+ bookings or SAR 20k+ spent)', true, NOW()),
    (gen_random_uuid(), 'LoyaltyGoldDiscount', 0.10, 'Gold tier discount (10% - 50+ bookings or SAR 50k+ spent)', true, NOW())
ON CONFLICT (id) DO NOTHING;

CREATE INDEX IF NOT EXISTS idx_discount_configurations_discount_type ON discount_configurations(discount_type);
CREATE INDEX IF NOT EXISTS idx_discount_configurations_is_active ON discount_configurations(is_active);

COMMENT ON TABLE discount_configurations IS 'Configurable discounts for World of Logistics KSA - allows admin to modify discount percentages without code changes';
COMMENT ON COLUMN discount_configurations.discount_type IS 'Type of discount: BackloadDiscount, FlexibleDateDiscount, SharedLoadMinDiscount, etc.';
COMMENT ON COLUMN discount_configurations.percentage IS 'Discount percentage as decimal (0.15 = 15% off)';
