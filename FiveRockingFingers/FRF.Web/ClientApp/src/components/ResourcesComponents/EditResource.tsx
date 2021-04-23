import * as React from 'react';
import { Button, Dialog, DialogActions, DialogTitle } from '@material-ui/core';
import { useForm } from 'react-hook-form';

const NewResourceDialog = (props: { open: boolean, handleClose: Function, openSnackbar: Function, updateList: Function }) => {

    const { handleSubmit } = useForm();

    const handleConfirm = () => {
        props.openSnackbar({ message: "Hubo un error al modificar el recurso", severity: "error" });
        props.handleClose();
    }

    const handleCancel = () => {
        props.handleClose();
    }

    return (
        <Dialog
            disableBackdropClick
            disableEscapeKeyDown
            open={props.open}
        >
            <DialogTitle>Modificar recurso</DialogTitle>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Listo</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    )
}

export default NewResourceDialog;