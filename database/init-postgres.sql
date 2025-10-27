

CREATE DATABASE wol_identity;
CREATE DATABASE wol_booking;
CREATE DATABASE wol_vehicle;
CREATE DATABASE wol_pricing;
CREATE DATABASE wol_backload;
CREATE DATABASE wol_tracking;
CREATE DATABASE wol_payment;
CREATE DATABASE wol_document;
CREATE DATABASE wol_compliance;


\c wol_identity;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_type VARCHAR(50) NOT NULL CHECK (user_type IN ('Individual', 'Commercial', 'ServiceProvider')),
    mobile_number VARCHAR(20) NOT NULL UNIQUE,
    email VARCHAR(255),
    iqama_number VARCHAR(50),
    id_number VARCHAR(50),
    password_hash VARCHAR(255) NOT NULL,
    is_email_verified BOOLEAN DEFAULT FALSE,
    is_mobile_verified BOOLEAN DEFAULT FALSE,
    preferred_language VARCHAR(10) DEFAULT 'en' CHECK (preferred_language IN ('en', 'ar', 'ur')),
    status VARCHAR(20) DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'Suspended', 'Deleted')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

CREATE INDEX idx_users_mobile ON users(mobile_number);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_iqama ON users(iqama_number);
CREATE INDEX idx_users_status ON users(status);
CREATE INDEX idx_users_type ON users(user_type);

CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE user_roles (
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID REFERENCES roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, role_id)
);

CREATE INDEX idx_user_roles_user ON user_roles(user_id);
CREATE INDEX idx_user_roles_role ON user_roles(role_id);

CREATE TABLE user_profiles (
    user_id UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    full_name VARCHAR(255) NOT NULL,
    commercial_license VARCHAR(100),
    commercial_license_expiry DATE,
    vat_number VARCHAR(50),
    city VARCHAR(100),
    address TEXT,
    profile_image_url TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE otp_verifications (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    mobile_number VARCHAR(20) NOT NULL,
    otp_code VARCHAR(6) NOT NULL,
    purpose VARCHAR(50) NOT NULL CHECK (purpose IN ('Registration', 'Login', 'PasswordReset', 'Verification')),
    is_verified BOOLEAN DEFAULT FALSE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_otp_mobile ON otp_verifications(mobile_number);
CREATE INDEX idx_otp_expires ON otp_verifications(expires_at);
CREATE INDEX idx_otp_purpose ON otp_verifications(purpose);

CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP
);

CREATE INDEX idx_refresh_tokens_user ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_expires ON refresh_tokens(expires_at);

CREATE TABLE user_claims (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    claim_type VARCHAR(255) NOT NULL,
    claim_value TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_user_claims_user ON user_claims(user_id);
CREATE INDEX idx_user_claims_type ON user_claims(claim_type);

INSERT INTO roles (name, description) VALUES
    ('Admin', 'System administrator with full access'),
    ('SuperAdmin', 'Super administrator with unrestricted access'),
    ('Customer', 'Customer/Shipper role'),
    ('Driver', 'Driver/Service provider role'),
    ('Support', 'Customer support role'),
    ('Finance', 'Finance team role');


\c wol_vehicle;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE vehicle_types (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    category VARCHAR(50) NOT NULL,
    capacity_tons DECIMAL(10,2),
    capacity_cubic_meters DECIMAL(10,2),
    length_meters DECIMAL(10,2),
    width_meters DECIMAL(10,2),
    height_meters DECIMAL(10,2),
    description TEXT,
    base_rate_per_km DECIMAL(10,2),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_vehicle_types_category ON vehicle_types(category);
CREATE INDEX idx_vehicle_types_active ON vehicle_types(is_active);

CREATE TABLE vehicles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    owner_id UUID NOT NULL,
    vehicle_type_id UUID REFERENCES vehicle_types(id),
    plate_number VARCHAR(50) NOT NULL UNIQUE,
    istemara_number VARCHAR(100),
    istemara_expiry DATE,
    mvpi_number VARCHAR(100),
    mvpi_expiry DATE,
    insurance_number VARCHAR(100),
    insurance_expiry DATE,
    vehicle_image_url TEXT,
    status VARCHAR(20) DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'Blocked', 'UnderMaintenance')),
    is_available BOOLEAN DEFAULT TRUE,
    current_location_lat DECIMAL(10,8),
    current_location_lng DECIMAL(11,8),
    current_city VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_vehicles_owner ON vehicles(owner_id);
CREATE INDEX idx_vehicles_type ON vehicles(vehicle_type_id);
CREATE INDEX idx_vehicles_status ON vehicles(status);
CREATE INDEX idx_vehicles_available ON vehicles(is_available);
CREATE INDEX idx_vehicles_city ON vehicles(current_city);
CREATE INDEX idx_vehicles_plate ON vehicles(plate_number);

CREATE TABLE drivers (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    owner_id UUID NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    mobile_number VARCHAR(20) NOT NULL,
    iqama_number VARCHAR(50),
    iqama_expiry DATE,
    license_number VARCHAR(100),
    license_expiry DATE,
    status VARCHAR(20) DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'Blocked', 'OnLeave')),
    rating DECIMAL(3,2) DEFAULT 0.00 CHECK (rating >= 0 AND rating <= 5),
    total_trips INTEGER DEFAULT 0,
    total_earnings DECIMAL(12,2) DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_drivers_user ON drivers(user_id);
CREATE INDEX idx_drivers_owner ON drivers(owner_id);
CREATE INDEX idx_drivers_status ON drivers(status);
CREATE INDEX idx_drivers_mobile ON drivers(mobile_number);
CREATE INDEX idx_drivers_rating ON drivers(rating);

CREATE TABLE vehicle_driver_assignments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vehicle_id UUID REFERENCES vehicles(id) ON DELETE CASCADE,
    driver_id UUID REFERENCES drivers(id) ON DELETE CASCADE,
    is_primary BOOLEAN DEFAULT FALSE,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    unassigned_at TIMESTAMP
);

CREATE INDEX idx_assignments_vehicle ON vehicle_driver_assignments(vehicle_id);
CREATE INDEX idx_assignments_driver ON vehicle_driver_assignments(driver_id);
CREATE INDEX idx_assignments_active ON vehicle_driver_assignments(unassigned_at) WHERE unassigned_at IS NULL;

CREATE TABLE vehicle_availability_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vehicle_id UUID REFERENCES vehicles(id) ON DELETE CASCADE,
    is_available BOOLEAN NOT NULL,
    location_lat DECIMAL(10,8),
    location_lng DECIMAL(11,8),
    city VARCHAR(100),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_availability_history_vehicle ON vehicle_availability_history(vehicle_id);
CREATE INDEX idx_availability_history_date ON vehicle_availability_history(changed_at);

INSERT INTO vehicle_types (name, category, capacity_tons, capacity_cubic_meters, length_meters, width_meters, height_meters, base_rate_per_km) VALUES
    ('Light Truck - Box', 'Light', 3.5, 15.0, 4.5, 2.0, 2.2, 2.5),
    ('Light Truck - Flatbed', 'Light', 3.5, 15.0, 4.5, 2.0, 2.2, 2.5),
    ('Light Truck - Reefer', 'Light', 3.5, 15.0, 4.5, 2.0, 2.2, 3.0),
    ('Light Truck - Canopy', 'Light', 3.5, 15.0, 4.5, 2.0, 2.2, 2.5),
    ('Box Truck - Rigid', 'Box', 10.0, 40.0, 6.0, 2.4, 2.6, 3.5),
    ('Box Truck - Semi-trailer', 'Box', 20.0, 80.0, 12.0, 2.4, 2.6, 4.0),
    ('Curtain-side Trailer', 'Curtain', 20.0, 80.0, 12.0, 2.4, 2.6, 4.0),
    ('Flatbed Trailer', 'Flatbed', 25.0, 90.0, 12.0, 2.4, 0.0, 3.8),
    ('Low Bed Trailer', 'LowBed', 40.0, 100.0, 15.0, 3.0, 0.0, 5.0),
    ('Refrigerated Trailer', 'Reefer', 20.0, 80.0, 12.0, 2.4, 2.6, 4.5),
    ('Water Tanker', 'Tanker', 30.0, 30.0, 10.0, 2.5, 3.0, 4.0),
    ('Tow Truck', 'Tow', 5.0, 0.0, 6.0, 2.5, 2.5, 3.0);


\c wol_booking;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE bookings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_number VARCHAR(50) NOT NULL UNIQUE,
    customer_id UUID NOT NULL,
    vehicle_id UUID,
    driver_id UUID,
    vehicle_type_id UUID NOT NULL,
    
    origin_address TEXT NOT NULL,
    origin_lat DECIMAL(10,8) NOT NULL,
    origin_lng DECIMAL(11,8) NOT NULL,
    origin_city VARCHAR(100) NOT NULL,
    
    destination_address TEXT NOT NULL,
    destination_lat DECIMAL(10,8) NOT NULL,
    destination_lng DECIMAL(11,8) NOT NULL,
    destination_city VARCHAR(100) NOT NULL,
    
    distance_km DECIMAL(10,2),
    estimated_duration_minutes INTEGER,
    
    pickup_date DATE NOT NULL,
    pickup_time TIME NOT NULL,
    is_whole_day BOOLEAN DEFAULT FALSE,
    is_flexible_date BOOLEAN DEFAULT FALSE,
    
    cargo_type VARCHAR(50),
    gross_weight_kg DECIMAL(10,2),
    dimensions_length_cm DECIMAL(10,2),
    dimensions_width_cm DECIMAL(10,2),
    dimensions_height_cm DECIMAL(10,2),
    number_of_boxes INTEGER,
    cargo_image_url TEXT,
    
    shipper_name VARCHAR(255),
    shipper_mobile VARCHAR(20),
    shipper_alternate_mobile VARCHAR(20),
    receiver_name VARCHAR(255),
    receiver_mobile VARCHAR(20),
    receiver_alternate_mobile VARCHAR(20),
    
    booking_type VARCHAR(20) DEFAULT 'OneWay' CHECK (booking_type IN ('OneWay', 'Backload', 'Shared')),
    is_backload BOOLEAN DEFAULT FALSE,
    is_shared_load BOOLEAN DEFAULT FALSE,
    
    base_fare DECIMAL(10,2) NOT NULL,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    waiting_charges DECIMAL(10,2) DEFAULT 0,
    total_fare DECIMAL(10,2) NOT NULL,
    vat_amount DECIMAL(10,2) DEFAULT 0,
    final_amount DECIMAL(10,2) NOT NULL,
    
    status VARCHAR(50) DEFAULT 'Pending' CHECK (status IN (
        'Pending', 'DriverAssigned', 'Accepted', 'DriverReached', 
        'Loading', 'InTransit', 'Delivered', 'Completed', 'Cancelled'
    )),
    
    driver_assigned_at TIMESTAMP,
    driver_accepted_at TIMESTAMP,
    driver_reached_at TIMESTAMP,
    loading_started_at TIMESTAMP,
    loading_completed_at TIMESTAMP,
    trip_started_at TIMESTAMP,
    delivered_at TIMESTAMP,
    completed_at TIMESTAMP,
    cancelled_at TIMESTAMP,
    
    free_time_minutes INTEGER DEFAULT 120,
    free_time_end_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_bookings_customer ON bookings(customer_id);
CREATE INDEX idx_bookings_driver ON bookings(driver_id);
CREATE INDEX idx_bookings_vehicle ON bookings(vehicle_id);
CREATE INDEX idx_bookings_status ON bookings(status);
CREATE INDEX idx_bookings_pickup_date ON bookings(pickup_date);
CREATE INDEX idx_bookings_number ON bookings(booking_number);
CREATE INDEX idx_bookings_origin_city ON bookings(origin_city);
CREATE INDEX idx_bookings_destination_city ON bookings(destination_city);
CREATE INDEX idx_bookings_type ON bookings(booking_type);
CREATE INDEX idx_bookings_created ON bookings(created_at);

CREATE TABLE booking_status_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    from_status VARCHAR(50),
    to_status VARCHAR(50) NOT NULL,
    changed_by UUID,
    reason TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_status_history_booking ON booking_status_history(booking_id);
CREATE INDEX idx_status_history_date ON booking_status_history(created_at);

CREATE TABLE booking_cancellations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    cancelled_by UUID NOT NULL,
    cancellation_reason TEXT,
    cancellation_fee DECIMAL(10,2) DEFAULT 0,
    refund_amount DECIMAL(10,2) DEFAULT 0,
    cancelled_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_cancellations_booking ON booking_cancellations(booking_id);
CREATE INDEX idx_cancellations_user ON booking_cancellations(cancelled_by);

CREATE TABLE waiting_charge_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    free_time_end_at TIMESTAMP NOT NULL,
    waiting_started_at TIMESTAMP NOT NULL,
    waiting_ended_at TIMESTAMP,
    hours_waited DECIMAL(10,2),
    charge_per_hour DECIMAL(10,2) DEFAULT 100,
    total_charge DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_waiting_logs_booking ON waiting_charge_logs(booking_id);

CREATE TABLE booking_documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID REFERENCES bookings(id) ON DELETE CASCADE,
    document_type VARCHAR(50) NOT NULL,
    file_url TEXT NOT NULL,
    uploaded_by UUID NOT NULL,
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_booking_docs_booking ON booking_documents(booking_id);


\c wol_pricing;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE pricing_rules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    rule_type VARCHAR(50) NOT NULL CHECK (rule_type IN ('BaseFare', 'PerKm', 'PerHour', 'Surge')),
    vehicle_type_id UUID,
    origin_city VARCHAR(100),
    destination_city VARCHAR(100),
    base_amount DECIMAL(10,2),
    rate_per_km DECIMAL(10,2),
    rate_per_hour DECIMAL(10,2),
    minimum_fare DECIMAL(10,2),
    is_active BOOLEAN DEFAULT TRUE,
    effective_from DATE,
    effective_to DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_pricing_vehicle_type ON pricing_rules(vehicle_type_id);
CREATE INDEX idx_pricing_cities ON pricing_rules(origin_city, destination_city);
CREATE INDEX idx_pricing_active ON pricing_rules(is_active);
CREATE INDEX idx_pricing_dates ON pricing_rules(effective_from, effective_to);

CREATE TABLE route_base_fares (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    origin_city VARCHAR(100) NOT NULL,
    destination_city VARCHAR(100) NOT NULL,
    vehicle_type_id UUID NOT NULL,
    base_fare DECIMAL(10,2) NOT NULL,
    distance_km DECIMAL(10,2) NOT NULL,
    estimated_duration_minutes INTEGER NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(origin_city, destination_city, vehicle_type_id)
);

CREATE INDEX idx_route_fares_cities ON route_base_fares(origin_city, destination_city);
CREATE INDEX idx_route_fares_vehicle ON route_base_fares(vehicle_type_id);
CREATE INDEX idx_route_fares_active ON route_base_fares(is_active);

CREATE TABLE discount_rules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    discount_type VARCHAR(50) NOT NULL CHECK (discount_type IN ('Backload', 'FlexibleDate', 'SharedLoad', 'Loyalty', 'Promotional')),
    discount_percentage DECIMAL(5,2),
    discount_amount DECIMAL(10,2),
    max_discount_amount DECIMAL(10,2),
    conditions JSONB,
    is_active BOOLEAN DEFAULT TRUE,
    valid_from DATE,
    valid_to DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_discount_type ON discount_rules(discount_type);
CREATE INDEX idx_discount_active ON discount_rules(is_active);
CREATE INDEX idx_discount_dates ON discount_rules(valid_from, valid_to);

CREATE TABLE surge_pricing_rules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    city VARCHAR(100) NOT NULL,
    day_of_week INTEGER CHECK (day_of_week >= 0 AND day_of_week <= 6),
    time_from TIME,
    time_to TIME,
    surge_multiplier DECIMAL(5,2) NOT NULL CHECK (surge_multiplier >= 1.0),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_surge_city ON surge_pricing_rules(city);
CREATE INDEX idx_surge_active ON surge_pricing_rules(is_active);

CREATE TABLE pricing_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID,
    vehicle_type_id UUID NOT NULL,
    origin_city VARCHAR(100) NOT NULL,
    destination_city VARCHAR(100) NOT NULL,
    distance_km DECIMAL(10,2) NOT NULL,
    base_fare DECIMAL(10,2) NOT NULL,
    surge_multiplier DECIMAL(5,2) DEFAULT 1.0,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    total_fare DECIMAL(10,2) NOT NULL,
    calculated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_pricing_history_booking ON pricing_history(booking_id);
CREATE INDEX idx_pricing_history_route ON pricing_history(origin_city, destination_city);
CREATE INDEX idx_pricing_history_date ON pricing_history(calculated_at);

INSERT INTO discount_rules (name, discount_type, discount_percentage, is_active) VALUES
    ('Backload Discount', 'Backload', 15.00, TRUE),
    ('Flexible Date Discount', 'FlexibleDate', 5.00, TRUE),
    ('Shared Load Discount', 'SharedLoad', 15.00, TRUE),
    ('Gold Customer Loyalty', 'Loyalty', 5.00, TRUE);


\c wol_backload;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE backload_availability (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    vehicle_id UUID NOT NULL,
    driver_id UUID NOT NULL,
    
    current_location_address TEXT,
    current_location_lat DECIMAL(10,8),
    current_location_lng DECIMAL(11,8),
    current_city VARCHAR(100) NOT NULL,
    
    return_city VARCHAR(100) NOT NULL,
    return_location_lat DECIMAL(10,8),
    return_location_lng DECIMAL(11,8),
    
    available_from TIMESTAMP NOT NULL,
    available_to TIMESTAMP NOT NULL,
    
    capacity_available_tons DECIMAL(10,2),
    capacity_available_cubic_meters DECIMAL(10,2),
    
    preferred_cargo_types VARCHAR(255),
    minimum_price_expected DECIMAL(10,2),
    
    status VARCHAR(20) DEFAULT 'Available' CHECK (status IN ('Available', 'Matched', 'Expired', 'Cancelled')),
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_backload_vehicle ON backload_availability(vehicle_id);
CREATE INDEX idx_backload_driver ON backload_availability(driver_id);
CREATE INDEX idx_backload_cities ON backload_availability(current_city, return_city);
CREATE INDEX idx_backload_dates ON backload_availability(available_from, available_to);
CREATE INDEX idx_backload_status ON backload_availability(status);

CREATE TABLE backload_matches (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    availability_id UUID REFERENCES backload_availability(id) ON DELETE CASCADE,
    booking_id UUID NOT NULL,
    match_score DECIMAL(5,2) NOT NULL,
    estimated_discount DECIMAL(10,2),
    distance_km DECIMAL(10,2),
    status VARCHAR(20) DEFAULT 'Suggested' CHECK (status IN ('Suggested', 'Accepted', 'Rejected', 'Expired')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_matches_availability ON backload_matches(availability_id);
CREATE INDEX idx_matches_booking ON backload_matches(booking_id);
CREATE INDEX idx_matches_status ON backload_matches(status);
CREATE INDEX idx_matches_score ON backload_matches(match_score);

CREATE TABLE route_utilization (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    origin_city VARCHAR(100) NOT NULL,
    destination_city VARCHAR(100) NOT NULL,
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    outbound_trips INTEGER DEFAULT 0,
    return_trips INTEGER DEFAULT 0,
    backload_matches INTEGER DEFAULT 0,
    utilization_rate DECIMAL(5,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(origin_city, destination_city, period_start, period_end)
);

CREATE INDEX idx_route_util_cities ON route_utilization(origin_city, destination_city);
CREATE INDEX idx_route_util_period ON route_utilization(period_start, period_end);


\c wol_tracking;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "postgis";

CREATE TABLE trip_tracking (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    trip_id UUID NOT NULL UNIQUE,
    booking_id UUID NOT NULL,
    driver_id UUID NOT NULL,
    vehicle_id UUID NOT NULL,
    
    origin_location GEOGRAPHY(POINT, 4326) NOT NULL,
    destination_location GEOGRAPHY(POINT, 4326) NOT NULL,
    current_location GEOGRAPHY(POINT, 4326),
    
    status VARCHAR(50) NOT NULL,
    
    distance_covered_km DECIMAL(10,2) DEFAULT 0,
    distance_remaining_km DECIMAL(10,2),
    
    eta TIMESTAMP,
    
    started_at TIMESTAMP,
    completed_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_trip_tracking_trip ON trip_tracking(trip_id);
CREATE INDEX idx_trip_tracking_booking ON trip_tracking(booking_id);
CREATE INDEX idx_trip_tracking_driver ON trip_tracking(driver_id);
CREATE INDEX idx_trip_tracking_status ON trip_tracking(status);
CREATE INDEX idx_trip_tracking_location ON trip_tracking USING GIST(current_location);

CREATE TABLE geofence_events (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    trip_id UUID NOT NULL,
    event_type VARCHAR(50) NOT NULL CHECK (event_type IN ('EnteredOrigin', 'LeftOrigin', 'EnteredDestination', 'LeftDestination')),
    location GEOGRAPHY(POINT, 4326) NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_geofence_trip ON geofence_events(trip_id);
CREATE INDEX idx_geofence_type ON geofence_events(event_type);
CREATE INDEX idx_geofence_timestamp ON geofence_events(timestamp);

CREATE TABLE arrival_proofs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    trip_id UUID NOT NULL,
    booking_id UUID NOT NULL,
    location GEOGRAPHY(POINT, 4326) NOT NULL,
    photo_url TEXT NOT NULL,
    timestamp TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_arrival_proofs_trip ON arrival_proofs(trip_id);
CREATE INDEX idx_arrival_proofs_booking ON arrival_proofs(booking_id);


\c wol_payment;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_id UUID NOT NULL,
    customer_id UUID NOT NULL,
    
    payment_method VARCHAR(50) NOT NULL CHECK (payment_method IN ('Card', 'Wallet', 'BankTransfer', 'Cash', 'Invoice')),
    payment_provider VARCHAR(50),
    
    amount DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'SAR',
    
    status VARCHAR(50) DEFAULT 'Pending' CHECK (status IN (
        'Pending', 'Authorized', 'Captured', 'Failed', 'Refunded', 'PartiallyRefunded', 'Cancelled'
    )),
    
    payment_intent_id VARCHAR(255),
    transaction_id VARCHAR(255),
    
    authorized_at TIMESTAMP,
    captured_at TIMESTAMP,
    failed_at TIMESTAMP,
    failure_reason TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_payments_booking ON payments(booking_id);
CREATE INDEX idx_payments_customer ON payments(customer_id);
CREATE INDEX idx_payments_status ON payments(status);
CREATE INDEX idx_payments_method ON payments(payment_method);
CREATE INDEX idx_payments_transaction ON payments(transaction_id);
CREATE INDEX idx_payments_created ON payments(created_at);

CREATE TABLE payment_methods (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    method_type VARCHAR(50) NOT NULL CHECK (method_type IN ('Card', 'BankAccount', 'Wallet')),
    provider VARCHAR(50),
    
    card_last_four VARCHAR(4),
    card_brand VARCHAR(50),
    card_expiry_month INTEGER,
    card_expiry_year INTEGER,
    
    account_number_last_four VARCHAR(4),
    bank_name VARCHAR(100),
    
    wallet_id VARCHAR(255),
    
    is_default BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_payment_methods_user ON payment_methods(user_id);
CREATE INDEX idx_payment_methods_type ON payment_methods(method_type);
CREATE INDEX idx_payment_methods_default ON payment_methods(is_default) WHERE is_default = TRUE;

CREATE TABLE refunds (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    payment_id UUID REFERENCES payments(id) ON DELETE CASCADE,
    booking_id UUID NOT NULL,
    
    refund_amount DECIMAL(10,2) NOT NULL,
    refund_reason TEXT,
    
    status VARCHAR(50) DEFAULT 'Pending' CHECK (status IN ('Pending', 'Processing', 'Processed', 'Failed', 'Cancelled')),
    
    refund_transaction_id VARCHAR(255),
    processed_at TIMESTAMP,
    failed_at TIMESTAMP,
    failure_reason TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_refunds_payment ON refunds(payment_id);
CREATE INDEX idx_refunds_booking ON refunds(booking_id);
CREATE INDEX idx_refunds_status ON refunds(status);

CREATE TABLE driver_settlements (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    driver_id UUID NOT NULL,
    
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    
    total_trips INTEGER DEFAULT 0,
    total_earnings DECIMAL(12,2) DEFAULT 0,
    platform_commission DECIMAL(12,2) DEFAULT 0,
    penalties DECIMAL(12,2) DEFAULT 0,
    bonuses DECIMAL(12,2) DEFAULT 0,
    net_amount DECIMAL(12,2) NOT NULL,
    
    status VARCHAR(50) DEFAULT 'Pending' CHECK (status IN ('Pending', 'Processing', 'Processed', 'Paid', 'Failed')),
    
    payment_method VARCHAR(50),
    payment_reference VARCHAR(255),
    paid_at TIMESTAMP,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_settlements_driver ON driver_settlements(driver_id);
CREATE INDEX idx_settlements_period ON driver_settlements(period_start, period_end);
CREATE INDEX idx_settlements_status ON driver_settlements(status);

CREATE TABLE invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    invoice_number VARCHAR(50) NOT NULL UNIQUE,
    booking_id UUID NOT NULL,
    customer_id UUID NOT NULL,
    
    subtotal DECIMAL(10,2) NOT NULL,
    vat_amount DECIMAL(10,2) NOT NULL,
    total_amount DECIMAL(10,2) NOT NULL,
    
    status VARCHAR(50) DEFAULT 'Draft' CHECK (status IN ('Draft', 'Issued', 'Paid', 'Cancelled', 'Overdue')),
    
    issued_at TIMESTAMP,
    due_date DATE,
    paid_at TIMESTAMP,
    
    invoice_url TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_invoices_number ON invoices(invoice_number);
CREATE INDEX idx_invoices_booking ON invoices(booking_id);
CREATE INDEX idx_invoices_customer ON invoices(customer_id);
CREATE INDEX idx_invoices_status ON invoices(status);
CREATE INDEX idx_invoices_due_date ON invoices(due_date);


\c wol_document;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE document_types (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL UNIQUE,
    category VARCHAR(50) NOT NULL CHECK (category IN ('Vehicle', 'Driver', 'User', 'Booking')),
    is_required BOOLEAN DEFAULT TRUE,
    has_expiry BOOLEAN DEFAULT TRUE,
    expiry_alert_days INTEGER DEFAULT 30,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    document_type_id UUID REFERENCES document_types(id),
    entity_type VARCHAR(50) NOT NULL CHECK (entity_type IN ('Vehicle', 'Driver', 'User', 'Booking')),
    entity_id UUID NOT NULL,
    
    document_number VARCHAR(100),
    issue_date DATE,
    expiry_date DATE,
    
    file_url TEXT NOT NULL,
    file_name VARCHAR(255),
    file_size_bytes BIGINT,
    mime_type VARCHAR(100),
    
    verification_status VARCHAR(50) DEFAULT 'Pending' CHECK (verification_status IN ('Pending', 'Verified', 'Rejected', 'Expired')),
    verified_by UUID,
    verified_at TIMESTAMP,
    rejection_reason TEXT,
    
    extracted_data JSONB,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_documents_entity ON documents(entity_type, entity_id);
CREATE INDEX idx_documents_type ON documents(document_type_id);
CREATE INDEX idx_documents_expiry ON documents(expiry_date);
CREATE INDEX idx_documents_verification ON documents(verification_status);

CREATE TABLE document_verification_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    document_id UUID REFERENCES documents(id) ON DELETE CASCADE,
    verified_by UUID NOT NULL,
    previous_status VARCHAR(50),
    new_status VARCHAR(50) NOT NULL,
    notes TEXT,
    verified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_verification_history_document ON document_verification_history(document_id);

INSERT INTO document_types (name, category, is_required, has_expiry, expiry_alert_days) VALUES
    ('Istemara', 'Vehicle', TRUE, TRUE, 30),
    ('MVPI', 'Vehicle', TRUE, TRUE, 30),
    ('Vehicle Insurance', 'Vehicle', TRUE, TRUE, 30),
    ('Driver Iqama', 'Driver', TRUE, TRUE, 30),
    ('Driver License', 'Driver', TRUE, TRUE, 30),
    ('Commercial License', 'User', TRUE, TRUE, 60),
    ('VAT Certificate', 'User', FALSE, FALSE, NULL),
    ('Cargo Photo', 'Booking', FALSE, FALSE, NULL),
    ('Delivery Proof', 'Booking', FALSE, FALSE, NULL);


\c wol_compliance;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE ban_times (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    city VARCHAR(100) NOT NULL,
    vehicle_type_id UUID,
    
    ban_start_time TIME NOT NULL,
    ban_end_time TIME NOT NULL,
    
    days_of_week INTEGER[] NOT NULL,
    
    is_active BOOLEAN DEFAULT TRUE,
    reason TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_ban_times_city ON ban_times(city);
CREATE INDEX idx_ban_times_active ON ban_times(is_active);

CREATE TABLE compliance_rules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    rule_name VARCHAR(255) NOT NULL,
    rule_type VARCHAR(50) NOT NULL CHECK (rule_type IN ('DocumentExpiry', 'BANTime', 'VehicleAge', 'DriverAge', 'Custom')),
    entity_type VARCHAR(50) NOT NULL CHECK (entity_type IN ('Vehicle', 'Driver', 'Booking')),
    severity VARCHAR(20) NOT NULL CHECK (severity IN ('Low', 'Medium', 'High', 'Critical')),
    is_blocking BOOLEAN DEFAULT FALSE,
    rule_config JSONB,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_compliance_rules_type ON compliance_rules(rule_type);
CREATE INDEX idx_compliance_rules_entity ON compliance_rules(entity_type);
CREATE INDEX idx_compliance_rules_active ON compliance_rules(is_active);

CREATE TABLE compliance_checks (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    entity_type VARCHAR(50) NOT NULL,
    entity_id UUID NOT NULL,
    rule_id UUID REFERENCES compliance_rules(id),
    
    check_result VARCHAR(20) NOT NULL CHECK (check_result IN ('Pass', 'Fail', 'Warning')),
    details JSONB,
    
    checked_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_compliance_checks_entity ON compliance_checks(entity_type, entity_id);
CREATE INDEX idx_compliance_checks_rule ON compliance_checks(rule_id);
CREATE INDEX idx_compliance_checks_result ON compliance_checks(check_result);
CREATE INDEX idx_compliance_checks_date ON compliance_checks(checked_at);

CREATE TABLE compliance_violations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    entity_type VARCHAR(50) NOT NULL CHECK (entity_type IN ('Vehicle', 'Driver', 'Booking', 'User')),
    entity_id UUID NOT NULL,
    
    violation_type VARCHAR(100) NOT NULL,
    severity VARCHAR(20) NOT NULL CHECK (severity IN ('Low', 'Medium', 'High', 'Critical')),
    
    description TEXT,
    
    is_blocking BOOLEAN DEFAULT FALSE,
    resolved_at TIMESTAMP,
    resolved_by UUID,
    resolution_notes TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_violations_entity ON compliance_violations(entity_type, entity_id);
CREATE INDEX idx_violations_type ON compliance_violations(violation_type);
CREATE INDEX idx_violations_severity ON compliance_violations(severity);
CREATE INDEX idx_violations_resolved ON compliance_violations(resolved_at);
CREATE INDEX idx_violations_blocking ON compliance_violations(is_blocking) WHERE is_blocking = TRUE;

CREATE TABLE document_expiry_alerts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    document_id UUID NOT NULL,
    entity_type VARCHAR(50) NOT NULL,
    entity_id UUID NOT NULL,
    document_type VARCHAR(100) NOT NULL,
    expiry_date DATE NOT NULL,
    days_until_expiry INTEGER NOT NULL,
    alert_sent BOOLEAN DEFAULT FALSE,
    alert_sent_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_expiry_alerts_document ON document_expiry_alerts(document_id);
CREATE INDEX idx_expiry_alerts_entity ON document_expiry_alerts(entity_type, entity_id);
CREATE INDEX idx_expiry_alerts_expiry ON document_expiry_alerts(expiry_date);
CREATE INDEX idx_expiry_alerts_sent ON document_expiry_alerts(alert_sent);

INSERT INTO ban_times (city, ban_start_time, ban_end_time, days_of_week, reason) VALUES
    ('Riyadh', '11:00:00', '16:00:00', ARRAY[0,1,2,3,4,5,6], 'Peak hours restriction'),
    ('Jeddah', '12:00:00', '15:00:00', ARRAY[0,1,2,3,4,5,6], 'Peak hours restriction'),
    ('Dammam', '11:30:00', '15:30:00', ARRAY[0,1,2,3,4,5,6], 'Peak hours restriction'),
    ('Mecca', '10:00:00', '17:00:00', ARRAY[0,1,2,3,4,5,6], 'Religious and peak hours restriction'),
    ('Medina', '10:00:00', '17:00:00', ARRAY[0,1,2,3,4,5,6], 'Religious and peak hours restriction');


DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'wol_app') THEN
        CREATE USER wol_app WITH PASSWORD 'wol_app_password';
    END IF;
END
$$;

\c wol_identity;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_booking;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_vehicle;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_pricing;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_backload;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_tracking;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_payment;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_document;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;

\c wol_compliance;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO wol_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO wol_app;


\echo 'Database initialization completed successfully!'
\echo 'All databases, tables, indexes, and initial data have been created.'
\echo ''
\echo 'Created databases:'
\echo '  - wol_identity'
\echo '  - wol_booking'
\echo '  - wol_vehicle'
\echo '  - wol_pricing'
\echo '  - wol_backload'
\echo '  - wol_tracking'
\echo '  - wol_payment'
\echo '  - wol_document'
\echo '  - wol_compliance'
\echo ''
\echo 'Application user: wol_app'
\echo 'Please change the default password in production!'
