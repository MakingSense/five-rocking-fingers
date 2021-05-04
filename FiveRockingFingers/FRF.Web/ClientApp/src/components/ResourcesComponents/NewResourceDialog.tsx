import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme, ThemeProvider } from '@material-ui/core/styles';
import * as React from 'react';
import ResourceForm from './ResourceForm';
import ResourceService from '../../services/ResourceService';
import { FormProvider, useForm, useFormContext } from 'react-hook-form';
import { handleErrorMessage } from '../../commons/Helpers';
import Resource from '../../interfaces/Resource';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
            display: 'flex',
            flexWrap: 'wrap',
        },
        formControl: {
            margin: theme.spacing(1),
            width: '100%'
        },
        inputF: {
            padding: 2,
            marginTop: 10
        },
        select: {
            marginBottom: 24,
            marginRight: 0,
            marginLeft: 0,
            marginTop: 12,
            "& .MuiSelect-outlined": {
                paddingBottom: 13
            }
        },
        dialog: {
            "& .MuiDialogContent": {
                padding: 0
            }
        },
        error: {
            color: 'red'
        }
    }),
);

const theme = createMuiTheme({
    overrides: {
        MuiDialogContent: {
            root: {
                padding: "0px 5px 0px 5px"
            },
        },
    },
});

const NewResourceDialog = (props: {
    showNewResourceDialog: boolean,
    closeNewResourceDialog: Function,
    updateList: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function
}) => {

    const { showNewResourceDialog, closeNewResourceDialog, updateList, setOpenSnackbar, setSnackbarSettings } = props;
    const methods = useForm();
    const { handleSubmit } = methods;

    const closeDialog = () => {
        closeNewResourceDialog();
    }

    const [resource, setResource] = React.useState<Resource>({
        roleName: "",
        description: "",
        salaryPerMonth: 0,
        workloadCapacity: 0
    });

    const handleConfirm = async () => {
        try {
            const response = await ResourceService.save(resource);
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
        <Dialog open={showNewResourceDialog}>
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

export default NewResourceDialog;