﻿import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Snackbar } from '@material-ui/core';
import * as React from 'react';
import Artifact from '../../interfaces/Artifact';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, deleteArtifact: Function, artifactToDelete: Artifact, setOpenSnackbar: Function, setSnackbarSettings: Function }) => {

    const handleClose = () => {
        props.setOpen(false);
    };

    const handleConfirm = () => {
        try {
            props.deleteArtifact(props.artifactToDelete.id);
            props.setSnackbarSettings({ message: "El artefacto ha sido borrado con éxito", severity: "success" });
            props.setOpenSnackbar(true);
        }
        catch {
            props.setSnackbarSettings({ message: "Hubo un error al borrar el artefacto", severity: "error" });
            props.setOpenSnackbar(true);
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