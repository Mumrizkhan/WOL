
INSERT INTO fee_configurations (id, fee_type, amount, description, is_active, created_at)
VALUES
    (gen_random_uuid(), 'WaitingChargePerHour', 100.00, 'Hourly waiting charge after 2-hour free time (SAR 100/hour)', true, NOW()),
    
    (gen_random_uuid(), 'CancellationFeeShipperNoShow', 250.00, 'Commercial customer no-show after 30 minutes (SAR 250)', true, NOW()),
    (gen_random_uuid(), 'CancellationFeeDriverWait', 500.00, 'Driver waited 1+ hour with no loading (SAR 500 to shipper)', true, NOW()),
    (gen_random_uuid(), 'CancellationFeeAdminAfter30Mins', 100.00, 'Customer cancellation after 30 mins, no driver assigned (SAR 100)', true, NOW()),
    (gen_random_uuid(), 'CancellationFeeDriverEarlyPercentage', 0.50, 'Driver cancellation before free time ends (50% of trip fare)', true, NOW()),
    
    (gen_random_uuid(), 'FreeTimeHours', 2.00, 'Free time for loading and offloading (2 hours total)', true, NOW()),
    
    (gen_random_uuid(), 'PaymentDeadlineIndividualMinutes', 15.00, 'Payment deadline for Individual customers (15 minutes)', true, NOW()),
    (gen_random_uuid(), 'DriverAssignmentTimeoutMinutes', 30.00, 'Auto-refund if no driver assigned within 30 minutes', true, NOW())
ON CONFLICT (id) DO NOTHING;

CREATE INDEX IF NOT EXISTS idx_fee_configurations_fee_type ON fee_configurations(fee_type);
CREATE INDEX IF NOT EXISTS idx_fee_configurations_is_active ON fee_configurations(is_active);

COMMENT ON TABLE fee_configurations IS 'Configurable fees for World of Logistics KSA - allows admin to modify fees without code changes';
COMMENT ON COLUMN fee_configurations.fee_type IS 'Type of fee: WaitingChargePerHour, CancellationFeeShipperNoShow, etc.';
COMMENT ON COLUMN fee_configurations.amount IS 'Fee amount in SAR or percentage (0-1 for percentages)';
