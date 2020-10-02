import React,{useContext,createContext } from "react";

export const AppContext = React.createContext<any>(false);

export function useAppContext() {
    return useContext(AppContext);
  }