import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, SimplePaletteColorOptions } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { Controller, useForm } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Artifact from '../../interfaces/Artifact';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import SyncAltIcon from '@material-ui/icons/SyncAlt';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import ArtifactService from '../../services/ArtifactService';
import { Select } from '@material-ui/core';

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
        selectArtifact: {
            margin: theme.spacing(1),
            width: '40%'
        },
        selectSetting: {
            margin: theme.spacing(1),
            width: '25%'
        },
        selectDirection: {
            margin: theme.spacing(1),
            width: '20%'
        },
        inputSelect: {
            width: '100%'
        },
        error: {
            color: 'red'
        }
    }),
);

const EditArtifactRelation = (props: { open: boolean, closeEditArtifactsRelation: Function, artifactId: string, artifactRelations: ArtifactRelation, setOpenSnackbar: Function, setSnackbarSettings: Function, artifacts: Artifact[] }) => {

    const classes = useStyles();
    const { register, handleSubmit, errors, control, getValues } = useForm();
    const [artifact1, setArtifact1] = React.useState<Artifact | null>(null);
    const [artifact2, setArtifact2] = React.useState<Artifact | null>(null);
    const [artifact1Settings, setArtifact1Settings] = React.useState<{ [key: string]: string }>({});
    const [artifact2Settings, setArtifact2Settings] = React.useState<{ [key: string]: string }>({});
    const [setting1, setSetting1] = React.useState<KeyValueStringPair | null>({key:props.artifactRelations.artifact1Property, value:'' });
    const [setting2, setSetting2] = React.useState<KeyValueStringPair | null>({key:props.artifactRelations.artifact2Property, value:'' });
    const [relationTypeId, setRelationTypeId] = React.useState<number>(-1);
    const [relation, setRelation] = React.useState<ArtifactRelation>(props.artifactRelations);

    React.useEffect(() => {
        updateArtifactsSettings1();
        updateArtifactsSettings2();
    }, [artifact1, artifact2, relation])

    const handleClose = () => {
        setArtifact1(null);
        setArtifact2(null);
        updateArtifactsSettings1();
        updateArtifactsSettings2();
        setSetting1(null);
        setSetting2(null);
        setRelationTypeId(-1);
        setRelation(props.artifactRelations);
        props.closeEditArtifactsRelation();
    }

    const isRelationRepeated = () => {
        let flag = false
        let i = 0;
        if (artifact1 === null || artifact2 === null || setting1 === null || setting2 === null) {
            return flag;
        }
        if ((artifact1.name === relation.artifact1.name && artifact2.name === relation.artifact2.name && setting1.key === relation.artifact1Property && setting2.key === relation.artifact2Property) || (artifact1.name === relation.artifact2.name && artifact2.name === relation.artifact1.name && setting1.key === relation.artifact2Property && setting2.key === relation.artifact1Property)) {
            flag = true;
        }

        return flag;
    }

    const handleConfirm = async () => {
        let artifactsRelationsList: any[] = [];
        let artifactsRelation = {
            id: relation.id,
            artifact1Id: relation.artifact1.id,
            artifact2Id: relation.artifact2.id,
            artifact1Property: relation.artifact1Property,
            artifact2Property: relation.artifact2Property,
            relationTypeId: relation.relationTypeId
        };
        artifactsRelationsList.push(artifactsRelation);
        try {
            let response = await ArtifactService.setRelations(artifactsRelationsList);
            if (response.status === 200) {
                props.setSnackbarSettings({ message: "Las relaciones han sido creado con éxito", severity: "success" });
                props.setOpenSnackbar(true);
            } else {
                props.setSnackbarSettings({ message: "Hubo un error al crear las relaciones", severity: "error" });
                props.setOpenSnackbar(true);
            }
        }
        catch (error) {
            props.setSnackbarSettings({ message: "Hubo un error al crear las relaciones", severity: "error" });
            props.setOpenSnackbar(true);
        }
        handleClose();
    }

    const updateArtifactsSettings1 = () => {
        if (artifact1 !== null && artifact1 !== undefined) {
            setArtifact1Settings(artifact1.settings);
        }
        else {
            setArtifact1Settings({});
        }
    }

    const updateArtifactsSettings2 = () => {
        if (artifact2 !== null && artifact2 !== undefined) {
            setArtifact2Settings(artifact2.settings);
        }
        else {
            setArtifact2Settings({});
        }
    }

    const handleChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        if (event.target.name === 'artifact1') {
            setArtifact1(props.artifacts.find(a => a.id === event.target.value) as Artifact);
        }
        else if (event.target.name === 'artifact2') {
            setArtifact2(props.artifacts.find(a => a.id === event.target.value) as Artifact);
        }
    }

    const handleSettingChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        if (event.target.name === 'setting1') {
            let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact1Settings[event.target.value as string] };
            setSetting1(setting);
        }
        else if (event.target.name === 'setting2') {
            let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact1Settings[event.target.value as string] };
            setSetting2(setting);
        }
    }

    const handleRelationTypeChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        setRelationTypeId(event.target.value as number);
    }

    const isOneRelationCreated = () => {
        //return relation;
        return false;
    }

    return (
        <Dialog open={props.open}>
            <DialogTitle id="alert-dialog-title">Modificar relación entre artefactos</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    Seleccione los artefactos que desea relacionar.
                </Typography>
                <form className={classes.container}>
                    <FormControl className={classes.selectArtifact} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select1">Artefacto 1</InputLabel>
                        <Controller
                            render={({ onChange }) =>
                                <Select
                                    inputProps={{
                                        name: 'artifact1',
                                        id: 'type-select1'
                                    }}
                                    onChange={(event) => handleChange(event)}
                                    /*defaultValue={props.artifactRelations.artifact2.id}*/
                                    defaultValue={''}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
                            rules={{ validate: { isValid: () => isOneRelationCreated() } }}
                            name='artifact1'
                            control={control}
                            defaultValue={''}
                        />
                    </FormControl>
                    <FormControl className={classes.selectArtifact} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select2">Artefacto 2</InputLabel>
                        <Controller
                            render={({ onChange }) =>
                                <Select
                                    inputProps={{
                                        name: 'artifact2',
                                        id: 'type-select2'
                                    }}
                                    onChange={(event) => handleChange(event)}
                                    /*defaultValue={props.artifactRelations.artifact2.id}*/
                                    defaultValue={''}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
                            rules={{ validate: { isValid: () => isOneRelationCreated() } }}
                            name='artifact2'
                            control={control}
                            defaultValue={''}
                        />
                    </FormControl>
                    <Typography gutterBottom>
                        Ahora seleccione las propiedades que desea relacionar y las direcciones de la relación
                    </Typography>
                    <FormControl className={classes.selectSetting} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select">Setting 1</InputLabel>
                        <Controller
                            render={({ onChange }) =>
                                <Select
                                    inputProps={{
                                        name: 'setting1',
                                        id: 'type-select'
                                    }}
                                    onChange={(event) => handleSettingChange(event)}
                                    defaultValue={''}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact1Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{key}</MenuItem>)}
                                </Select>
                            }
                            rules={{ validate: { isValid: () => isOneRelationCreated() } }}
                            name="setting1"
                            control={control}
                            defaultValue={''}
                        />
                        <FormHelperText>{setting1 !== null ? setting1?.value : null}</FormHelperText>
                    </FormControl>
                    <FormControl className={classes.selectDirection} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select">Dirección</InputLabel>
                        <Controller
                            render={({ onChange }) =>
                                <Select
                                    inputProps={{
                                        name: 'artifactType',
                                        id: 'type-select'
                                    }}
                                    onChange={(event) => handleRelationTypeChange(event)}
                                    defaultValue={props.artifactRelations.relationTypeId}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    <MenuItem value="0">
                                        <em><ArrowForwardIcon /></em>
                                    </MenuItem>
                                    <MenuItem value="1">
                                        <em><ArrowBackIcon /></em>
                                    </MenuItem>
                                    <MenuItem value="2">
                                        <em><SyncAltIcon /></em>
                                    </MenuItem>
                                </Select>
                            }
                            rules={{ validate: { isValid: () => isOneRelationCreated() } }}
                            name="artifactType"
                            control={control}
                            defaultValue={''}
                        />
                    </FormControl>
                    <FormControl className={classes.selectSetting} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select">Setting 2</InputLabel>
                        <Controller
                            render={({ onChange }) =>
                                <Select
                                    inputProps={{
                                        name: 'setting2',
                                        id: 'type-select'
                                    }}
                                    onChange={(event) => handleSettingChange(event)}
                                    defaultValue={''}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact2Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{key}</MenuItem>)}
                                </Select>
                            }
                            rules={{ validate: { isValid: () => isOneRelationCreated() } }}
                            name="setting2"
                            control={control}
                            defaultValue={''}
                        />
                        <FormHelperText>{setting2 !== null ? setting2?.value : null}</FormHelperText>
                    </FormControl>
                    {errors.settings ?
                        <Typography gutterBottom className={classes.error}>
                            Al menos debe crear una relación
                        </Typography> : null
                    }
                    {errors.settings ?
                        <Typography gutterBottom className={classes.error}>
                            Todos los campos deben ser completados para crear una relación
                        </Typography> : null
                    }
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleClose}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default EditArtifactRelation;