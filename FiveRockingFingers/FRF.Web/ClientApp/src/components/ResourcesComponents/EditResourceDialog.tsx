import { Button, Dialog, DialogActions, DialogTitle } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme, ThemeProvider } from '@material-ui/core/styles';
import * as React from 'react';
import Resource from '../../interfaces/Resource';
import ResourceForm from './ResourceForm';
import { handleErrorMessage } from '../../commons/Helpers';
import ResourceService from '../../services/ResourceService';
import { FormProvider, useForm, useFormContext } from 'react-hook-form';

const theme = createMuiTheme({
    overrides: {
        MuiDialogContent: {
            root: {
                padding: "0px 5px 0px 5px"
            },
        },
    },
});

const EditResourceDialog = (props: {
    resourceToEdit: Resource,
    showEditResourceDialog: boolean,
    closeEditResourceDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    updateResources: Function,
    manageOpenSnackbar: Function
}) => {

    const { resourceToEdit, showEditResourceDialog, closeEditResourceDialog, setOpenSnackbar, setSnackbarSettings, updateResources, manageOpenSnackbar } = props;
    const methods = useForm();
    const { handleSubmit } = methods;

    const [resource, setResource] = React.useState<Resource>(resourceToEdit);

    const closeDialog = () => {
        closeEditResourceDialog();
    }

    const handleConfirm = async () => {
        try {
            if (!resource.id) {
                return;
            }
            const response = await ResourceService.update(resource.id, resource);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El recurso ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateResources();
            } else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al crear el recurso",
                    setSnackbarSettings,
                    setOpenSnackbar
                );
            }
        }
        catch (error) {
            setSnackbarSettings({ message: "Hubo un error al crear el recurso", severity: "error" });
            setOpenSnackbar(true);
        }
        closeDialog();
    }

    const handleCancel = () => {
        closeDialog();
    }

    return (
        <Dialog open={showEditResourceDialog}>
            <FormProvider {...methods}>
                <ThemeProvider theme={theme}>
                    <DialogTitle >Formulario de creación de recursos</DialogTitle>
                    <ResourceForm
                        resource={resource}
                        setResource={setResource}
                    />
                    <DialogActions>
                        <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Listo</Button>
                        <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                    </DialogActions>
                </ThemeProvider>
            </FormProvider>
        </Dialog>
    );
}

export default EditResourceDialog;