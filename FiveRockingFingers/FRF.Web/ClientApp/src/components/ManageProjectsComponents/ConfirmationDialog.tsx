import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import axios from 'axios';
import * as React from 'react';
import Project from '../../interfaces/Project';

export default function ConfirmationDialog(props: { keepMounted: boolean, open: boolean, project: Project | null, onClose: Function, resetView: Function, openSnackbar: Function }) {
    const { onClose, project, open } = props;

    const handleCancel = () => {
        onClose();
    };

    const handleOk = async () => {
        if (project) {
            props.resetView(project.id);
            try {
                const response = await axios.delete("https://localhost:44346/api/Projects/Delete/" + project.id.toString());
                if (response.status === 204) {
                    props.openSnackbar("Se elimin\u00F3 correctamente el proyecto", "success");
                } else if (response.status === 404) {
                    props.openSnackbar("No se encontr\u00F3 el proyecto a eliminar", "info");
                }
                else {
                    props.openSnackbar("Ocurri\u00F3 un error al eliminar el proyecto", "warning");
                }
            }
            catch {
                props.openSnackbar("Ocurri\u00F3 un error al eliminar el proyecto", "warning");
            }
        }
        onClose();
    };

    return (
        <Dialog
            disableBackdropClick
            disableEscapeKeyDown
            open={open}
        >
            <DialogTitle>{"¿Está seguro que desea eliminar el proyecto?"}</DialogTitle>
            <DialogContent>
                <DialogContentText>
                    El proyecto <i>"{project ? project.name : ''}</i>" será eliminado
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleOk} color="primary">
                    Eliminar
                </Button>
                <Button onClick={handleCancel} color="secondary">
                    Cancelar
                </Button>
            </DialogActions>
        </Dialog>
    );
}