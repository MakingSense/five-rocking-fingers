import { Dialog } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import EditArtifact from './EditArtifact';
import EditArtifactConfirmation from './EditArtifactConfirmation';
import Setting from '../../interfaces/Setting';


const EditArtifactDialog = (props: {
    artifactToEdit: Artifact,
    showEditArtifactDialog: boolean,
    closeEditArtifactDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    projectId: number,
    artifactsRelations: ArtifactRelation[],
    updateArtifacts: Function,
    updateRelations: Function }) => {

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

    React.useEffect(() => {
        getArtifactsRelations();
    }, [props.artifactToEdit]);

    return (
        <Dialog open={showEditArtifactDialog}>
            {!isArtifactEdited ?
                <EditArtifact
                    artifactToEdit={props.artifactToEdit}
                    closeEditArtifactDialog={closeEditArtifactDialog}
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
                    updateArtifacts={props.updateArtifacts}
                    updateRelations={props.updateRelations}
                />
            }
        </Dialog>
    );
}

export default EditArtifactDialog;