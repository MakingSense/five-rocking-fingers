import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { Controller, useForm } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Artifact from '../interfaces/Artifact';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';
import ArtifactRelation from '../interfaces/ArtifactRelation';
import RelationCard from './NewArtifactRelationComponents/RelationCard';
import SyncAltIcon from '@material-ui/icons/SyncAlt';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import ArtifactService from '../services/ArtifactService';

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

const NewArtifactsRelation = (props: { showNewArtifactsRelation: boolean, closeNewArtifactsRelation: Function, projectId: number, setOpenSnackbar: Function, setSnackbarSettings: Function, artifacts: Artifact[] }) => {

    const classes = useStyles();
    const { register, handleSubmit, errors, control, getValues } = useForm();
    const [artifact1, setArtifact1] = React.useState<Artifact | null>(null);
    const [artifact2, setArtifact2] = React.useState<Artifact | null>(null);
    const [artifact1Settings, setArtifact1Settings] = React.useState<{ [key: string]: string }>({});
    const [artifact2Settings, setArtifact2Settings] = React.useState<{ [key: string]: string }>({});
    const [setting1, setSetting1] = React.useState<KeyValueStringPair | null>(null);
    const [setting2, setSetting2] = React.useState<KeyValueStringPair | null>(null);
    const [relationTypeId, setRelationTypeId] = React.useState<number>(-1);
    const [relationList, setRelationList] = React.useState<ArtifactRelation[]>([]);

    React.useEffect(() => {
        updateArtifactsSettings1();
        updateArtifactsSettings2();
    }, [artifact1, artifact2, relationList])

    const handleClose = () => {
        setArtifact1(null);
        setArtifact2(null);
        updateArtifactsSettings1();
        updateArtifactsSettings2();
        setSetting1(null);
        setSetting2(null);
        setRelationTypeId(-1);
        setRelationList([]);
        props.closeNewArtifactsRelation();
    }

    const isRelationRepeated = () => {
        let flag = false
        let i = 0;
        while (!flag && i < relationList.length) {

            let relation = relationList[i];
            if (artifact1 === null || artifact2 === null || setting1 === null || setting2 === null) {
                return flag;
            }
            if ((artifact1.name === relation.artifact1.name && artifact2.name === relation.artifact2.name && setting1.key === relation.setting1.key && setting2.key === relation.setting2.key) || (artifact1.name === relation.artifact2.name && artifact2.name === relation.artifact1.name && setting1.key === relation.setting2.key && setting2.key === relation.setting1.key)) {
                flag = true;
            }

            i++
        }

        return flag;
    }

    const handleConfirm = async () => {
        let artifactsRelationsList: any[] = [];
        relationList.map(relation => {
            let artifactsRelation = {
                artifact1Id: relation.artifact1.id,
                artifact2Id: relation.artifact2.id,
                artifact1Property: relation.setting1.key,
                artifact2Property: relation.setting2.key,
                relationTypeId: relation.relationTypeId
            };
            artifactsRelationsList.push(artifactsRelation);
        });
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

    const deleteRelation = (index: number) => {
        let relationListCopy = [...relationList];
        relationListCopy.splice(index, 1);
        setRelationList(relationListCopy);
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
            let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact1Settings[event.target.value as string]};
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
        return relationList.length > 0;
    }

    const addRelation = () => {
        if (isRelationRepeated()) {
            console.log("Esta repetida");
            return;
        }
        let newRelation: ArtifactRelation = {
            id: null,
            artifact1: artifact1 as Artifact,
            artifact2: artifact2 as Artifact,
            setting1: setting1 as KeyValueStringPair,
            setting2: setting2 as KeyValueStringPair,
            relationTypeId: relationTypeId
        }
        let relationListCopy = [...relationList];
        relationListCopy.push(newRelation);
        setRelationList(relationListCopy);
    }

    return (
        <Dialog open={props.showNewArtifactsRelation}>
            <DialogTitle id="alert-dialog-title">Formulario de para crear relación entre artefactos</DialogTitle>
            <DialogContent>
                {relationList.map((relation, index) => 
                    <RelationCard
                        Relation={relation}
                        deleteRelation={deleteRelation}
                        index={index}
                    />
                )}
                <hr />
                <Typography gutterBottom>
                    Seleccione los artefactos que desea relacionar.
                </Typography>
                <form className={classes.container}>
                    <FormControl className={classes.selectArtifact} error={Boolean(errors.artifactType)}>
                        <InputLabel htmlFor="type-select1">Artefacto 1</InputLabel>
                        <Controller
                            render={({onChange}) =>
                                <Select
                                    inputProps={{
                                        name: 'artifact1',
                                        id: 'type-select1'
                                    }}
                                    onChange={(event) => handleChange(event)}
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
                                    defaultValue={''}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    <MenuItem value="0">
                                        <em><ArrowForwardIcon/></em>
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
                <Button size="small" color="primary" type="submit" onClick={addRelation}>Agregar</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleClose}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default NewArtifactsRelation;