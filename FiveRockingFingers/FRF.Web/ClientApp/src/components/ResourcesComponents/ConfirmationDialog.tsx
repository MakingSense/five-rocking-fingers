import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import * as React from 'react';
import Resource from '../../interfaces/Resource';
import ResourceService from '../../services/ResourceService';
import { handleErrorMessage } from '../../commons/Helpers';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, resourceToDelete: Resource, openSnackbar: Function, updateList: Function }) => {

    const { open, setOpen, resourceToDelete, openSnackbar, updateList } = props;

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            if (!resourceToDelete.id) {
                return;
            }
            const response = await ResourceService.delete(resourceToDelete.id);
            if (response.status == 204) {
                openSnackbar({ message: "El recurso ha sido borrado con éxito", severity: "success" });
                updateList();
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al borrar el recurso",
                    openSnackbar,
                    undefined
                );
            }
        }
        catch {
            openSnackbar({ message: "Hubo un error al borrar el recurso", severity: "error" });
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
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea eliminar el recurso?"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        El recurso <i>"{resourceToDelete.roleName}</i>" será eliminado
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