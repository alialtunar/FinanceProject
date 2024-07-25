import { createSlice } from "@reduxjs/toolkit";

import { createAsyncThunk } from "@reduxjs/toolkit";


const initialState = {
    user:{},
    error: null,
    loading:false,
    status: 'idle',
}


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
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }

    return response.json();
});

export const {reducer,actions} = createSlice({
    name: 'user',
    initialState,
    reducers:{},
    extraReducers: (builder) => {
        builder
        .addCase(fetchUser.pending, (state) => {
            state.loading = true;
            state.status = 'loading';
        })
        .addCase(fetchUser.fulfilled, (state, action) => {
            state.loading = false;
            state.status = 'succeeded';
            state.user = action.payload;
        })
        .addCase(fetchUser.rejected, (state, action) => {
            state.loading = false;
            state.status = 'failed';
            state.error = action.error.message;
        });
    },

})