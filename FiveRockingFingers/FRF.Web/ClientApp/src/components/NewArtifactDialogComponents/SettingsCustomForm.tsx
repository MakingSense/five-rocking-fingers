﻿import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField, IconButton } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { PROVIDERS } from '../../Constants';
import ArtifactType from '../../interfaces/ArtifactType';
import ArtifactService from '../../services/ArtifactService';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';


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
        }
    }),
);

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function }) => {

    const classes = useStyles();

    const { register, handleSubmit, errors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    const [settingsList, setSettingsList] = React.useState<object>({});

    const handleConfirm = async (data: { name: string, provider: string, artifactType: string }) => {
        const artifactToCreate = {
            name: data.name.trim(),
            provider: data.provider,
            artifactTypeId: parseInt(data.artifactType, 10),
            projectId: projectId,
            settings: { empty: "" }
        };

        console.log(artifactToCreate);

        try {
            const response = await ArtifactService.save(artifactToCreate);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El artefacto ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateList();
            } else {
                setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
                setOpenSnackbar(true);
            }
        }
        catch (error) {
            setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
            setOpenSnackbar(true);
        }
        closeNewArtifactDialog()
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Bienvenido al asistente para la creación de un nuevo artefacto</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese el tipo y el nombre de su nuevo artefacto
                </Typography>
                <form className={classes.container}>
                    <TextField
                        inputRef={register({ required: true, validate: { isValid: value => value.trim() != "" } })}
                        error={errors.name ? true : false}
                        id="name"
                        name="name"
                        label="Nombre de la setting"
                        helperText="Requerido*"
                        variant="outlined"
                        className={classes.inputF}
                    />

                    <TextField
                        inputRef={register({ required: true, validate: { isValid: value => value.trim() != "" } })}
                        error={errors.name ? true : false}
                        id="name"
                        name="name"
                        label="Valor de la setting"
                        helperText="Requerido*"
                        variant="outlined"
                        className={classes.inputF}
                    />
                    <IconButton aria-label="add" disabled color="primary">
                        <AddCircleIcon />
                    </IconButton>
                    <IconButton aria-label="delete" disabled color="secondary">
                        <DeleteIcon />
                    </IconButton>
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Continuar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsCustomForm;