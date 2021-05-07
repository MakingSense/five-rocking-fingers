import { Button, Dialog, DialogActions, DialogTitle } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme, ThemeProvider } from '@material-ui/core/styles';
import * as React from 'react';
import Module from '../../interfaces/Module';
import ModulesForm from './ModulesForm';
import { handleErrorMessage } from '../../commons/Helpers';
import ModuleService from '../../services/ModuleService';
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

const EditModuleDialog = (props: {
    moduleToEdit: Module,
    showEditModuleDialog: boolean,
    closeEditModuleDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    updateModules: Function,
    manageOpenSnackbar: Function
}) => {

    const { moduleToEdit, showEditModuleDialog, closeEditModuleDialog, setOpenSnackbar, setSnackbarSettings, updateModules, manageOpenSnackbar } = props;
    const methods = useForm();
    const { handleSubmit } = methods;

    const [Module, setModule] = React.useState<Module>(moduleToEdit);

    const closeDialog = () => {
        closeEditModuleDialog();
    }

    const handleConfirm = async () => {
        try {
            if (!Module.id) {
                return;
            }
            const response = await ModuleService.update(Module.id, Module);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El recurso ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateModules();
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
        <Dialog open={showEditModuleDialog}>
            <FormProvider {...methods}>
                <ThemeProvider theme={theme}>
                    <DialogTitle >Formulario de creación de recursos</DialogTitle>
                    <ModulesForm
                        module={Module}
                        setModule={setModule}
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

export default EditModuleDialog;