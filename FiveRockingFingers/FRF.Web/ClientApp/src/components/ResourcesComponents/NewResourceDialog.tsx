import * as React from 'react';
import { Button, Dialog, DialogActions, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { useForm, Controller } from 'react-hook-form';

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

const NewResourceDialog = (props: { open: boolean, handleClose: Function, openSnackbar: Function, updateList: Function, resources: any }) => {

    const classes = useStyles();

    const { handleSubmit, errors, control } = useForm();

    const handleConfirm = () => {
        props.openSnackbar({ message: "Hubo un error al crear el recurso", severity: "error" });
        props.handleClose();
    }

    const handleCancel = () => {
        props.handleClose();
    }

    return (
        <Dialog
            disableBackdropClick
            disableEscapeKeyDown
            open={props.open}
        >
            <DialogTitle>Añadir un nuevo recurso</DialogTitle>
            <form className={classes.container}>
                <FormControl className={classes.formControl} error={Boolean(errors.resource)}>
                    <InputLabel htmlFor="resource-select">Tipo de recurso</InputLabel>
                    <Controller
                        as={
                            <Select
                                inputProps={{
                                    name: 'resource',
                                    id: 'resource-select'
                                }}
                            >
                                <MenuItem value="">
                                    None
                                </MenuItem>
                                {props.resources.map(r => <MenuItem key={r.Id} value={r.Id}>{r.RoleName}</MenuItem>)}
                            </Select>
                        }
                        name="resource"
                        rules={{ required: true }}
                        control={control}
                    />
                    <FormHelperText>Requerido*</FormHelperText>
                </FormControl>
            </form>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Listo</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
        )
}

export default NewResourceDialog;