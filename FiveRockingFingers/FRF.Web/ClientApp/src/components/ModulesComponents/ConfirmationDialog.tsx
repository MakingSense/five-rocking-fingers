import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import * as React from 'react';
import Module from '../../interfaces/Module';
import ModuleService from '../../services/ModuleService';
import { handleErrorMessage } from '../../commons/Helpers';

const ConfirmationDialog = (props: { open: boolean, setOpen: Function, moduleToDelete: Module, openSnackbar: Function, updateList: Function }) => {

    const { open, setOpen, moduleToDelete, openSnackbar, updateList } = props;

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            if (!moduleToDelete.id) {
                return;
            }
            const response = await ModuleService.delete(moduleToDelete.id);
            if (response.status == 204) {
                openSnackbar({ message: "El módulo ha sido borrado con éxito", severity: "success" });
                updateList();
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al borrar el módulo",
                    openSnackbar,
                    undefined
                );
            }
        }
        catch {
            openSnackbar({ message: "Hubo un error al borrar el módulo", severity: "error" });
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
                        El módulo <i>"{moduleToDelete.name}</i>" será eliminado
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