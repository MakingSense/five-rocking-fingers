import UserPublicProfile from '../../interfaces/UserPublicProfile';
import UserByProject from '../../interfaces/UserByProject';

export function HelperAddUser(user: UserPublicProfile, usersByProject: UserByProject[], emailField: Function, openSnackbar: Function) {
    const aux: UserByProject = {
        id: 0,
        userId: user.userId,
        projectId: 0,
        email: user.email
    }
    var auxState = usersByProject;
    var indexItem = auxState.findIndex(x => x.userId === user.userId);

    if (indexItem === -1) {
        auxState.push(aux);
        openSnackbar("Usuario asignado correctamente!", "success");
        emailField("");
        return auxState;
    }
    else {
        openSnackbar("Usuario ya ha sido asignado al projecto!", "warning");
        emailField("");
        return null;
    }
}
