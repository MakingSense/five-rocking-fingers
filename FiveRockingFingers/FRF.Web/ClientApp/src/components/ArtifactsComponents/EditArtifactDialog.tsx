import { Dialog } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import EditArtifact from './EditArtifact';
import EditArtifactConfirmation from './EditArtifactConfirmation';
import { handleErrorMessage } from '../../commons/Helpers';
import { useArtifact } from '../../commons/useArtifact';

const EditArtifactDialog = (props: {
    artifactToEdit: Artifact,
    showEditArtifactDialog: boolean,
    closeEditArtifactDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    updateArtifacts: Function,
    manageOpenSnackbar: Function, 
    settingTypes: { [key: string]: string }, 
    setSettingTypes: Function  }) => {

    const { showEditArtifactDialog, closeEditArtifactDialog, settingTypes } = props;

    const [isArtifactEdited, setIsArtifactEdited] = React.useState<boolean>(false);
    const [artifactEdited, setArtifactEdited] = React.useState<Artifact>(props.artifactToEdit);
    const [namesOfSettingsChanged, setNamesOfSettingsChanged] = React.useState<string[]>([]);
    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [firstRender, setFirstRender] = React.useState<boolean>(true);

    const [originalSettingTypes, setOriginalSettingTypes] = React.useState<{ [key: string]: string }>({});
    const { resetStateOnClose, setSettingTypes } = useArtifact();

    const getRelations = async () => {
        try {
            const response = await ArtifactService.getRelationsAsync(props.artifactToEdit.id);
            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al cargar las relaciones entre artefactos",
                    props.manageOpenSnackbar,
                    undefined
                  );
                closeEditArtifactDialog();
            }
        }
        catch {
            props.manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones entre artefactos", severity: "error" });
            closeEditArtifactDialog();
        }
    }

    const closeDialog = () => {
        resetStateOnClose();
        closeEditArtifactDialog();
    } 

    React.useEffect(() => {
        if (firstRender) {
            setSettingTypes(props.settingTypes);
            getRelations();
            setFirstRender(false);
        }        
    }, [artifactEdited]);

    React.useEffect(() => {
        setOriginalSettingTypes(props.artifactToEdit.relationalFields);
    }, [props.artifactToEdit])

    return (
        <Dialog open={showEditArtifactDialog}>
            {!isArtifactEdited ?
                <EditArtifact
                    artifactToEdit={props.artifactToEdit}
                    closeEditArtifactDialog={closeDialog}
                    setIsArtifactEdited={setIsArtifactEdited}
                    setArtifactEdited={setArtifactEdited}
                    setNamesOfSettingsChanged={setNamesOfSettingsChanged}
                    artifactsRelations={artifactsRelations}
                /> :
                <EditArtifactConfirmation
                    artifactToEdit={artifactEdited}
                    namesOfSettingsChanged={namesOfSettingsChanged}
                    closeEditArtifactDialog={closeDialog}
                    setOpenSnackbar={props.setOpenSnackbar}
                    setSnackbarSettings={props.setSnackbarSettings}
                    updateArtifacts={props.updateArtifacts}
                    originalSettingTypes={originalSettingTypes}
                />
            }
        </Dialog>
    );
}

export default EditArtifactDialog;
