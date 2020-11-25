import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Snackbar } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, artifactToDelete: Artifact, openSnackbar: Function, updateList: Function }) => {

    const handleClose = () => {
        props.setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            const response = await ArtifactService.delete(props.artifactToDelete.id)

            if (response.status == 204) {
                props.openSnackbar({ message: "El artefacto ha sido borrado con éxito", severity: "success" });
                props.updateList();
            }
            else {
                props.openSnackbar({ message: "Hubo un error al borrar el artefacto", severity: "error" });
            }
        }
        catch {
            props.openSnackbar({ message: "Hubo un error al borrar el artefacto", severity: "error" });
        }
        handleClose();
    };

    const handleCancel = () => {
        handleClose();
    };

    return (
        <div>
            <Dialog
                open={props.open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea eliminar el artefacto?"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        El proyecto <i>"{props.artifactToDelete.name}</i>" será eliminado
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleConfirm} color="primary">
                        Confirmar
                    </Button>
                    <Button onClick={handleCancel} color="secondary">
                        Cancelar
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}

export default ConfirmationDialog;