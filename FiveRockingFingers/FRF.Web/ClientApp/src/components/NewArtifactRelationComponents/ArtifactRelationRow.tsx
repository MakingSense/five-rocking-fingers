import * as React from 'react';
import ArtifactRelation from '../../interfaces/ArtifactRelation'
import { Button } from 'reactstrap';
import EditArtifactRelation from './EditArtifactRelation';
import DeleteArtifactsRelation from './DeleteArtifactsRelation';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import Artifact from '../../interfaces/Artifact';

const ArtifactRelationRow = (props: { artifactRelation: ArtifactRelation, openSnackbar: Function, artifacts: Artifact[] }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [openEditArtifactRelation, setOpenEditArtifactRelation] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const editRelationClick = () => {
        setOpenEditArtifactRelation(true);
    }

    const closeEditArtifactsRelation = () => {
        setOpenEditArtifactRelation(false);
    }

    return (
        <>
            <tr>
                <td>{props.artifactRelation.artifact1.name}</td>
                <td>{props.artifactRelation.artifact2.name}</td>
                <td>{props.artifactRelation.artifact1Property}</td>
                <td>{props.artifactRelation.artifact2Property}</td>
                <td>{props.artifactRelation.relationTypeId}</td>
                <td>
                    <Button color="danger" onClick={deleteButtonClick}>Borrar</Button>
                    <Button color="warning" onClick={editRelationClick}>Modificar</Button>
                </td>
            </tr>
            <EditArtifactRelation 
            open={openEditArtifactRelation} 
            closeEditArtifactsRelation={closeEditArtifactsRelation}
            artifactId={props.artifactRelation.id!} 
            artifactRelations={props.artifactRelation} 
            setOpenSnackbar={setOpenSnackbar}
            setSnackbarSettings={setSnackbarSettings}
            artifacts={props.artifacts}/>
        </>
    );
};

export default ArtifactRelationRow;