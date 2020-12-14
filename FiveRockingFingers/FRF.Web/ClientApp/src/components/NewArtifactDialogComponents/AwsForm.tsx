import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import * as React from 'react';
import Typography from '@material-ui/core/Typography';

const AwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, handlePreviousStep: Function }) => {

    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    const handlePreviousStep = () => {
        props.handlePreviousStep();
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos AWS</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    El formulario de artefactos AWS todavía no ha sido implementado
                </Typography>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={handlePreviousStep}>Atrás</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default AwsForm;