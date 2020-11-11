import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';

const ArtifactsTableRow = (props: { artifact: Artifact, setOpenSnackbar: Function, setSnackbarSettings: Function }) => {

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
            <ConfirmationDialog
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                artifactToDelete={props.artifact}
                setOpenSnackbar={props.setOpenSnackbar}
                setSnackbarSettings={props.setSnackbarSettings}
            />
         </React.Fragment>
    );
};

export default ArtifactsTableRow;