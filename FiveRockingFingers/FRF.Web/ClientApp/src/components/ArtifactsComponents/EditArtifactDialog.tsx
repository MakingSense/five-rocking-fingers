import { Dialog } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import EditArtifact from './EditArtifact';
import EditArtifactConfirmation from './EditArtifactConfirmation';
import Setting from '../../interfaces/Setting';


const EditArtifactDialog = (props: { artifactToEdit: Artifact, showEditArtifactDialog: boolean, closeEditArtifactDialog: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, projectId: number, artifactsRelations: ArtifactRelation[] }) => {
    const { showEditArtifactDialog, closeEditArtifactDialog } = props;

    //Hook for save the user's settings input
    const [isArtifactEdited, setIsArtifactEdited] = React.useState<boolean>(false);
    const [artifactEdited, setArtifactEdited] = React.useState<Artifact>(props.artifactToEdit);
    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [namesOfSettingsChanged, setNamesOfSettingsChanged] = React.useState<string[]>([]);

    const getArtifactsRelations = async () => {
        try {
            const response = await ArtifactService.getRelations(props.artifactToEdit.id);

            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                props.setSnackbarSettings({ message: "Hubo un error al cargar las relaciones", severity: "error" });
                props.setOpenSnackbar(true);
            }
        }
        catch {
            props.setSnackbarSettings({ message: "Hubo un error al cargar las relaciones", severity: "error" });
            props.setOpenSnackbar(true);
        }
    }

    const createSettingsListFromArtifact = () => {
        let settingsListFromArtifact: Setting[] = [];
        if (props.artifactToEdit.id === 0) {
            return settingsListFromArtifact;
        }
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            let settingFromArtifact: Setting = Object.assign({ name: key, value: value });
            settingsListFromArtifact.push(settingFromArtifact);
        });
        return settingsListFromArtifact;
    }

    const createSettingsMapFromArtifact = () => {
        let settingsMapFromArtifact: { [key: string]: number[] } = {};
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            settingsMapFromArtifact[key] = [index];
        });
        return settingsMapFromArtifact;
    }

    React.useEffect(() => {
        getArtifactsRelations();
    }, [props.artifactToEdit]);

    return (
        <Dialog open={showEditArtifactDialog}>
            {!isArtifactEdited ?
                <EditArtifact
                    artifactToEdit={props.artifactToEdit}
                    settingsList={createSettingsListFromArtifact()}
                    settingMap={createSettingsMapFromArtifact()}
                    closeEditArtifactDialog={closeEditArtifactDialog}
                    artifactsRelations={artifactsRelations}
                    setIsArtifactEdited={setIsArtifactEdited}
                    setArtifactEdited={setArtifactEdited}
                    setNamesOfSettingsChanged={setNamesOfSettingsChanged}
                /> :
                <EditArtifactConfirmation
                    artifactToEdit={artifactEdited}
                    namesOfSettingsChanged={namesOfSettingsChanged}
                    closeEditArtifactDialog={closeEditArtifactDialog}
                    setOpenSnackbar={props.setOpenSnackbar}
                    setSnackbarSettings={props.setSnackbarSettings}
                    artifactsRelations={artifactsRelations}
                />
            }
        </Dialog>
    );
}

export default EditArtifactDialog;