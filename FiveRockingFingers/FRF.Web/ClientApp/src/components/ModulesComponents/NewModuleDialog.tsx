import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme, ThemeProvider } from '@material-ui/core/styles';
import * as React from 'react';
import ModulesForm from './ModulesForm';
import ModuleService from '../../services/ModuleService';
import { FormProvider, useForm, useFormContext } from 'react-hook-form';
import { handleErrorMessage } from '../../commons/Helpers';
import Module from '../../interfaces/Module';

const theme = createMuiTheme({
    overrides: {
        MuiDialogContent: {
            root: {
                padding: "0px 5px 0px 5px"
            },
        },
    },
});

const NewModuleDialog = (props: {
    showNewModuleDialog: boolean,
    closeNewModuleDialog: Function,
    updateList: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function
}) => {

    const { showNewModuleDialog, closeNewModuleDialog, updateList, setOpenSnackbar, setSnackbarSettings } = props;
    const methods = useForm();
    const { handleSubmit } = methods;

    const closeDialog = () => {
        closeNewModuleDialog();
    }

    const [Module, setModule] = React.useState<Module>({
        name: "",
        description: "",
        suggestedCost: 0
    });

    const handleConfirm = async () => {
        try {
            const response = await ModuleService.save(Module);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El recurso ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateList();
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
        <Dialog open={showNewModuleDialog}>
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

export default NewModuleDialog;