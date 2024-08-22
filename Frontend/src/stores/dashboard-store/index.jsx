import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';

const API_URL = 'http://localhost:5233/api';

// Async thunk'lar
export const fetchTransactionVolumeLast24Hours = createAsyncThunk(
  'dashboard/fetchTransactionVolumeLast24Hours',
  async (userId) => {
    const response = await fetch(`${API_URL}/TransactionHistory/totalamountlast24hours/${userId}`);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();
    if (!data.isSuccess) {
      throw new Error(data.errorMessages.join(', '));
    }
    return data.result;
  }
);

export const fetchAccountDetails = createAsyncThunk(
  'dashboard/fetchAccountDetails',
  async (userId) => {
    const response = await fetch(`${API_URL}/account/details/${userId}`);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();
    if (!data.isSuccess) {
      throw new Error(data.errorMessages.join(', '));
    }
    return data.result;
  }
);

export const fetchLast5Transaction = createAsyncThunk(
  'dashboard/fetchLast5Transaction',
  async (userId) => {
    const response = await fetch(`${API_URL}/TransactionHistory/last5/${userId}`);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();
    if (!data.isSuccess) {
      throw new Error(data.errorMessages.join(', '));
    }
    return data.result;
  }
);

// Slice
export const dashboardSlice = createSlice({
  name: 'dashboard',
  initialState: {
    transactionVolume: null,
    accountDetails: null,
    last5Transactions: [],
    status: 'idle',
    error: null
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchTransactionVolumeLast24Hours.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchTransactionVolumeLast24Hours.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.transactionVolume = action.payload;
        state.error = null;
      })
      .addCase(fetchTransactionVolumeLast24Hours.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message;
      })
      .addCase(fetchAccountDetails.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchAccountDetails.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.accountDetails = action.payload;
        state.error = null;
      })
      .addCase(fetchAccountDetails.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message;
      })
      .addCase(fetchLast5Transaction.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchLast5Transaction.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.last5Transactions = action.payload;
        state.error = null;
      })
      .addCase(fetchLast5Transaction.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message;
      });
  }
});

export const { reducer, actions } = dashboardSlice;
