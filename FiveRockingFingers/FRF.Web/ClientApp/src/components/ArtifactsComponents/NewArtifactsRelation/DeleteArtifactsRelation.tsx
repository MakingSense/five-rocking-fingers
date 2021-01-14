import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Snackbar } from '@material-ui/core';
import * as React from 'react';
import ArtifactService from '../../../services/ArtifactService';
import ArtifactRelation from '../../../interfaces/ArtifactRelation';

const DeleteArtifactsRelation = (open: boolean, setOpen: Function, artifactRelationToDelete: ArtifactRelation, openSnackbar: Function, updateList: Function ) => {

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            const response = await ArtifactService.deleteRelation(artifactRelationToDelete.id)

            if (response.status == 204) {
                openSnackbar({ message: "La relacion ha sido borrado con éxito", severity: "success" });
                updateList();
            }
            else {
                openSnackbar({ message: "Hubo un error al borrar la relacion", severity: "error" });
            }
        }
        catch {
            openSnackbar({ message: "Hubo un error al borrar la relacion", severity: "error" });
        }
        handleClose();
    };

    const handleCancel = () => {
        handleClose();
    };

    return (
        <div>
            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea eliminar el artefacto?"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        ¿ Desea eliminar la relacion ?
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

export default DeleteArtifactsRelation;