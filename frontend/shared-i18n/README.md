# Shared i18n Translations

This package contains shared translation files for all World of Logistics frontend applications.

## Languages Supported

- **English (en)** - Default language
- **Arabic (ar)** - RTL support
- **Urdu (ur)** - RTL support

## Structure

```
shared-i18n/
├── locales/
│   ├── en/
│   │   └── common.json
│   ├── ar/
│   │   └── common.json
│   └── ur/
│       └── common.json
└── README.md
```

## Usage

### Admin Web App (React)

Import translations in your i18n configuration:

```typescript
import enCommon from '@wol/shared-i18n/locales/en/common.json';
import arCommon from '@wol/shared-i18n/locales/ar/common.json';
import urCommon from '@wol/shared-i18n/locales/ur/common.json';

i18n.use(initReactI18next).init({
  resources: {
    en: { common: enCommon },
    ar: { common: arCommon },
    ur: { common: urCommon },
  },
  // ...
});
```

### Mobile Apps (React Native)

Configure Metro bundler to watch the shared directory:

```javascript
// metro.config.js
const path = require('path');

module.exports = {
  watchFolders: [
    path.resolve(__dirname, '../shared-i18n'),
  ],
  // ...
};
```

Then import translations:

```typescript
import enCommon from '../shared-i18n/locales/en/common.json';
import arCommon from '../shared-i18n/locales/ar/common.json';
import urCommon from '../shared-i18n/locales/ur/common.json';
```

## Translation Keys

All translation keys follow a hierarchical structure:

- `app.*` - Application name and branding
- `auth.*` - Authentication related strings
- `navigation.*` - Navigation labels
- `common.*` - Common UI elements
- `dashboard.*` - Dashboard specific strings
- `users.*` - User management
- `bookings.*` - Booking management
- `vehicles.*` - Vehicle management
- `payments.*` - Payment related strings
- `reports.*` - Reports and analytics
- `profile.*` - User profile
- `jobs.*` - Job management (driver app)
- `earnings.*` - Earnings tracking (driver app)
- `tracking.*` - Live tracking
- `settings.*` - Settings and preferences
- `stats.*` - Statistics and metrics
- `messages.*` - User feedback messages
- `languages.*` - Language names

## Adding New Translations

1. Add the key to all three language files (en, ar, ur)
2. Ensure the key follows the existing hierarchical structure
3. Test the translation in both LTR and RTL modes
4. Verify text wrapping and layout for longer translations

## RTL Support

Arabic and Urdu require right-to-left (RTL) text direction:

- **Web**: Set `document.documentElement.dir = 'rtl'`
- **Mobile**: Use `I18nManager.forceRTL(true)`

## Best Practices

1. **Keep keys consistent** across all apps
2. **Use namespaces** for better organization
3. **Avoid hardcoded strings** in components
4. **Test RTL layouts** thoroughly
5. **Use logical properties** (start/end instead of left/right)
6. **Consider text expansion** - Arabic/Urdu text can be 30% longer
7. **Use proper fonts** for Arabic/Urdu script rendering

## Maintenance

When adding new features:
1. Add translation keys to this shared package first
2. Update all three language files simultaneously
3. Test in all three languages before committing
4. Document any context-specific translations
