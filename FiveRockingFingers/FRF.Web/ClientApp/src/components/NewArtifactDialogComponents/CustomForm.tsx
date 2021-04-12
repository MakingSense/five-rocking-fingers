import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import ArtifactType from '../../interfaces/ArtifactType';
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

const CustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, handleNextStep: Function, handlePreviousStep: Function, getArtifactTypes: Function }) => {

    const classes = useStyles();

    const { register, handleSubmit, errors, getValues } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    const { artifactType, name, setName, setArtifactType } = useArtifact();

    React.useEffect(() => {
        (async () => {
            let artifactTypeAux = await props.getArtifactTypes(1);
            setArtifactType(artifactTypeAux[0]);
        })();
    }, [])

    const handleConfirm = async (data: { name: string }) => {
        setName(data.name);
        props.handleNextStep();
    }

    const handlePreviousStep = () => {
        let data = getValues();
        setName(data.name);
        setArtifactType(artifactType);
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
                    A continuación el nombre de su nuevo artefacto custom
                </Typography>
                <form className={classes.container}>
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
                        defaultValue={name}
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