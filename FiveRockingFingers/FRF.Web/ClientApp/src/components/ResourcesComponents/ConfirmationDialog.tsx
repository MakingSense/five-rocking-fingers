import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, projectResourceToDelete: any, openSnackbar: Function, updateList: Function }) => {

    const handleClose = () => {
        props.setOpen(false);
    };

    const handleConfirm = async () => {
        props.openSnackbar({ message: "Hubo un error al borrar el recurso", severity: "error" });
        handleClose();
    };

    const handleCancel = () => {
        handleClose();
    };

    return (
        <>
            <Dialog
                open={props.open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea eliminar el artefacto?"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        El recurso <i>"{props.projectResourceToDelete.Resource.RoleName}</i>" será eliminado
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