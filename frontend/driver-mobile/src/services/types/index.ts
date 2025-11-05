export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken?: string;
  userId: string;
  email: string;
  userType: string;
  roles: string[];
}

export interface RegisterUserRequest {
  email: string;
  password: string;
  phoneNumber: string;
  userType: 'Individual' | 'Commercial' | 'ServiceProvider';
  fullName?: string;
  companyName?: string;
  iqamaNumber?: string;
  commercialLicense?: string;
  vatNumber?: string;
}

export interface RegisterUserResponse {
  userId: string;
  email: string;
  message: string;
}

export interface CreateBookingRequest {
  customerId: string;
  bookingType: 'OneWay' | 'Backload' | 'SharedLoad';
  vehicleTypeId: string;
  pickupLocation: LocationDto;
  deliveryLocation: LocationDto;
  pickupDateTime: string;
  cargoDetails: CargoDetailsDto;
  contactInfo: ContactInfoDto;
  isFlexibleDate?: boolean;
}

export interface LocationDto {
  address: string;
  latitude: number;
  longitude: number;
  city: string;
}

export interface CargoDetailsDto {
  description: string;
  weight: number;
  volume?: number;
  quantity?: number;
  specialInstructions?: string;
}

export interface ContactInfoDto {
  name: string;
  phoneNumber: string;
  alternatePhoneNumber?: string;
}

export interface CreateBookingResponse {
  bookingId: string;
  bookingNumber: string;
  totalPrice: number;
  paymentDeadline: string;
  message: string;
}

export interface Booking {
  id: string;
  bookingNumber: string;
  customerId: string;
  customerName: string;
  driverId?: string;
  driverName?: string;
  vehicleId?: string;
  bookingType: string;
  status: string;
  paymentStatus: string;
  pickupLocation: LocationDto;
  deliveryLocation: LocationDto;
  pickupDateTime: string;
  cargoDetails: CargoDetailsDto;
  totalPrice: number;
  distance: number;
  createdAt: string;
  updatedAt: string;
}

export interface CalculatePriceRequest {
  customerId: string;
  vehicleType: string;
  fromCity: string;
  toCity: string;
  distance: number;
  weight: number;
  isBackload?: boolean;
  isFlexibleDate?: boolean;
  isSharedLoad?: boolean;
  capacityUtilization?: number;
  bookingDateTime: string;
  waitingHours?: number;
  isCancelled?: boolean;
}

export interface PriceLineItem {
  description: string;
  amount: number;
  type: 'Charge' | 'Discount';
}

export interface CalculatePriceResponse {
  basePrice: number;
  distancePrice: number;
  weightPrice: number;
  subTotal: number;
  backloadDiscount: number;
  flexibleDateDiscount: number;
  sharedLoadDiscount: number;
  loyaltyDiscount: number;
  totalDiscount: number;
  surgeAmount: number;
  waitingCharges: number;
  cancellationFee: number;
  totalPrice: number;
  lineItems: PriceLineItem[];
}

export interface Vehicle {
  id: string;
  vehicleNumber: string;
  vehicleType: string;
  make: string;
  model: string;
  year: number;
  capacity: number;
  status: string;
  driverId?: string;
  driverName?: string;
  createdAt: string;
}

export interface RegisterVehicleRequest {
  vehicleNumber: string;
  vehicleTypeId: string;
  make: string;
  model: string;
  year: number;
  capacity: number;
  driverId?: string;
}

export interface ProcessPaymentRequest {
  bookingId: string;
  amount: number;
  paymentMethod: 'CreditCard' | 'DebitCard' | 'Cash' | 'BankTransfer';
  paymentDetails?: any;
}

export interface ProcessPaymentResponse {
  paymentId: string;
  status: string;
  transactionId?: string;
  message: string;
}

export interface Payment {
  id: string;
  bookingId: string;
  amount: number;
  paymentMethod: string;
  status: string;
  transactionId?: string;
  processedAt: string;
  createdAt: string;
}

export interface User {
  id: string;
  email: string;
  phoneNumber: string;
  userType: string;
  fullName?: string;
  companyName?: string;
  isActive: boolean;
  roles: string[];
  createdAt: string;
}

export interface DashboardStats {
  totalBookings: number;
  activeBookings: number;
  completedBookings: number;
  totalRevenue: number;
  totalDrivers: number;
  activeDrivers: number;
  totalVehicles: number;
  complianceViolations: number;
}

export interface BookingTrend {
  date: string;
  count: number;
  revenue: number;
}

export interface RevenueTrend {
  date: string;
  amount: number;
}

export interface RecordLocationRequest {
  bookingId: string;
  driverId: string;
  latitude: number;
  longitude: number;
  speed?: number;
  heading?: number;
}

export interface LocationHistory {
  bookingId: string;
  driverId: string;
  locations: LocationPoint[];
}

export interface LocationPoint {
  latitude: number;
  longitude: number;
  timestamp: string;
  speed?: number;
  heading?: number;
}

export interface CreateBackloadOpportunityRequest {
  driverId: string;
  vehicleId: string;
  originCity: string;
  destinationCity: string;
  availableFrom: string;
  availableTo: string;
  capacity: number;
  vehicleType: string;
}

export interface BackloadOpportunity {
  id: string;
  driverId: string;
  vehicleId: string;
  originCity: string;
  destinationCity: string;
  availableFrom: string;
  availableTo: string;
  capacity: number;
  status: string;
  createdAt: string;
}

export interface RouteHeatmapData {
  origin: string;
  destination: string;
  outboundCount: number;
  returnCount: number;
  imbalancePercentage: number;
}

export interface ComplianceCheck {
  id: string;
  driverId?: string;
  vehicleId?: string;
  documentType: string;
  status: string;
  expiryDate?: string;
  checkDate: string;
}

export interface UploadDocumentRequest {
  entityId: string;
  entityType: 'Driver' | 'Vehicle' | 'Customer';
  documentType: string;
  fileName: string;
  fileContent: string;
  expiryDate?: string;
}

export interface Document {
  id: string;
  entityId: string;
  entityType: string;
  documentType: string;
  fileName: string;
  fileUrl: string;
  expiryDate?: string;
  status: string;
  uploadedAt: string;
}

export interface GenerateReportRequest {
  reportType: 'Daily' | 'Weekly' | 'Monthly';
  startDate: string;
  endDate: string;
  filters?: any;
}

export interface GenerateReportResponse {
  reportId: string;
  reportUrl: string;
  message: string;
}

export interface Report {
  id: string;
  reportType: string;
  startDate: string;
  endDate: string;
  generatedAt: string;
  fileUrl: string;
  status: string;
}

export interface BANTiming {
  id: string;
  city: string;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateBANTimingRequest {
  city: string;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
}

export interface FeeConfiguration {
  id: string;
  feeType: string;
  amount: number;
  percentage?: number;
  isActive: boolean;
  description: string;
}

export interface DiscountConfiguration {
  id: string;
  discountType: string;
  percentage: number;
  isActive: boolean;
  description: string;
}

export interface UpdateFeeConfigurationRequest {
  amount?: number;
  percentage?: number;
  description?: string;
}

export interface UpdateDiscountConfigurationRequest {
  percentage: number;
  description?: string;
}

export interface GenerateOtpRequest {
  phoneNumber: string;
  purpose: 'Login' | 'Registration' | 'PasswordReset' | '2FA' | 'PhoneVerification' | 'EmailVerification';
}

export interface VerifyOtpRequest {
  phoneNumber: string;
  code: string;
  purpose: string;
}

export interface OtpLoginRequest {
  phoneNumber: string;
  code: string;
}
