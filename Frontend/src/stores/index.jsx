import { configureStore } from '@reduxjs/toolkit'
import {reducer as userReducer} from './user-store/index'

export function createStore(preloadedState = {}){
     const store = configureStore({
        reducer:{
            user:userReducer
        },
        preloadedState
    });

    return store;
}

export const store = createStore();