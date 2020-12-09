import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { PROVIDERS } from '../../Constants';
import ArtifactType from '../../interfaces/ArtifactType';
import ArtifactService from '../../services/ArtifactService';
import Typography from '@material-ui/core/Typography';

// Once the ArtifactType API and service are running, this should be replaced with a call to that API
// Until then, you might an error if you don't have this 3 types created on your local DataBase before using this
const constArtifactTypes = [
    {
        "id": 1,
        "name": "Atype",
        "description": "ADescription"
    },
    {
        "id": 2,
        "name": "Btype",
        "description": "BDescription"
    },
    {
        "id": 3,
        "name": "Ctype",
        "description": "CDescription"
    }
]

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
        setArtifactTypes(constArtifactTypes);
    }, [])

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
                                    {artifactTypes.map(at => <MenuItem value={at.id}>{at.name}</MenuItem>)}
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