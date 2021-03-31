import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, IconButton, ButtonGroup, Select, MenuItem, Grid, FormGroup, FormHelperText, FormControl, InputLabel } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm, Controller } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';
import Setting from '../../interfaces/Setting';
import { SETTINGTYPES, CUSTOM_REQUIRED_FIELDS } from '../../Constants';

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
        },
        select: {
            marginBottom: 24,
            marginRight: 0,
            marginLeft: 0,
            marginTop: 12,
            "& .MuiSelect-outlined": {
                paddingBottom: 13
              }
        },
        error: {
            color: 'red'
        }
    }),
);

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function, settingsList: Setting[], setSettingsList: Function, settingsMap: { [key: string]: number[] }, setSettingsMap: Function, setSettings: Function, settingTypes: { [key: string]: string }, setSettingTypes: Function }) => {

    const classes = useStyles();
    const { handleSubmit, errors, setError, clearErrors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog,setSettingTypes,settingTypes } = props;
    //Hook for save the user's settings input
    const [settingsList, setSettingsList] = React.useState<Setting[]>(props.settingsList);
    //Hook for saving the numbers of times a setting's name input is repeated
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>(props.settingsMap);
    const [price, setPrice] = React.useState<number>(() => {
        let index = settingsList.findIndex(s => s.name === CUSTOM_REQUIRED_FIELDS);
        if (index != -1) {
            let price = settingsList[index];
            settingsList.splice(index, 1);
            props.setSettingsList(settingsList);
            return parseFloat(price.value);
        }
        return 0;
    });

    React.useEffect(() => {
        setNameSettingsErrors();
    }, [settingsMap]);

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (!settingsList.find(s => s.name === CUSTOM_REQUIRED_FIELDS)) {
            settingsList.unshift({ name: CUSTOM_REQUIRED_FIELDS, value: price.toString() });
        }
        props.setSettingsList(settingsList);
        props.setSettingsMap(settingsMap);
        props.setSettings({ settings: createSettingsObject() });
        props.setSettingTypes(settingTypes);
        props.handleNextStep();
        
    }
   
    const handleChangePrice = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        setPrice(parseInt(event.target.value, 10));
    }

    const isPriceValid = () => {
        if (price >= 0) return true
        return false
    }

    const isValidNumber = (index: number):boolean => {
        return !isNaN(Number(settingsList[index].value))?  Math.sign(Number(settingsList[index].value)) >= 0 ? true:false:false;
    }

    const isNumberSameType = (index: number) => {
        const numberType = settingTypes[settingsList[index].name];
        if (numberType === undefined) return true;
        const settingValue = parseFloat(settingsList[index].value);
        if (numberType === SETTINGTYPES[1]) {
            return Boolean( settingValue % 1 === 0);
        }
        else if (numberType === SETTINGTYPES[0]) {
            return Boolean( settingValue % 1 !== 0 || settingValue % 1 === 0);
        }
    }

    //Handle changes in the inputs fields
    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
        let { name, value } = event.target;
        name = name.split(".")[1];
        value = value.replace(/\s/g, '_').trim();
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
        else if (mapList[settingName] === undefined) return;
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
        for (let [, array] of Object.entries(settingsMap)) {
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
        if (key != null && (settingsMap[key].length > 1 || key === CUSTOM_REQUIRED_FIELDS)) {
            return true;
        }
        return false;
    }

    const isFieldEmpty = (index: number, field: string, select: boolean) => {
        if (!select) {
            return (settingsList[index][field].trim() === "");
        }
        else{
            return (settingTypes[settingsList[index].name] === undefined || settingTypes[settingsList[index].name].trim() === "");
        }
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const handleAddSetting = () => {
        setSettingsList([...settingsList, { name: "", value: "0" }]);
    }

    const handleDeleteSetting = (index: number) => {
        let listTypes = {...settingTypes};
        delete listTypes[settingsList[index].name];
        setSettingTypes(listTypes);
        let listSettings = [...settingsList];
        listSettings.splice(index, 1);
        setSettingsList(listSettings);

        let mapList = { ...settingsMap };
        let key = searchIndexInObject(mapList, index);
        if (key != null) {
            deleteIndexFromObject(mapList, index, key);
            updateSettingsMap(mapList, index);

        }
        setSettingsMap(mapList);
    }

    const updateSettingsMap = (object: { [key: string]: number[] }, index: number) => {
        for (let [, array] of Object.entries(object)) {
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
            settingsObject[settingsList[i].name.trim().replace(/\s+/g, '')] = settingsList[i].value;
        }

        return settingsObject;
    }

    const handleTypeChange = (event: React.ChangeEvent<{ value: unknown }>, index: number) => {
        if (areNamesRepeated(index)) return;
        let auxSettingTypes = {...settingTypes};
        auxSettingTypes[settingsList[index].name] = event.target.value as string;
        setSettingTypes(auxSettingTypes);
    }

    const goPrevStep = () => {
        settingsList.unshift({ name: CUSTOM_REQUIRED_FIELDS, value: price.toString() });
        props.setSettingsList(settingsList);
        props.setSettingsMap(settingsMap);
        props.setSettingTypes(settingTypes);
        props.handlePreviousStep();
    }
    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle >Formulario de artefactos custom</DialogTitle>
                <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto custom y el valor que tomarán esas propiedades.
                    Recuerde que no se aceptan nombres con espacios.
                </Typography>
                <Typography gutterBottom>
                    Recuerde que no se aceptan nombres con espacios.
                </Typography>
                </DialogContent>
                <form className={classes.container}>
                <DialogContent>
                    <TextField
                        disabled
                        label="Nombre de la propiedad"
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
                        defaultValue={price}
                        render={({ onChange }) => (
                            <TextField
                                error={!isPriceValid()}
                                label="Valor"
                                helperText="Requerido*"
                                variant="outlined"
                                value={price}
                                className={classes.inputF}
                                onChange={event => { handleChangePrice(event); onChange(event); }}
                                autoComplete='off'
                                type="number"
                            />
                        )}
                    />
                </DialogContent>
                <DialogTitle style={{marginBottom: '-14px', paddingTop:1}} >Propiedades: </DialogTitle>
                {settingsList.map((setting: Setting, index: number) => {
                    return (
                        <DialogContent key={index}>
                            <FormGroup row>
                            <Grid container>
                                <Grid item xs={5} zeroMinWidth spacing={0} style={{flexBasis:'31%'}}>
                                    <Controller
                                        control={control}
                                        name={`settings[${index}].name`}
                                        key={index}
                                        rules={{ validate: { isValid: () => !isFieldEmpty(index, "name", false), isRepeate: () => !areNamesRepeated(index) } }}
                                        defaultValue={setting.name}
                                        render={({ onChange }) => (
                                            <TextField
                                                error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined' || setting.name === 'price'}
                                                id={`name[${index}]`}
                                                name={`settings[${index}].name`}
                                                label="Nombre"
                                                helperText={areNamesRepeated(index) ? "Los nombres no pueden repetirse" : "Requerido*"}
                                                variant="outlined"
                                                value={setting.name}
                                                className={classes.inputF}
                                                onChange={event => {handleInputChange(event, index); onChange(event); }}
                                                autoComplete='off'
                                            />
                                        )}
                                    />
                                </Grid>

                                <Grid item xs={3} zeroMinWidth spacing={0}>
                                    <Controller
                                        control={control}
                                        name={`settings[${index}].value`}
                                        key={index}
                                        rules={{ validate: { isValid: () => isValidNumber(index), isEmpty: () => !isFieldEmpty(index, "value", false) } }}
                                        defaultValue={setting.value}
                                        render={({ onChange }) => (
                                            <TextField
                                                error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.value !== 'undefined' || !isValidNumber(index) || !isNumberSameType(index)}
                                                id={`value[${index}]`}
                                                name={`settings[${index}].value`}
                                                label="Valor"
                                                helperText={!isValidNumber(index)? "Solo puede contener numeros positivos":"Requerido*"}
                                                variant="outlined"
                                                value={setting.value}
                                                className={classes.inputF}
                                                onChange={event => { handleInputChange(event, index); onChange(event); }}
                                                autoComplete='off'
                                                type="number"
                                            />
                                        )}
                                    />
                                    </Grid>
                                    <FormControl variant="outlined" className={classes.select} error={errors.relationalSettings && errors.relationalSettings[index] && typeof errors.relationalSettings[index] !== 'undefined' || !isNumberSameType(index)}>
                                        <InputLabel id="settingTypeLabel">{!isNumberSameType(index) ? <Typography gutterBottom className={classes.error}>Tipo</Typography> : "Tipo"}</InputLabel>
                                        <Controller
                                            control={control}
                                            name={`relationalSettings[${index}].type`}
                                            key={index}
                                            error={!isNumberSameType(index)}
                                            rules={{ validate: { isValid: () => isValidNumber(index), isEmpty: () => !isFieldEmpty(index, "value", true) } }}
                                            defaultValue={settingTypes[settingsList[index].name] === undefined ? '' : settingTypes[settingsList[index].name]}
                                            render={({ onChange }) => (
                                                <Select
                                                    style={{ paddingTop: 5 }}
                                                    labelId="settingTypeLabel"
                                                    id="settingTypeLabel"
                                                    name={`types[${index}]`}
                                                    label="Tipo"
                                                    autoWidth
                                                    value={settingTypes[settingsList[index].name] === undefined ? '' : settingTypes[settingsList[index].name]}
                                                    onChange={event => { handleTypeChange(event, index); onChange(event); }}
                                                >
                                                    <MenuItem value="">
                                                        <em>None</em>
                                                    </MenuItem>
                                                    {SETTINGTYPES.map((value: string) => {
                                                        return <MenuItem value={value}><em>{`${value.charAt(0).toUpperCase()}${value.slice(1).replace(/([a-z])([A-Z])/g, '$1 $2')}`}</em></MenuItem>
                                                    })}
                                                </Select>
                                            )}
                                        />
                                        <FormHelperText>{!isNumberSameType(index) ? <Typography gutterBottom className={classes.error}>Tipo invalido</Typography> : "Requerido*"}</FormHelperText>
                                    </FormControl>
                                <ButtonGroup size='small' key={index}>
                                    {settingsList.length - 1 === index &&
                                        <IconButton onClick={handleAddSetting} aria-label="add" color="primary">
                                            <AddCircleIcon />
                                        </IconButton>
                                    }

                                    {settingsList.length !== 1 &&
                                        <IconButton onClick={() => handleDeleteSetting(index)} aria-label="delete" color="secondary">
                                            <DeleteIcon />
                                        </IconButton>
                                    }
                                </ButtonGroup>
                                </Grid>
                            </FormGroup>
                        </DialogContent>
                    );
                })}
            </form>
            <DialogActions>
                <Button size="small" color="primary" onClick={() => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsCustomForm;