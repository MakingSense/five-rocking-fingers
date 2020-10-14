import React,{useContext,createContext } from "react";

export const UserContext = React.createContext<any>(null);

export function useUserContext() {
    return useContext(UserContext);
  }