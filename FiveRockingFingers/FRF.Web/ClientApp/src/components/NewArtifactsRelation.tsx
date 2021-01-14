import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { Controller, useForm } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Artifact from '../interfaces/Artifact';
import AwsArtifact from '../interfaces/AwsArtifact';
import SyncAltIcon from '@material-ui/icons/SyncAlt';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';

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
    const [setting1, setSetting1] = React.useState< AwsArtifact | null>(null);
    const [setting2, setSetting2] = React.useState<AwsArtifact | null>(null);

    React.useEffect(() => {
        updateArtifactsSettings1();
        updateArtifactsSettings2();
    }, [artifact1, artifact2])

    const handleCancel = () => {
        setArtifact1(null);
        setArtifact2(null);
        updateArtifactsSettings1();
        updateArtifactsSettings2();
        setSetting1(null);
        setSetting2(null);
        props.closeNewArtifactsRelation();
    }

    let parser = new DOMParser();

    const handleConfirm = () => {

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
            let setting: AwsArtifact = { key: event.target.value as string, value: artifact1Settings[event.target.value as string]};
            setSetting1(setting);
        }
        else if (event.target.name === 'setting2') {
            let setting: AwsArtifact = { key: event.target.value as string, value: artifact1Settings[event.target.value as string] };
            setSetting2(setting);
        }
    }

    return (
        <Dialog open={props.showNewArtifactsRelation}>
            <DialogTitle id="alert-dialog-title">Formulario de para crear relación entre artefactos</DialogTitle>
            <DialogContent>
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
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
                            name='artifact1'
                            rules={{ required: true }}
                            control={control}
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
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
                            name='artifact2'
                            rules={{ required: true }}
                            control={control}
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
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact1Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{key}</MenuItem>)}
                                </Select>
                            }
                            name="setting1"
                            rules={{ required: true }}
                            control={control}
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
                            name="artifactType"
                            rules={{ required: true }}
                            control={control}
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
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact2Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{key}</MenuItem>)}
                                </Select>
                            }
                            name="setting2"
                            rules={{ required: true }}
                            control={control}
                        />
                        <FormHelperText>{setting2 !== null ? setting2?.value : null}</FormHelperText>
                    </FormControl>
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default NewArtifactsRelation;