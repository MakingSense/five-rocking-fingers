import React from "react";
import UserProfile from '../interfaces/UserProfile';
import { useUser } from './useUser'
import Button from '@material-ui/core/Button';
import ExitToAppOutlinedIcon from '@material-ui/icons/ExitToAppOutlined';
import Navbar from "./Navbar";
import { useHistory } from "react-router-dom";

export type UserContext = {
  userProfile: UserProfile | null
  setUserProfile: (user: UserProfile | null) => void
}

export const UserContext = React.createContext<UserContext>({
  userProfile: null,
  setUserProfile: () => {}
});

export const useUserContext = () => React.useContext(UserContext);

export const UserContextProvider : React.FC<{}>  = ({ children }) =>{
  const {getUser, logout }= useUser();
  const [currentUser, setCurrentUser] = React.useState<UserProfile | null>(getUser());
  const History = useHistory();
  
  const handleLogout = () => {
    setCurrentUser(null);  
    logout();
    History.push("/login");
  }

  return <UserContext.Provider value={{ userProfile: currentUser, setUserProfile: setCurrentUser }}>
    {currentUser
      ? <Navbar userName={currentUser.fullname} logoutComponent={<Button
        variant="contained"
        color="default"
        startIcon={<ExitToAppOutlinedIcon />}
        onClick={handleLogout}>Logout</Button>} /> : <></>
    }
    {children}
  </UserContext.Provider>
} 