import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, IconButton, ButtonGroup } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm, Controller } from 'react-hook-form';
import ArtifactService from '../../services/ArtifactService';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';


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

interface Setting {
    name: string;
    value: string;
    [key: string]: string;
}

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, provider: string | null, name: string | null, projectId: number, artifactTypeId: number | null, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function, settingsList: Setting[], setSettingsList: Function, settingsMap: { [key: string]: number[] }, setSettingsMap: Function }) => {

    const classes = useStyles();
    const { handleSubmit, register, errors, setError, clearErrors, control, getValues } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, provider, name, projectId, artifactTypeId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    //Hook for save the user's settings input
    const [settingsList, setSettingsList] = React.useState<Setting[]>(props.settingsList);
    //Hook for saving the numbers of times a setting's name input is repeated
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>(props.settingsMap);

    React.useEffect(() => {
        setNameSettingsErrors();
    }, [settingsMap]);

    //Create the artifact after submit
    const handleConfirm = async () => {
        const artifactToCreate = {
            name: name,
            provider: provider,
            artifactTypeId: artifactTypeId,
            projectId: projectId,
            settings: { settings: createSettingsObject()}
        };

        try {
            const response = await ArtifactService.save(artifactToCreate);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El artefacto ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateList();
            } else {
                setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
                setOpenSnackbar(true);
            }
        }
        catch (error) {
            setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
            setOpenSnackbar(true);
        }
        closeNewArtifactDialog();
    }

    //Handle changes in the inputs fields
    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
        console.log(event.target.value);
        let { name, value } = event.target;
        name = name.split(".")[1];
        if (name === 'name') {
            checkSettingName(value, index);
        }
        const list = [...settingsList];
        list[index][name] = value;
        setSettingsList(list);
    }

    //Check if the setting's name the user enters has already have been used
    const checkSettingName = (settingName: string, index: number) => {
        let mapList = { ...settingsMap };
        let key = searchIndexInObject(mapList, index);
        if (key != null) {
            deleteIndexFromObject(mapList, index, key);
        }
        if (!settingsMap.hasOwnProperty(settingName)) {
            mapList[settingName] = [index];
            setSettingsMap(mapList);
        }
        else {
            mapList[settingName].push(index);
            setSettingsMap(mapList);
        }
    }

    //Search the key of the input index in settingsMap
    const searchIndexInObject = (object: { [key: string]: number[] }, index: number) => {
        for (let [key, array] of Object.entries(object)) {
            for (let i = 0; i < array.length; i++) {
                if (index === array[i]) {
                    return key;
                }
            }
        }
        return null;
    }

    //Delete the input index in settingsMap
    const deleteIndexFromObject = (object: { [key: string]: number[] }, index: number, key: string) => {
        if (key !== null) {
            object[key] = object[key].filter(number => number !== index);
            if (object[key].length === 0) {
                delete object[key];
            }
        }
    }

    //Set errors if the setting's name the user enters are repeat
    const setNameSettingsErrors = () => {
        for (let [key, array] of Object.entries(settingsMap)) {
            if (array.length > 1) {
                for (let i = 0; i < array.length; i++) {
                    setError(`settings[${array[i]}].name`, {
                        type: "repeat",
                        message: "Los nombres no pueden repetirse"
                    });
                }
            }
            else if (array.length === 1) {
                clearErrors(`settings[${array[0]}].name`);
            }
        }
    }

    //Check if there are names repeated in settingsMap
    const areNamesRepeated = (index: number) => {
        let key = searchIndexInObject(settingsMap, index);
        if (key !=null && settingsMap[key].length > 1) {
            return true;
        }
        return false;
    }

    const isFieldEmpty = (index: number, field: string) => {
        if (settingsList[index][field].trim() === "") {
            return true;
        }
        return false;
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const handleAddSetting = () => {
        setSettingsList([...settingsList, { name: "", value: "" }]);
    }

    const handleDeleteSetting = (index: number) => {
        const list = [...settingsList];
        list.splice(index, 1);
        setSettingsList(list);
        let mapList = { ...settingsMap };
        let key = searchIndexInObject(mapList, index);
        if (key != null) {
            deleteIndexFromObject(mapList, index, key);
            updateSettingsMap(mapList, index);
        }
        setSettingsMap(mapList);
    }

    const updateSettingsMap = (object: { [key: string]: number[] }, index: number) => {
        for (let [key, array] of Object.entries(object)) {
            for (let i = 0; i < array.length; i++) {
                if (array[i] > index) {
                    array[i] = array[i] - 1;
                }
            }
        }
    }

    //Create and return the settings object for the create the artifact
    const createSettingsObject = () => {
        let settingsObject: { [key: string]: string } = {};

        for (let i = 0; i < settingsList.length; i++) {
            settingsObject[settingsList[i].name] = settingsList[i].value;
        }

        return settingsObject;
    }

    const goPrevStep = () => {
        props.setSettingsList(settingsList);
        props.setSettingsMap(settingsMap);
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos custom</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto custom y el valor que tomarán esas propiedades
                </Typography>
                <form className={classes.container}>
                    {settingsList.map((setting: Setting, index: number) => {
                        let value = getValues(`settings[${index}].name`)
                        return (
                            <React.Fragment>
                                <Controller
                                    control={control}
                                    name={`settings[${index}].name`}
                                    rules={{ validate: { isValid: () => !isFieldEmpty(index, "name"), isRepeate: () => !areNamesRepeated(index) } }}
                                    render={({ onChange }) => (
                                        <TextField
                                            error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined'}
                                            id={`name[${index}]`}
                                            name={`settings[${index}].name`}
                                            label="Nombre de la setting"
                                            helperText={areNamesRepeated(index) ? "Los nombres no pueden repetirse" : "Requerido*"}
                                            variant="outlined"
                                            defaultValue={setting.name}
                                            value={setting.name}
                                            className={classes.inputF}
                                            onChange={event => { handleInputChange(event, index); onChange(event); }}
                                            autoComplete='off'
                                        />
                                    )}
                                />

                                <Controller
                                    control={control}
                                    name={`settings[${index}].value`}
                                    rules={{ validate: { isValid: () => !isFieldEmpty(index, "value") }}}
                                    render={({ onChange }) => (
                                        <TextField
                                            error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.value !== 'undefined'}
                                            id={`value[${index}]`}
                                            name={`settings[${index}].value`}
                                            label="Valor de la setting"
                                            helperText="Requerido*"
                                            variant="outlined"
                                            defaultValue={setting.value}
                                            value={setting.value}
                                            className={classes.inputF}
                                            onChange={event => { handleInputChange(event, index); onChange(event); }}
                                            autoComplete='off'
                                        />
                                    )}
                                />

                                <ButtonGroup>
                                    {settingsList.length - 1 === index &&
                                        <IconButton onClick={handleAddSetting } aria-label="add" color="primary">
                                            <AddCircleIcon />
                                        </IconButton>
                                    }

                                    {settingsList.length !== 1 &&
                                        <IconButton onClick={event => handleDeleteSetting(index)} aria-label="delete" color="secondary">
                                            <DeleteIcon />
                                        </IconButton>
                                    }
                                </ButtonGroup>
                        </React.Fragment>
                        ); 
                    })}
                    
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsCustomForm;