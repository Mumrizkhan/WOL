import React from 'react';
import { useTranslation } from 'react-i18next';
import { Languages } from 'lucide-react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { setLanguage } from '../store/slices/uiSlice';

const LanguageSwitcher: React.FC = () => {
  const { i18n, t } = useTranslation();
  const dispatch = useAppDispatch();
  const currentLanguage = useAppSelector((state) => state.ui.language);
  const [isOpen, setIsOpen] = React.useState(false);

  const languages = [
    { code: 'en', name: t('languages.english') },
    { code: 'ar', name: t('languages.arabic') },
    { code: 'ur', name: t('languages.urdu') },
  ];

  const handleLanguageChange = (langCode: 'en' | 'ar' | 'ur') => {
    i18n.changeLanguage(langCode);
    dispatch(setLanguage(langCode));
    
    const isRTL = langCode === 'ar' || langCode === 'ur';
    document.documentElement.dir = isRTL ? 'rtl' : 'ltr';
    document.documentElement.lang = langCode;
    
    setIsOpen(false);
  };

  return (
    <div className="relative">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center gap-2 px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors"
      >
        <Languages className="w-5 h-5" />
        <span className="text-sm font-medium">
          {languages.find((l) => l.code === currentLanguage)?.name}
        </span>
      </button>

      {isOpen && (
        <>
          <div
            className="fixed inset-0 z-10"
            onClick={() => setIsOpen(false)}
          />
          <div className="absolute end-0 mt-2 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-20">
            {languages.map((lang) => (
              <button
                key={lang.code}
                onClick={() => handleLanguageChange(lang.code as 'en' | 'ar' | 'ur')}
                className={`w-full text-start px-4 py-2 text-sm hover:bg-gray-100 transition-colors ${
                  currentLanguage === lang.code
                    ? 'bg-purple-50 text-purple-600 font-medium'
                    : 'text-gray-700'
                }`}
              >
                {lang.name}
              </button>
            ))}
          </div>
        </>
      )}
    </div>
  );
};

export default LanguageSwitcher;
