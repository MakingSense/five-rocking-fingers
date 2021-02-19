import React,{useContext,createContext } from "react";
import UserProfile from '../../interfaces/UserProfile';

export const UserContext = React.createContext<any>(null);

export function useUserContext() {
    return useContext(UserContext);
  }