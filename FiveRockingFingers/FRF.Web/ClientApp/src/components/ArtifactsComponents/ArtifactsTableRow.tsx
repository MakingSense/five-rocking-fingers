import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';
import { Link } from 'react-router-dom';
import Project from '../../interfaces/Project';

const ArtifactsTableRow = (props: { artifact: Artifact, openSnackbar: Function, updateList: Function }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const [openEditArtifactRelation, setOpenEditArtifactRelation] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    
    return (
        <>
            <tr>
                <td>{props.artifact.name}</td>
                <td>{props.artifact.provider}</td>
                <td>{props.artifact.artifactType.name}</td>
                <td>
                    <Button color="danger" onClick={deleteButtonClick}>Borrar</Button>
                </td>
                <td>
                <Button color="info" tag={Link} to={`/projects/${props.artifact.projectId}/artifacts/${props.artifact.id}`}> 
                        Relaciones
                    </Button>
                </td>
            </tr>
            <ConfirmationDialog
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                artifactToDelete={props.artifact}
                openSnackbar={props.openSnackbar}
                updateList={props.updateList}
            />
        </>
    );
};

export default ArtifactsTableRow;