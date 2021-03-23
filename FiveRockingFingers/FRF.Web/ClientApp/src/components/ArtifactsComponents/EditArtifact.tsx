import { Button, DialogActions, DialogContent, DialogTitle, TextField, IconButton, ButtonGroup } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm, Controller } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';
import Setting from '../../interfaces/Setting';
import Artifact from '../../interfaces/Artifact';
import ArtifactRelation from '../../interfaces/ArtifactRelation';

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

const EditArtifact = (props: {
    artifactToEdit: Artifact,
    closeEditArtifactDialog: Function,
    setIsArtifactEdited: Function,
    setArtifactEdited: Function,
    setNamesOfSettingsChanged: Function,
    artifactsRelations: ArtifactRelation[]}) => {

    const classes = useStyles();
    const { register, handleSubmit, errors, setError, clearErrors, control } = useForm();
    const { closeEditArtifactDialog } = props;

    const createSettingsListFromArtifact = () => {
        let settingsListFromArtifact: Setting[] = [];
        if (props.artifactToEdit.id === 0) {
            return settingsListFromArtifact;
        }
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            let settingFromArtifact: Setting = Object.assign({ name: key, value: value });
            settingsListFromArtifact.push(settingFromArtifact);
        });
        return settingsListFromArtifact;
    }

    //Hook for save the user's settings input
    const [settingsList, setSettingsList] = React.useState<Setting[]>(createSettingsListFromArtifact());
    const [price, setPrice] = React.useState(() => {
        let index = settingsList.findIndex(s => s.name === 'price');
        if (index != -1) {
            let price = settingsList[index];
            settingsList.splice(index, 1);
            setSettingsList(settingsList);
            return price.value;
        }
        return 0;
    });
    const [artifactName, setArtifactName] = React.useState(props.artifactToEdit.name);

    const createSettingsMapFromArtifact = () => {
        let settingsMapFromArtifact: { [key: string]: number[] } = {};
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            if (key !== 'price') {
                settingsMapFromArtifact[key] = [index];
            }            
        });
        return settingsMapFromArtifact;
    }

    //Hook for saving the numbers of times a setting's name input is repeated
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>(createSettingsMapFromArtifact());

    React.useEffect(() => {
        setNameSettingsErrors();
    }, [settingsMap, props.artifactToEdit]);

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (!settingsList.find(s => s.name === 'price')) {
            settingsList.unshift({ name: 'price', value: price.toString() });
        }
        let artifactEdited = Object.create(props.artifactToEdit);
        artifactEdited.name = artifactName;
        artifactEdited.settings = createSettingsObject();
        props.setArtifactEdited(artifactEdited);
        props.setNamesOfSettingsChanged(getNamesOfSettingsChanged());
        props.setIsArtifactEdited(true);
    }

    const getNamesOfSettingsChanged = () => {
        let namesOfSettingsChanged : string[] = [];
        let originalSettingsList = createSettingsListFromArtifact();
        let index = originalSettingsList.findIndex(s => s.name === 'price');
        if (index != -1) {
            originalSettingsList.splice(index, 1);
        }
        originalSettingsList.forEach((setting, index) => {
            if (!settingsList.find(s => s.name === setting.name)) {
                namesOfSettingsChanged.push(setting.name);
            }
        });
        return namesOfSettingsChanged;
    }

    const handleChangeName = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        setArtifactName(event.target.value);
    }

    const handleChangePrice = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        setPrice(parseInt(event.target.value, 10));
    }

    const isPriceValid = () => {
        if (price >= 0) return true
        return false
    }

    //Handle changes in the inputs fields
    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
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
        if (key != null && (settingsMap[key].length > 1 || key === 'price')) {
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
        closeEditArtifactDialog();
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

    const isSettingAtTheEndOfAnyRelation = (settingName: string) => {
        const flag: ArtifactRelation | undefined = props.artifactsRelations
            .find((relation) =>
                isSettingAtTheEndOfRelation(settingName, relation));

        return flag === undefined ? false : true;
    }

    const isSettingAtTheEndOfRelation = (settingName: string, relation: ArtifactRelation) => {
        if (relation.relationTypeId === 0) {
            return relation.artifact2Property === settingName;
        }
        else {
            return relation.artifact1Property === settingName;
        }
    }

    return (
        <>
            <DialogTitle id="alert-dialog-title">Formulario de actualización de artefactos custom</DialogTitle>
            <DialogContent>
                <Controller
                    control={control}
                    name={'price.value'}
                    rules={{ validate: { isValid: () => isPriceValid() } }}
                    render={({ onChange }) => (
                        <TextField
                            inputRef={register({ required: true, validate: { isValid: value => value.trim() != "" } })}
                            error={errors.name ? true : false}
                            id="name"
                            name="name"
                            label="Nombre del artefacto"
                            helperText="Requerido*"
                            variant="outlined"
                            className={classes.inputF}
                            onChange={event => { handleChangeName(event); onChange(event); }}
                            fullWidth
                            defaultValue={props.artifactToEdit.name}
                        />
                    )}
                />
                <Typography gutterBottom>
                    Propiedades del artefacto
                </Typography>
                <form className={classes.container}>
                    <TextField
                        disabled
                        label="Nombre de la setting"
                        helperText={"Requerido*"}
                        variant="outlined"
                        defaultValue='Precio'
                        value='Precio'
                        className={classes.inputF}
                    />
                    <Controller
                        control={control}
                        name={'price.value'}
                        rules={{ validate: { isValid: () => isPriceValid() } }}
                        render={({ onChange }) => (
                            <TextField
                                error={!isPriceValid()}
                                label="Valor de la setting"
                                helperText="Requerido*"
                                variant="outlined"
                                defaultValue={price}
                                value={price}
                                className={classes.inputF}
                                onChange={event => { handleChangePrice(event); onChange(event); }}
                                autoComplete='off'
                                type="number"
                            />
                        )}
                    />
                    {settingsList.map((setting: Setting, index: number) => {
                        return (
                            <React.Fragment key={index}>
                                <Controller
                                    control={control}
                                    name={`settings[${index}].name`}
                                    rules={{ validate: { isValid: () => !isFieldEmpty(index, "name"), isRepeate: () => !areNamesRepeated(index) } }}
                                    render={({ onChange }) => (
                                        <TextField
                                            error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined' || setting.name === 'price'}
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
                                    rules={{ validate: { isValid: () => !isFieldEmpty(index, "value") } }}
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
                                            disabled={isSettingAtTheEndOfAnyRelation(setting.name)}
                                        />
                                    )}
                                />

                                <ButtonGroup>
                                    {settingsList.length - 1 === index &&
                                        <IconButton onClick={handleAddSetting} aria-label="add" color="primary">
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
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Listo</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </>
    );
}

export default EditArtifact;