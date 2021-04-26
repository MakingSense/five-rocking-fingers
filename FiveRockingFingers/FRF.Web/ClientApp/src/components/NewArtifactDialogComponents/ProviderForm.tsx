import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { PROVIDERS } from '../../Constants';
import Typography from '@material-ui/core/Typography';
import { useArtifact } from '../../commons/useArtifact';

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

const ProviderForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, handleNextStep: Function }) => {

    const classes = useStyles();

    const { handleSubmit, errors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    const { provider, setProvider } = useArtifact();


    const handleConfirmProvider = async (data: { provider: string }) => {
        setProvider(data.provider);
        props.handleNextStep();
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Bienvenido al asistente para la creación de un nuevo artefacto</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    Primero ingrese el provedor de su nuevo artefacto
                </Typography>
                <form className={classes.container}>
                    <FormControl className={classes.formControl} error={Boolean(errors.provider)}>
                        <InputLabel htmlFor="provider-select">Proveedor</InputLabel>
                        <Controller
                            as={

                                <Select
                                    inputProps={{
                                        name: 'provider',
                                        id: 'provider-select'
                                    }}
                                >
                                    <MenuItem value="">
                                        None
                                    </MenuItem>
                                    {PROVIDERS.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
                                </Select>
                            }
                            name="provider"
                            rules={{ required: true }}
                            control={control}
                            defaultValue={provider}
                        />
                        <FormHelperText>Requerido*</FormHelperText>
                    </FormControl>
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={handleSubmit(handleConfirmProvider)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default ProviderForm;