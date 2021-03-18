import { useContext } from "react";
import UserProfile from "../interfaces/UserProfile";
import UserService from "../services/UserService";
import { UserContext } from "./UserContext";

export const useUser = () => {
	const { userProfile, setUserProfile } = useContext(UserContext);

	const findUser = (): string | null => {
		const userLocal = localStorage.getItem("user");
		const userSession = sessionStorage.getItem("user");
		return userLocal ?? userSession;
	};

	const parseUser = (userJson: string | null): UserProfile | null => {
    if (userJson !== null) {
      try {
        const currentUser: UserProfile = JSON.parse(userJson);
        return {
          userId: currentUser["userId"],
          fullname: currentUser["fullname"],
          email: currentUser["email"],
          avatar: currentUser["avatar"],
        };
      } catch {
        return null;
      }
    } else return null;
  };

	const getUser = (): UserProfile | null => {
		const userJson = findUser();
		return parseUser(userJson);
	};

	const storageUser = (userJSON: string, remmember: boolean) => {
		remmember ? localStorage.setItem("user", userJSON) : sessionStorage.setItem("user", userJSON);
		setUserProfile(parseUser(userJSON));
	};

	const cleanUserStorage = () => {
		sessionStorage.removeItem("user");
		localStorage.removeItem("user");
	};

	const logout = () => {
		cleanUserStorage();
		UserService.logout();
	};

	return {
		isLogged: Boolean(userProfile),
		user: userProfile,
		logout: logout,
		storageUser: storageUser,
		getUser: getUser,
		cleanUserStorage: cleanUserStorage
	};
};

