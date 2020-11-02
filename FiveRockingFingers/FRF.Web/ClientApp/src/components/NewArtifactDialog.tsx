import * as React from 'react';
import { Dialog, DialogActions, Button, DialogTitle, DialogContent, Select, MenuItem, InputLabel, FormControl, TextField, FormHelperText } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Project from '../interfaces/Project';
import ArtifactType from '../interfaces/ArtifactType';
import axios from 'axios';
import { useForm, Controller } from 'react-hook-form';

const providers = ["custom", "AWS"];

// Once the ArtifactType API and service are running,
// this should be replaced with a call to that API
const constArtifactTypes = [
    {
      "id": 1,
      "name": "Atype",
      "description": "asdasd"
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

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, project: Project, openSnackbar: Function }) => {

    const classes = useStyles();

    const { register, handleSubmit, errors, control } = useForm();

    const { showNewArtifactDialog, closeNewArtifactDialog, project, openSnackbar } = props;
    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);

    React.useEffect(() => {
        setArtifactTypes(constArtifactTypes);
    }, [])

    const handleConfirm = async (data: { name: string, provider: string, artifactType: string }) => {
        const artifactToCreate = {
            name: data.name,
            provider: data.provider,
            artifactType: artifactTypes.find(at => at.id === parseInt(data.artifactType)),
            projectId: props.project.id,
            settings: { empty: "" }
        };

        try {
            const response = await axios.post("https://localhost:44346/api/Artifacts/Save", artifactToCreate);
            console.log(response);
            if (response.status === 200) {
                openSnackbar("Creaci\u00F3n del artefacto exitosa", "success");
            } else {
                openSnackbar("Ocurri\u00F3 un error al crear el artefacto", "warning");
            }
        }
        catch (error) {
            openSnackbar("Ocurri\u00F3 un error al crear el artefacto", "warning");
        }
        closeNewArtifactDialog()
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Agregar un nuevo artefacto a {project.name}</DialogTitle>
            <DialogContent>
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
                                        <em>None</em>
                                    </MenuItem>
                                    {providers.map(p => <MenuItem value={p}>{p}</MenuItem>)}
                                </Select>
                            }
                            name="provider"
                            rules={{ required: true }}
                            control={control}
                            defaultValue=""
                        />
                        <FormHelperText>Requerido*</FormHelperText>
                    </FormControl>
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
                            defaultValue=""
                        />
                        <FormHelperText>Requerido*</FormHelperText>
                    </FormControl>
                    <TextField
                        inputRef={register({ required: true })}
                        error={errors.name ? true : false}
                        id="name"
                        name="name"
                        label="Nombre del artefacto"
                        helperText="Requerido*"
                        variant="outlined"
                        className={classes.inputF}
                        fullWidth
                        />
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" type="submit" onClick={handleSubmit(handleConfirm)}>Agregar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default NewArtifactDialog