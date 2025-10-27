import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type Language = 'en' | 'ar' | 'ur';

interface UiState {
  language: Language;
  isRTL: boolean;
}

const initialState: UiState = {
  language: 'en',
  isRTL: false,
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setLanguage: (state, action: PayloadAction<Language>) => {
      state.language = action.payload;
      state.isRTL = action.payload === 'ar' || action.payload === 'ur';
    },
  },
});

export const { setLanguage } = uiSlice.actions;
export default uiSlice.reducer;
