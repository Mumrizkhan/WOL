import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type Language = 'en' | 'ar' | 'ur';

interface UiState {
  language: Language;
  isRTL: boolean;
  theme: 'light' | 'dark';
}

const initialState: UiState = {
  language: 'en',
  isRTL: false,
  theme: 'light',
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setLanguage: (state, action: PayloadAction<Language>) => {
      state.language = action.payload;
      state.isRTL = action.payload === 'ar' || action.payload === 'ur';
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.theme = action.payload;
    },
  },
});

export const { setLanguage, setTheme } = uiSlice.actions;
export default uiSlice.reducer;
