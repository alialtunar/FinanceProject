"use client"
import { Provider } from "react-redux";
import {createStore} from '../stores'



export  function StoreProvider({children,preloadedStore}){
    const store = createStore(preloadedStore);
    return (
        <Provider store={store}>
      {children}
        </Provider>
    )
}