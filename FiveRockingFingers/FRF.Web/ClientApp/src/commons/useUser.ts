import { useContext } from "react";
import UserProfile from "../interfaces/UserProfile";
import UserService from "../services/UserService";
import { UserContext } from "./UserContext";
import jwt_decode from "jwt-decode";

interface IUserJwtPayload {
	sub: string,
	email: string,
	family_name: string,
	name: string
}

export const useUser = () => {
	const { userProfile, setUserProfile } = useContext(UserContext);

	const findUser = (): string | null => {
		const userLocal = localStorage.getItem("user_token");
		const userSession = sessionStorage.getItem("user_token");
		return userLocal ?? userSession;
	};

	const parseUser = (userToken: string | null): UserProfile | null => {
		if (userToken === null) return null;
		try {
			const currentUser = jwt_decode<IUserJwtPayload | undefined>(userToken);
			return currentUser ?
			{
				userId: currentUser.sub,
				fullname: `${currentUser.name} ${currentUser.family_name}`,
				email: currentUser.email,
				// need to add avatar in claim
				avatar: null,
			} : null;
		} catch {
			return null;
		}
	};

	const getUser = (): UserProfile | null => {
		const userToken = findUser();
		return parseUser(userToken);
	};

	const storeUser = (userToken: string, remmember: boolean) => {
		remmember ? localStorage.setItem("user_token", userToken) : sessionStorage.setItem("user_token", userToken);
		setUserProfile(parseUser(userToken));
	};

	const cleanUserStorage = () => {
		sessionStorage.removeItem("user_token");
		localStorage.removeItem("user_token");
	};

	const logout = () => {
		cleanUserStorage();
		UserService.logout();
	};

	return {
		isLogged: Boolean(userProfile),
		user: userProfile,
		logout: logout,
		storeUser: storeUser,
		getUser: getUser,
		cleanUserStorage: cleanUserStorage
	};
};

