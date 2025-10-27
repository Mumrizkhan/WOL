import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import enCommon from '../../../shared-i18n/locales/en/common.json';
import arCommon from '../../../shared-i18n/locales/ar/common.json';
import urCommon from '../../../shared-i18n/locales/ur/common.json';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: {
        common: enCommon,
      },
      ar: {
        common: arCommon,
      },
      ur: {
        common: urCommon,
      },
    },
    fallbackLng: 'en',
    defaultNS: 'common',
    ns: ['common'],
    supportedLngs: ['en', 'ar', 'ur'],
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage'],
    },
  });

export default i18n;
