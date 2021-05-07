import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import ProjectResource from '../../interfaces/ProjectResource';
import ProjectResourceService from '../../services/ProjectResourceService';
import { handleErrorMessage } from '../../commons/Helpers';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, projectResourceToDelete: ProjectResource, openSnackbar: Function, updateList: Function }) => {
    const { setOpen, openSnackbar, projectResourceToDelete, open, updateList } = props;

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            const response = await ProjectResourceService.delete(projectResourceToDelete.id!)
            if (response.status === 204) {
                updateList();
                openSnackbar({ message: "El recurso a si desvinculado del proyecto con éxito", severity: "success" });
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al desvincular el recurso",
                    openSnackbar,
                    undefined
                );
            }
        }
        catch {
            openSnackbar({ message: "Hubo un error al desvincular el recurso", severity: "error" });
        }
        handleClose();
    };

    const handleCancel = () => {
        handleClose();
    };

    return (
        <>
            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea desvincular el recurso?"}</DialogTitle>
                <DialogContent dividers >
                    <DialogContentText id="alert-dialog-description">
                        El recurso <i>"{projectResourceToDelete.resource.roleName}</i>" será desvinculado del proyecto.
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
        </>
    );
}

export default ConfirmationDialog;