import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import * as Localization from 'expo-localization';

import enCommon from '../../../shared-i18n/locales/en/common.json';
import arCommon from '../../../shared-i18n/locales/ar/common.json';
import urCommon from '../../../shared-i18n/locales/ur/common.json';

const deviceLanguage = Localization.locale.split('-')[0];

i18n
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
    lng: ['en', 'ar', 'ur'].includes(deviceLanguage) ? deviceLanguage : 'en',
    fallbackLng: 'en',
    defaultNS: 'common',
    ns: ['common'],
    supportedLngs: ['en', 'ar', 'ur'],
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;
