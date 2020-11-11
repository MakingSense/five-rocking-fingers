﻿import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import axios from 'axios';
import * as React from 'react';
import Project from '../../interfaces/Project';

export default function ConfirmationDialog(props: { keepMounted: boolean, open: boolean, project: Project | null, onClose: Function, resetView: Function, openSnackbar: Function, updateProjects: Function }) {
    const { onClose, project, open, updateProjects } = props;

    const handleCancel = () => {
        onClose();
    };

    const handleOk = async () => {
        if (project) {
            props.resetView(project.id);
            try {
                const response = await axios.delete("https://localhost:44346/api/Projects/Delete/" + project.id.toString());
                if (response.status === 204) {
                    props.openSnackbar({ message: "Se eliminó el proyecto con éxito", severity: "success" });
                    updateProjects();
                } else {
                    props.openSnackbar({ message: "Ocurrió un error al eliminar el proyecto", severity: "error" });
                }
            }
            catch {
                props.openSnackbar({ message: "Ocurrió un error al eliminar el proyecto", severity: "error" });
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