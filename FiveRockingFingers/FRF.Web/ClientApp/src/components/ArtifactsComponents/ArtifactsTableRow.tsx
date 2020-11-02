import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';

const ArtifactsTableRow = (props: { artifact: Artifact, deleteArtifact: Function }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);

    

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    return (
        <React.Fragment>
            <tr>
                <td>{props.artifact.name}</td>
                <td>{props.artifact.provider}</td>
                <td>{props.artifact.artifactType.name}</td>
                <td>
                    <Button color="danger" onClick={deleteButtonClick}>Borrar</Button>
                </td>
            </tr>
            <ConfirmationDialog open={openConfirmDialog} setOpen={setOpenConfirmDialog} artifactToDelete={props.artifact} deleteArtifact={props.deleteArtifact} />

         </React.Fragment>
    );
};

export default ArtifactsTableRow;