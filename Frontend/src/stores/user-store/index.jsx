import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

// Başlangıç durumu
const initialState = {
    user: {},
    error: null,
    loading: false,
    status: 'idle',
};

// Kullanıcı verilerini almak için async thunk
export const fetchUser = createAsyncThunk("user/FetchUser", async (formData) => {
    const response = await fetch('http://localhost:5233/api/User/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json(); // JSON formatında hata verisini al
        throw new Error(errorData.errorMessages.join(', ')); // Hata mesajlarını birleştir
    }

    const data = await response.json();
    
    if (!data.isSuccess) {
        throw new Error(data.errorMessages.join(', ')); // Hata mesajlarını birleştir
    }

    return data.result; // Başarılı ise `result` verisini döndür
});

// Slice oluşturma
export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
        .addCase(fetchUser.pending, (state) => {
            state.loading = true;
            state.status = 'loading';
        })
        .addCase(fetchUser.fulfilled, (state, action) => {
            state.loading = false;
            state.status = 'succeeded';
            state.error = null;
            state.user = action.payload; // `result` verisi `user` state'ine atanır
        })
        .addCase(fetchUser.rejected, (state, action) => {
            state.loading = false;
            state.status = 'failed';
            state.error = action.error.message; // Hata mesajını güncelle
        });
    },
});

export const { reducer, actions } = userSlice;
