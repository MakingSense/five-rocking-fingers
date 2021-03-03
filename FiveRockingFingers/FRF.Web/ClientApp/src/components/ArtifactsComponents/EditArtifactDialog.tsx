import { Dialog } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import EditArtifact from './EditArtifact';
import EditArtifactConfirmation from './EditArtifactConfirmation';


const EditArtifactDialog = (props: {
    artifactToEdit: Artifact,
    showEditArtifactDialog: boolean,
    closeEditArtifactDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    updateArtifacts: Function,
    updateRelations: Function }) => {

    const { showEditArtifactDialog, closeEditArtifactDialog } = props;

    const [isArtifactEdited, setIsArtifactEdited] = React.useState<boolean>(false);
    const [artifactEdited, setArtifactEdited] = React.useState<Artifact>(props.artifactToEdit);
    const [namesOfSettingsChanged, setNamesOfSettingsChanged] = React.useState<string[]>([]);

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
                    updateArtifacts={props.updateArtifacts}
                    updateRelations={props.updateRelations}
                />
            }
        </Dialog>
    );
}

export default EditArtifactDialog;