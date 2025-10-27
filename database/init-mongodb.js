

db = db.getSiblingDB('wol_notifications');

db.createCollection('notifications', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['userId', 'type', 'title', 'message', 'createdAt'],
            properties: {
                userId: { bsonType: 'string' },
                type: { 
                    enum: ['BookingCreated', 'BookingAssigned', 'BookingAccepted', 'DriverReached', 
                           'TripStarted', 'Delivered', 'PaymentReceived', 'DocumentExpiring', 
                           'ComplianceAlert', 'Promotional', 'System']
                },
                title: { bsonType: 'string' },
                message: { bsonType: 'string' },
                data: { bsonType: 'object' },
                channels: { 
                    bsonType: 'array',
                    items: { enum: ['Push', 'SMS', 'Email', 'InApp'] }
                },
                priority: { enum: ['Low', 'Medium', 'High', 'Urgent'] },
                isRead: { bsonType: 'bool' },
                readAt: { bsonType: 'date' },
                sentAt: { bsonType: 'date' },
                deliveredAt: { bsonType: 'date' },
                failedAt: { bsonType: 'date' },
                failureReason: { bsonType: 'string' },
                createdAt: { bsonType: 'date' },
                expiresAt: { bsonType: 'date' }
            }
        }
    }
});

db.notifications.createIndex({ userId: 1, createdAt: -1 });
db.notifications.createIndex({ userId: 1, isRead: 1 });
db.notifications.createIndex({ type: 1 });
db.notifications.createIndex({ createdAt: -1 });
db.notifications.createIndex({ expiresAt: 1 }, { expireAfterSeconds: 0 });
db.notifications.createIndex({ 'data.bookingId': 1 });

db.createCollection('notification_templates', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['templateId', 'type', 'title', 'message'],
            properties: {
                templateId: { bsonType: 'string' },
                type: { bsonType: 'string' },
                title: { bsonType: 'object' },
                message: { bsonType: 'object' },
                channels: { bsonType: 'array' },
                priority: { enum: ['Low', 'Medium', 'High', 'Urgent'] },
                isActive: { bsonType: 'bool' },
                createdAt: { bsonType: 'date' },
                updatedAt: { bsonType: 'date' }
            }
        }
    }
});

db.notification_templates.createIndex({ templateId: 1 }, { unique: true });
db.notification_templates.createIndex({ type: 1 });
db.notification_templates.createIndex({ isActive: 1 });

db.createCollection('push_tokens', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['userId', 'token', 'platform'],
            properties: {
                userId: { bsonType: 'string' },
                token: { bsonType: 'string' },
                platform: { enum: ['iOS', 'Android', 'Web'] },
                deviceId: { bsonType: 'string' },
                isActive: { bsonType: 'bool' },
                createdAt: { bsonType: 'date' },
                lastUsedAt: { bsonType: 'date' }
            }
        }
    }
});

db.push_tokens.createIndex({ userId: 1 });
db.push_tokens.createIndex({ token: 1 }, { unique: true });
db.push_tokens.createIndex({ platform: 1 });
db.push_tokens.createIndex({ isActive: 1 });

db.notification_templates.insertMany([
    {
        templateId: 'booking_created',
        type: 'BookingCreated',
        title: {
            en: 'Booking Created Successfully',
            ar: 'تم إنشاء الحجز بنجاح',
            ur: 'بکنگ کامیابی سے بنائی گئی'
        },
        message: {
            en: 'Your booking {{bookingNumber}} has been created. We are finding the best driver for you.',
            ar: 'تم إنشاء حجزك {{bookingNumber}}. نحن نبحث عن أفضل سائق لك.',
            ur: 'آپ کی بکنگ {{bookingNumber}} بنائی گئی ہے۔ ہم آپ کے لیے بہترین ڈرائیور تلاش کر رہے ہیں۔'
        },
        channels: ['Push', 'SMS', 'InApp'],
        priority: 'High',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        templateId: 'driver_assigned',
        type: 'BookingAssigned',
        title: {
            en: 'Driver Assigned',
            ar: 'تم تعيين السائق',
            ur: 'ڈرائیور مقرر کیا گیا'
        },
        message: {
            en: 'Driver {{driverName}} has been assigned to your booking {{bookingNumber}}.',
            ar: 'تم تعيين السائق {{driverName}} لحجزك {{bookingNumber}}.',
            ur: 'ڈرائیور {{driverName}} آپ کی بکنگ {{bookingNumber}} کے لیے مقرر کیا گیا ہے۔'
        },
        channels: ['Push', 'SMS', 'InApp'],
        priority: 'High',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        templateId: 'driver_reached',
        type: 'DriverReached',
        title: {
            en: 'Driver Has Arrived',
            ar: 'وصل السائق',
            ur: 'ڈرائیور پہنچ گیا'
        },
        message: {
            en: 'Your driver has reached the pickup location. Please be ready for loading.',
            ar: 'وصل السائق إلى موقع الاستلام. يرجى الاستعداد للتحميل.',
            ur: 'آپ کا ڈرائیور پک اپ لوکیشن پر پہنچ گیا ہے۔ براہ کرم لوڈنگ کے لیے تیار رہیں۔'
        },
        channels: ['Push', 'SMS', 'InApp'],
        priority: 'Urgent',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        templateId: 'trip_started',
        type: 'TripStarted',
        title: {
            en: 'Trip Started',
            ar: 'بدأت الرحلة',
            ur: 'سفر شروع ہو گیا'
        },
        message: {
            en: 'Your shipment is on the way. Track it in real-time.',
            ar: 'شحنتك في الطريق. تتبعها في الوقت الفعلي.',
            ur: 'آپ کی کھیپ راستے میں ہے۔ اسے ریئل ٹائم میں ٹریک کریں۔'
        },
        channels: ['Push', 'InApp'],
        priority: 'High',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        templateId: 'delivered',
        type: 'Delivered',
        title: {
            en: 'Delivered Successfully',
            ar: 'تم التسليم بنجاح',
            ur: 'کامیابی سے ڈیلیور کیا گیا'
        },
        message: {
            en: 'Your shipment has been delivered successfully. Thank you for using WOL!',
            ar: 'تم تسليم شحنتك بنجاح. شكرا لاستخدامك WOL!',
            ur: 'آپ کی کھیپ کامیابی سے ڈیلیور کر دی گئی ہے۔ WOL استعمال کرنے کا شکریہ!'
        },
        channels: ['Push', 'SMS', 'InApp'],
        priority: 'High',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        templateId: 'document_expiring',
        type: 'DocumentExpiring',
        title: {
            en: 'Document Expiring Soon',
            ar: 'الوثيقة تنتهي قريبا',
            ur: 'دستاویز جلد ختم ہو رہی ہے'
        },
        message: {
            en: 'Your {{documentType}} will expire on {{expiryDate}}. Please renew it soon.',
            ar: 'ستنتهي صلاحية {{documentType}} الخاصة بك في {{expiryDate}}. يرجى تجديدها قريبا.',
            ur: 'آپ کی {{documentType}} {{expiryDate}} کو ختم ہو جائے گی۔ براہ کرم جلد تجدید کریں۔'
        },
        channels: ['Push', 'SMS', 'InApp'],
        priority: 'Medium',
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
    }
]);

print('Notification Service database initialized successfully');


db = db.getSiblingDB('wol_analytics');

db.createCollection('trip_analytics', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['tripId', 'bookingId', 'createdAt'],
            properties: {
                tripId: { bsonType: 'string' },
                bookingId: { bsonType: 'string' },
                customerId: { bsonType: 'string' },
                driverId: { bsonType: 'string' },
                vehicleId: { bsonType: 'string' },
                vehicleTypeId: { bsonType: 'string' },
                originCity: { bsonType: 'string' },
                destinationCity: { bsonType: 'string' },
                distanceKm: { bsonType: 'double' },
                durationMinutes: { bsonType: 'int' },
                baseFare: { bsonType: 'double' },
                discountAmount: { bsonType: 'double' },
                waitingCharges: { bsonType: 'double' },
                totalFare: { bsonType: 'double' },
                finalAmount: { bsonType: 'double' },
                paymentMethod: { bsonType: 'string' },
                bookingType: { enum: ['OneWay', 'Backload', 'Shared'] },
                isBackload: { bsonType: 'bool' },
                isSharedLoad: { bsonType: 'bool' },
                status: { bsonType: 'string' },
                createdAt: { bsonType: 'date' },
                completedAt: { bsonType: 'date' },
                cancelledAt: { bsonType: 'date' },
                cancellationReason: { bsonType: 'string' }
            }
        }
    }
});

db.trip_analytics.createIndex({ tripId: 1 }, { unique: true });
db.trip_analytics.createIndex({ bookingId: 1 });
db.trip_analytics.createIndex({ customerId: 1, createdAt: -1 });
db.trip_analytics.createIndex({ driverId: 1, createdAt: -1 });
db.trip_analytics.createIndex({ vehicleId: 1, createdAt: -1 });
db.trip_analytics.createIndex({ originCity: 1, destinationCity: 1 });
db.trip_analytics.createIndex({ createdAt: -1 });
db.trip_analytics.createIndex({ status: 1 });
db.trip_analytics.createIndex({ bookingType: 1 });

db.createCollection('user_behavior', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['userId', 'eventType', 'timestamp'],
            properties: {
                userId: { bsonType: 'string' },
                eventType: { bsonType: 'string' },
                eventData: { bsonType: 'object' },
                platform: { enum: ['iOS', 'Android', 'Web'] },
                appVersion: { bsonType: 'string' },
                deviceInfo: { bsonType: 'object' },
                sessionId: { bsonType: 'string' },
                timestamp: { bsonType: 'date' }
            }
        }
    }
});

db.user_behavior.createIndex({ userId: 1, timestamp: -1 });
db.user_behavior.createIndex({ eventType: 1, timestamp: -1 });
db.user_behavior.createIndex({ sessionId: 1 });
db.user_behavior.createIndex({ timestamp: -1 });
db.user_behavior.createIndex({ platform: 1 });

db.createCollection('revenue_analytics', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['period', 'periodStart', 'periodEnd'],
            properties: {
                period: { enum: ['Daily', 'Weekly', 'Monthly', 'Yearly'] },
                periodStart: { bsonType: 'date' },
                periodEnd: { bsonType: 'date' },
                totalBookings: { bsonType: 'int' },
                completedBookings: { bsonType: 'int' },
                cancelledBookings: { bsonType: 'int' },
                totalRevenue: { bsonType: 'double' },
                platformCommission: { bsonType: 'double' },
                driverEarnings: { bsonType: 'double' },
                averageBookingValue: { bsonType: 'double' },
                revenueByCity: { bsonType: 'object' },
                revenueByVehicleType: { bsonType: 'object' },
                revenueByBookingType: { bsonType: 'object' },
                createdAt: { bsonType: 'date' }
            }
        }
    }
});

db.revenue_analytics.createIndex({ period: 1, periodStart: 1 }, { unique: true });
db.revenue_analytics.createIndex({ periodStart: -1 });

db.createCollection('driver_performance', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['driverId', 'period', 'periodStart', 'periodEnd'],
            properties: {
                driverId: { bsonType: 'string' },
                period: { enum: ['Daily', 'Weekly', 'Monthly', 'Yearly'] },
                periodStart: { bsonType: 'date' },
                periodEnd: { bsonType: 'date' },
                totalTrips: { bsonType: 'int' },
                completedTrips: { bsonType: 'int' },
                cancelledTrips: { bsonType: 'int' },
                totalEarnings: { bsonType: 'double' },
                totalDistanceKm: { bsonType: 'double' },
                totalDurationMinutes: { bsonType: 'int' },
                averageRating: { bsonType: 'double' },
                onTimeDeliveryRate: { bsonType: 'double' },
                acceptanceRate: { bsonType: 'double' },
                cancellationRate: { bsonType: 'double' },
                createdAt: { bsonType: 'date' }
            }
        }
    }
});

db.driver_performance.createIndex({ driverId: 1, period: 1, periodStart: 1 }, { unique: true });
db.driver_performance.createIndex({ driverId: 1, periodStart: -1 });
db.driver_performance.createIndex({ averageRating: -1 });
db.driver_performance.createIndex({ totalEarnings: -1 });

db.createCollection('route_analytics', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['originCity', 'destinationCity', 'period', 'periodStart', 'periodEnd'],
            properties: {
                originCity: { bsonType: 'string' },
                destinationCity: { bsonType: 'string' },
                period: { enum: ['Daily', 'Weekly', 'Monthly', 'Yearly'] },
                periodStart: { bsonType: 'date' },
                periodEnd: { bsonType: 'date' },
                totalTrips: { bsonType: 'int' },
                averageDistance: { bsonType: 'double' },
                averageDuration: { bsonType: 'int' },
                averageFare: { bsonType: 'double' },
                peakHours: { bsonType: 'array' },
                popularVehicleTypes: { bsonType: 'array' },
                backloadOpportunities: { bsonType: 'int' },
                createdAt: { bsonType: 'date' }
            }
        }
    }
});

db.route_analytics.createIndex({ originCity: 1, destinationCity: 1, period: 1, periodStart: 1 }, { unique: true });
db.route_analytics.createIndex({ originCity: 1, destinationCity: 1, periodStart: -1 });
db.route_analytics.createIndex({ totalTrips: -1 });

db.createCollection('system_metrics', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['metricType', 'timestamp'],
            properties: {
                metricType: { bsonType: 'string' },
                metricName: { bsonType: 'string' },
                value: { bsonType: 'double' },
                unit: { bsonType: 'string' },
                tags: { bsonType: 'object' },
                timestamp: { bsonType: 'date' }
            }
        }
    }
});

db.system_metrics.createIndex({ metricType: 1, timestamp: -1 });
db.system_metrics.createIndex({ metricName: 1, timestamp: -1 });
db.system_metrics.createIndex({ timestamp: -1 });

print('Analytics Service database initialized successfully');


db = db.getSiblingDB('wol_audit');

db.createCollection('audit_logs', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['entityType', 'entityId', 'action', 'userId', 'timestamp'],
            properties: {
                entityType: { bsonType: 'string' },
                entityId: { bsonType: 'string' },
                action: { enum: ['Create', 'Update', 'Delete', 'View', 'Export', 'Login', 'Logout'] },
                userId: { bsonType: 'string' },
                userName: { bsonType: 'string' },
                userRole: { bsonType: 'string' },
                changes: { bsonType: 'object' },
                oldValues: { bsonType: 'object' },
                newValues: { bsonType: 'object' },
                ipAddress: { bsonType: 'string' },
                userAgent: { bsonType: 'string' },
                timestamp: { bsonType: 'date' }
            }
        }
    }
});

db.audit_logs.createIndex({ entityType: 1, entityId: 1, timestamp: -1 });
db.audit_logs.createIndex({ userId: 1, timestamp: -1 });
db.audit_logs.createIndex({ action: 1, timestamp: -1 });
db.audit_logs.createIndex({ timestamp: -1 });

db.createCollection('api_request_logs', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['method', 'path', 'statusCode', 'timestamp'],
            properties: {
                requestId: { bsonType: 'string' },
                method: { bsonType: 'string' },
                path: { bsonType: 'string' },
                queryParams: { bsonType: 'object' },
                requestBody: { bsonType: 'object' },
                responseBody: { bsonType: 'object' },
                statusCode: { bsonType: 'int' },
                durationMs: { bsonType: 'int' },
                userId: { bsonType: 'string' },
                ipAddress: { bsonType: 'string' },
                userAgent: { bsonType: 'string' },
                timestamp: { bsonType: 'date' }
            }
        }
    }
});

db.api_request_logs.createIndex({ requestId: 1 }, { unique: true });
db.api_request_logs.createIndex({ userId: 1, timestamp: -1 });
db.api_request_logs.createIndex({ path: 1, timestamp: -1 });
db.api_request_logs.createIndex({ statusCode: 1, timestamp: -1 });
db.api_request_logs.createIndex({ timestamp: -1 });

db.createCollection('error_logs', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['errorType', 'message', 'timestamp'],
            properties: {
                errorId: { bsonType: 'string' },
                errorType: { bsonType: 'string' },
                errorCode: { bsonType: 'string' },
                message: { bsonType: 'string' },
                stackTrace: { bsonType: 'string' },
                serviceName: { bsonType: 'string' },
                requestId: { bsonType: 'string' },
                userId: { bsonType: 'string' },
                context: { bsonType: 'object' },
                severity: { enum: ['Low', 'Medium', 'High', 'Critical'] },
                isResolved: { bsonType: 'bool' },
                resolvedAt: { bsonType: 'date' },
                timestamp: { bsonType: 'date' }
            }
        }
    }
});

db.error_logs.createIndex({ errorId: 1 }, { unique: true });
db.error_logs.createIndex({ errorType: 1, timestamp: -1 });
db.error_logs.createIndex({ serviceName: 1, timestamp: -1 });
db.error_logs.createIndex({ severity: 1, timestamp: -1 });
db.error_logs.createIndex({ isResolved: 1, timestamp: -1 });
db.error_logs.createIndex({ timestamp: -1 });

print('Audit Log database initialized successfully');


db = db.getSiblingDB('wol_location_history');

db.createCollection('location_history', {
    timeseries: {
        timeField: 'timestamp',
        metaField: 'metadata',
        granularity: 'seconds'
    }
});

db.location_history.createIndex({ 'metadata.tripId': 1, timestamp: -1 });
db.location_history.createIndex({ 'metadata.driverId': 1, timestamp: -1 });
db.location_history.createIndex({ 'metadata.vehicleId': 1, timestamp: -1 });
db.location_history.createIndex({ timestamp: -1 });


print('Location History database initialized successfully');


db = db.getSiblingDB('wol_reports');

db.createCollection('scheduled_reports', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['reportName', 'reportType', 'schedule'],
            properties: {
                reportName: { bsonType: 'string' },
                reportType: { bsonType: 'string' },
                description: { bsonType: 'string' },
                schedule: { enum: ['Daily', 'Weekly', 'Monthly', 'Quarterly', 'Yearly'] },
                recipients: { bsonType: 'array' },
                filters: { bsonType: 'object' },
                isActive: { bsonType: 'bool' },
                lastRunAt: { bsonType: 'date' },
                nextRunAt: { bsonType: 'date' },
                createdAt: { bsonType: 'date' },
                updatedAt: { bsonType: 'date' }
            }
        }
    }
});

db.scheduled_reports.createIndex({ reportType: 1 });
db.scheduled_reports.createIndex({ schedule: 1 });
db.scheduled_reports.createIndex({ isActive: 1, nextRunAt: 1 });

db.createCollection('generated_reports', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['reportName', 'reportType', 'generatedAt'],
            properties: {
                reportName: { bsonType: 'string' },
                reportType: { bsonType: 'string' },
                periodStart: { bsonType: 'date' },
                periodEnd: { bsonType: 'date' },
                data: { bsonType: 'object' },
                fileUrl: { bsonType: 'string' },
                format: { enum: ['PDF', 'Excel', 'CSV', 'JSON'] },
                generatedBy: { bsonType: 'string' },
                generatedAt: { bsonType: 'date' },
                expiresAt: { bsonType: 'date' }
            }
        }
    }
});

db.generated_reports.createIndex({ reportType: 1, generatedAt: -1 });
db.generated_reports.createIndex({ generatedBy: 1, generatedAt: -1 });
db.generated_reports.createIndex({ generatedAt: -1 });
db.generated_reports.createIndex({ expiresAt: 1 }, { expireAfterSeconds: 0 });

print('Reporting database initialized successfully');


db = db.getSiblingDB('wol_cache');

db.createCollection('sessions', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['sessionId', 'userId', 'createdAt', 'expiresAt'],
            properties: {
                sessionId: { bsonType: 'string' },
                userId: { bsonType: 'string' },
                data: { bsonType: 'object' },
                createdAt: { bsonType: 'date' },
                expiresAt: { bsonType: 'date' }
            }
        }
    }
});

db.sessions.createIndex({ sessionId: 1 }, { unique: true });
db.sessions.createIndex({ userId: 1 });
db.sessions.createIndex({ expiresAt: 1 }, { expireAfterSeconds: 0 });

db.createCollection('rate_limits', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['key', 'count', 'windowStart', 'expiresAt'],
            properties: {
                key: { bsonType: 'string' },
                count: { bsonType: 'int' },
                windowStart: { bsonType: 'date' },
                expiresAt: { bsonType: 'date' }
            }
        }
    }
});

db.rate_limits.createIndex({ key: 1 }, { unique: true });
db.rate_limits.createIndex({ expiresAt: 1 }, { expireAfterSeconds: 0 });

print('Cache database initialized successfully');


print('');
print('='.repeat(80));
print('MongoDB initialization completed successfully!');
print('='.repeat(80));
print('');
print('Created databases:');
print('  - wol_notifications (Notification Service)');
print('  - wol_analytics (Analytics Service)');
print('  - wol_audit (Audit Logs)');
print('  - wol_location_history (Location Tracking - Time Series)');
print('  - wol_reports (Reporting Service)');
print('  - wol_cache (Cache & Sessions)');
print('');
print('All collections, indexes, and validation rules have been created.');
print('Sample notification templates have been inserted.');
print('');
print('Next steps:');
print('  1. Configure authentication for production');
print('  2. Set up backup and restore procedures');
print('  3. Configure monitoring and alerting');
print('  4. Review and adjust TTL indexes based on retention policies');
print('='.repeat(80));
