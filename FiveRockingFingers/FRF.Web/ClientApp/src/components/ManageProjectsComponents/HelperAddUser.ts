import UserProfile from '../../interfaces/UserProfile';

export function HelperAddUser(user: UserProfile, usersProfile: UserProfile[], emailField: Function, openSnackbar: Function) {
    const aux: UserProfile = {
        userId: user.userId,
        email: user.email,
        fullName: "",
        avatar: ""
    }
    var auxState = usersProfile;
    var indexItem = auxState.findIndex(x => x.userId === user.userId);

    if (indexItem === -1) {
        auxState.push(aux);
        openSnackbar({ message: "Usuario asignado correctamente!", severity: "success"});
        emailField("");
        return auxState;
    }
    else {
        openSnackbar({ message: "Usuario ya ha sido asignado al projecto!", severity: "warning" });
        emailField("");
        return null;
    }
}
