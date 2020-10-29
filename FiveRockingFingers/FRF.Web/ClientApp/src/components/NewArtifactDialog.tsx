import * as React from 'react';
import { Dialog, DialogActions, Button, DialogTitle, DialogContent, Select, MenuItem, InputLabel, FormControl, TextField, FormHelperText } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Project from '../interfaces/Project';
import ArtifactType from '../interfaces/ArtifactType';
import axios from 'axios';
import { useForm, Controller } from 'react-hook-form';

const providers = ["custom", "AWS"];

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
            display: 'flex',
            flexWrap: 'wrap',
        },
        formControl: {
            margin: theme.spacing(1),
            minWidth: 150,
        },
        inputF: {
            padding: 2,
            marginTop: 10
        }
    }),
);

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: any, project: Project }) => {

    const classes = useStyles();

    const { register, handleSubmit, errors, control } = useForm();

    const { showNewArtifactDialog, closeNewArtifactDialog, project } = props;
    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);
    const [state, setState] = React.useState<{ projectId: number; provider: string; artifactType: string; name: string; prueba: string }>({
        projectId: project.id,
        provider: '',
        artifactType: '',
        name: '',
        prueba: ''
    });

    React.useEffect(() => {
        getArtifactTypes();
    })

    const handleChange = (event: React.ChangeEvent<{ name?: string; value: unknown }>) => {
        const name = event.target.name as keyof typeof state;
        if (typeof event.target.value === "string") {
            setState({
                ...state,
                [name]: event.target.value,
            });
        }
    };

    const getArtifactTypes = async () => {
        try {
            const response = await axios.get("http://localhost:3000/ArtifactType");
            setArtifactTypes(response.data);
        }
        catch {
            console.log("error en la carga");
        }
    }

    const handleConfirm = (data) => {
        setState({
            ...state,
            provider: data.provider
        });

        const { projectId, name, provider, artifactType, prueba } = state;
        console.log(projectId, name, provider, artifactType, prueba)
        console.log(data)
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
                                    {artifactTypes.map(at => <MenuItem value={at.name}>{at.name}</MenuItem>)}
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
                        onChange={handleChange}
                        className={classes.inputF}
                    />
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" type="submit" onClick={handleSubmit(handleConfirm)}>Agregar</Button>
                <Button size="small" color="secondary" onClick={closeNewArtifactDialog}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default NewArtifactDialog