﻿import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import ArtifactType from '../../interfaces/ArtifactType';
import ArtifactTypeService from '../../services/ArtifactTypeService';
import { PROVIDERS } from '../../Constants';
import Typography from '@material-ui/core/Typography';

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

const CustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, handleNextStep: Function, handlePreviousStep: Function, setName: Function, setArtifactTypeId: Function, name: string|null, artifactTypeId: number|null }) => {

    const classes = useStyles();

    const { register, handleSubmit, errors, control, getValues } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);

    React.useEffect(() => {
        getArtifactTypes();
    }, [])

    const getArtifactTypes = async () => {
        let artifactTypes = await ArtifactTypeService.getAllByProvider(PROVIDERS[1]);
        setArtifactTypes(artifactTypes.data);
    }

    const handleConfirm = async (data: { name: string, artifactType: string }) => {
        props.setName(data.name);
        props.setArtifactTypeId(data.artifactType);
        props.handleNextStep();
    }

    const handlePreviousStep = () => {
        let data = getValues();
        props.setName(data.name);
        props.setArtifactTypeId(data.artifactType);
        props.handlePreviousStep();
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos custom</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese el tipo y el nombre de su nuevo artefacto custom
                </Typography>
                <form className={classes.container}>
                    <FormControl className={classes.formControl} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select">Tipo de artefacto</InputLabel>
                        <Controller
                            as={
                                <Select
                                    inputProps={{
                                        name: 'artifactType',
                                        id: 'type-select'
                                    }}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {artifactTypes.map(at => <MenuItem key={at.id} value={at.id}>{at.name}</MenuItem>)}
                                </Select>
                            }
                            name="artifactType"
                            rules={{ required: true }}
                            control={control}
                            defaultValue={props.artifactTypeId}
                        />
                        <FormHelperText>Requerido*</FormHelperText>
                    </FormControl>
                    <TextField
                        inputRef={register({ required: true, validate: { isValid: value => value.trim() != "" } })}
                        error={errors.name ? true : false}
                        id="name"
                        name="name"
                        label="Nombre del artefacto"
                        helperText="Requerido*"
                        variant="outlined"
                        className={classes.inputF}
                        fullWidth
                        defaultValue={props.name}
                    />
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={handlePreviousStep}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default CustomForm;