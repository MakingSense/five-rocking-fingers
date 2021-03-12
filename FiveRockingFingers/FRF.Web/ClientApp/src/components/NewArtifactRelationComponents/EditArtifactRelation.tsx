import * as React from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import { Controller, useForm } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Artifact from '../../interfaces/Artifact';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import ArtifactService from '../../services/ArtifactService';
import { Select } from '@material-ui/core';
import { PROVIDERS } from '../../Constants';

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

const EditArtifactRelation = (props: { open: boolean, closeEditArtifactsRelation: Function, artifactId: number, artifactRelations: ArtifactRelation, openSnackbar: Function, artifacts: Artifact[], updateList: Function,artifactsRelations: ArtifactRelation[] }) => {

    const classes = useStyles();
    const {handleSubmit, errors, control } = useForm();
    const [artifact1, setArtifact1] = React.useState<Artifact | null>(null);
    const [artifact2, setArtifact2] = React.useState<Artifact | null>(null);
    const [artifact1Settings, setArtifact1Settings] = React.useState<{ [key: string]: string }>({});
    const [artifact2Settings, setArtifact2Settings] = React.useState<{ [key: string]: string }>({});
    const [setting1, setSetting1] = React.useState<KeyValueStringPair | null>(null);
    const [setting2, setSetting2] = React.useState<KeyValueStringPair | null>(null);
    const [relationTypeId, setRelationTypeId] = React.useState<number>(props.artifactRelations.relationTypeId);
    const [relation, setRelation] = React.useState<ArtifactRelation>(props.artifactRelations);
    const [isErrorRelationRepeated, setIsErrorRelationRepeated] = React.useState<boolean>(false);
    const [isErrorEmptyField, setIsErrorEmptyField] = React.useState<boolean>(false);

    const updateArtifactsSettings = () => {
        props.updateList(true);
        if (artifact1 !== null && artifact1 !== undefined) {
            if (artifact1.artifactType?.name != PROVIDERS[1]) {
                var relationalSettings: { [key: string]: string } = {};
                Object.entries(artifact1.relationalFields).map(([key, value]) => relationalSettings[key] = artifact1.settings[key]);
                setArtifact1Settings(relationalSettings);
            } else {
                setArtifact1Settings(artifact1.settings);
            }
        }
        else {
            setArtifact1Settings({});
        }
        if (artifact2 !== null && artifact2 !== undefined) {
            if (artifact2.artifactType?.name != PROVIDERS[1]) {
                var relationalSettings: { [key: string]: string } = {};
                Object.entries(artifact2.relationalFields).map(([key, value]) => relationalSettings[key] = artifact2.settings[key]);
                setArtifact2Settings(relationalSettings);
            } else {
                setArtifact2Settings(artifact2.settings);
            }
        }
        else {
            setArtifact2Settings({});
        }
    }

    React.useEffect(() => {
        setArtifact1(props.artifacts.find(a => a.id === props.artifactRelations.artifact1.id) as Artifact);
        setArtifact2(props.artifacts.find(a => a.id === props.artifactRelations.artifact2.id) as Artifact);
    }, [props.open])

    React.useEffect(() => {
        updateArtifactsSettings();
    }, [artifact1, artifact2, relation])

    const areRelationsEqual = (relation: ArtifactRelation) => {
        if (artifact1 === null || artifact2 === null || setting1 === null || setting2 === null) {
            return false;
        }
        return (artifact1.name === relation.artifact1.name && artifact2.name === relation.artifact2.name && setting1.key === relation.artifact1Property && setting2.key === relation.artifact2Property && relationTypeId === relation.relationTypeId) || (artifact1.name === relation.artifact2.name && artifact2.name === relation.artifact1.name && setting1.key === relation.artifact2Property && setting2.key === relation.artifact1Property && relationTypeId === relation.relationTypeId);
    }

    const isRelationRepeated = () => {
        let flag = false
        let i = 0;

        if (artifact1 === null || artifact2 === null || setting1 === null || setting2 === null) {
            return flag;
        }

        if (!flag) {
            i = 0;
            while (!flag && i < props.artifactsRelations.length) {
                let relation = props.artifactsRelations[i];

                if (areRelationsEqual(relation)) {
                    flag = true;
                }

                i++
            }
        }

        return flag;
    }

    const resetState = () => {
        updateArtifactsSettings();
        setArtifact1(props.artifacts.find(a => a.id === props.artifactRelations.artifact1.id) as Artifact);
        setArtifact2(props.artifacts.find(a => a.id === props.artifactRelations.artifact2.id) as Artifact);
        setSetting1(null);
        setSetting2(null);
        setRelationTypeId(props.artifactRelations.relationTypeId);
        setRelation(props.artifactRelations);
        setIsErrorRelationRepeated(false);
        setIsErrorEmptyField(false);
    }

    const handleClose = () => {
        resetState();
        props.closeEditArtifactsRelation();
    }

    const hasFieldsEmpty = () => {
        if (artifact1 === null || artifact2 === null || setting1 === null || setting2 === null || relationTypeId === -1) {
            return true;
        }
        return false;
    }

    const handleConfirm = async () => {
        if (hasFieldsEmpty()) {
            setIsErrorEmptyField(true);
            return;
        }
        setIsErrorEmptyField(false);
        if (isRelationRepeated()) {
            setIsErrorRelationRepeated(true);
            return;
        }
        else setIsErrorRelationRepeated(false);

        let artifactsRelationsList: any[] = [];
        let artifactsRelation = {
            id: relation.id,
            artifact1Id: artifact1!.id,
            artifact2Id: artifact2!.id,
            artifact1Property: setting1!.key,
            artifact2Property: setting2!.key,
            relationTypeId: relationTypeId
        };
        artifactsRelationsList.push(artifactsRelation);
        try {
            let response = await ArtifactService.updateArtifactsRelations(props.artifactId, artifactsRelationsList);
            if (response.status === 200) {
                props.openSnackbar({ message: "La relacion ha sido modificada con éxito", severity: "success" });
                props.updateList();
            } else {
                props.openSnackbar({ message: "Hubo un error al modificar la relacion", severity: "error" });
            }
        }
        catch (error) {
            props.openSnackbar({ message: "Hubo un error al modificar la relacion", severity: "error" });
        }
        handleClose();
    }

    const handleChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        if (event.target.name === 'artifact1') {
            setSetting1(null)
            setArtifact1(props.artifacts.find(a => a.id === event.target.value) as Artifact);
        }
        else if (event.target.name === 'artifact2') {
            setSetting2(null)
            setArtifact2(props.artifacts.find(a => a.id === event.target.value) as Artifact);
        }
    }

    const handleSettingChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        if (event.target.name === 'setting1') {
            let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact1Settings[event.target.value as string] };
            setSetting1(setting);
        }
        else if (event.target.name === 'setting2') {
            let setting: KeyValueStringPair = { key: event.target.value as string, value: artifact2Settings[event.target.value as string] };
            setSetting2(setting);
        }
    }

    const handleRelationTypeChange = (event: React.ChangeEvent<{ name?: string | undefined; value: unknown }>) => {
        setRelationTypeId(event.target.value as number);
    }

    const toRegularSentence = (value: string) => {
        // Value will be null only when you select a Custom artifact setting, since their type selection hasn't been implemented
        // REMOVE ONCE IT'S DONE
        if (!value) return "";

        const result = value.replace(/([A-Z])/g, ' $1');
        const final = result.charAt(0).toUpperCase() + result.slice(1);
        return final;
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
                                    defaultValue={artifact1?.id}
                                    error={false}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
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
                                    defaultValue={artifact2?.id}
                                    error={false}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {props.artifacts.map(a => <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>)}
                                </Select>
                            }
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
                                    value={setting1 ? setting1.key : ''}
                                    error={false}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact1Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{toRegularSentence(key)}</MenuItem>)}
                                </Select>
                            }
                            name="setting1"
                            control={control}
                            defaultValue={''}
                        />
                        <FormHelperText>{setting1 && `${setting1.value} (${artifact1 && toRegularSentence(artifact1.relationalFields[setting1.key])})`}</FormHelperText>
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
                                </Select>
                            }
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
                                    value={setting2 ? setting2.key : ''}
                                    error={false}
                                >
                                    <MenuItem value="">
                                        <em>None</em>
                                    </MenuItem>
                                    {Object.entries(artifact2Settings).map(([key, value], index) => <MenuItem key={key} value={key}>{toRegularSentence(key)}</MenuItem>)}
                                </Select>
                            }
                            name="setting2"
                            control={control}
                            defaultValue={''}
                        />
                        <FormHelperText>{setting2 && `${setting2.value} (${artifact2 && toRegularSentence(artifact2?.relationalFields[setting2.key])})`}</FormHelperText>
                    </FormControl>
                    {errors.settings ?
                        <Typography gutterBottom className={classes.error}>
                            Al menos debe modificar una relación
                        </Typography> : null
                    }
                    {errors.settings ?
                        <Typography gutterBottom className={classes.error}>
                            Todos los campos deben ser completados para modificar una relación
                        </Typography> : null
                    }
                    {isErrorRelationRepeated ?
                        <Typography gutterBottom className={classes.error}>
                            La relación ya existe
                        </Typography> : null
                    }
                    {isErrorEmptyField ?
                        <Typography gutterBottom className={classes.error}>
                            Todos los campos deben ser completados para modificar una relación
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