import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, IconButton, ButtonGroup, Select, MenuItem, Grid, FormGroup, OutlinedInput, FormHelperText, FormControl, InputLabel } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm, Controller } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';
import Setting from '../../interfaces/Setting';
import { SETTINGTYPES } from '../../Constants';

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
    }),
);

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function, settingsList: Setting[], setSettingsList: Function, settingsMap: { [key: string]: number[] }, setSettingsMap: Function, setSettings: Function, settingTypes: { [key: string]: string }, setSettingTypes: Function }) => {

    const classes = useStyles();
    const { handleSubmit, errors, setError, clearErrors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;
    //Hook for save the user's settings input
    const [settingsList, setSettingsList] = React.useState<Setting[]>(props.settingsList);
    //Hook for saving the numbers of times a setting's name input is repeated
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>(props.settingsMap);
    const [price, setPrice] = React.useState(() => {
        let index = settingsList.findIndex(s => s.name === 'price');
        if (index != -1) {
            let price = settingsList[index];
            settingsList.splice(index, 1);
            props.setSettingsList(settingsList);
            return price.value;
        }
        return 0;
    });
    const [settingTypes, setSettingTypes] = React.useState<string[]>([]);
    const [isValid, setIsValid] = React.useState<boolean>(true);
    const [errorNumber, setErrorNumber] = React.useState(true);

    React.useEffect(() => {
        setNameSettingsErrors();
    }, [settingsMap]);

    const createSettingTypesList = () => {
        let settingsObject: { [key: string]: string } = {};
        for (let i = 1; i < settingsList.length; i++) {
            settingsObject[settingsList[i].name] = settingTypes[i-1];
        }
        return settingsObject;
    }

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (!settingsList.find(s => s.name === 'price')) {
            settingsList.unshift({ name: 'price', value: price.toString() });
        }
        if(!hasTypeSelected()){
            setIsValid(false);
            settingsList.shift();
            }
    
        else{
        props.setSettingsList(settingsList);
        props.setSettingsMap(settingsMap);
        props.setSettings({ settings: createSettingsObject() });
        props.setSettingTypes(createSettingTypesList());
        props.handleNextStep();
        }
    }
   
    const handleChangePrice = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        setPrice(parseInt(event.target.value, 10));
    }

    const isPriceValid = () => {
        if (price >= 0) return true
        return false
    }

    const isValidNumber = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number):boolean => {
        let { name, value} = event.target;
        if (name === `settings[${index}].value`){
            return !isNaN(Number(value))
        }
        return false;
    }

    //Handle changes in the inputs fields
    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
        let { name, value } = event.target;
        name = name.split(".")[1];
        if (name === 'name') {
            checkSettingName(value, index);
        }
       // else isValidNumber(event,index);
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
        closeNewArtifactDialog();
    }

    const handleAddSetting = () => {
        setSettingsList([...settingsList, { name: "", value: "" }]);
        setSettingTypes([...settingTypes, SETTINGTYPES[0]]);
    }

    const handleDeleteSetting = (index: number) => {
        const listSettings = [...settingsList];
        listSettings.splice(index, 1);
        setSettingsList(listSettings);
        const listTypes = [...settingTypes];
        listTypes.splice(index, 1);
        setSettingTypes(listTypes);

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

    const handleTypeChange = (event: React.ChangeEvent<{ value: unknown }>, index: number) => {
        let auxSettingTypes = [...settingTypes];
        auxSettingTypes[index] = event.target.value as string;
        setSettingTypes(auxSettingTypes);
    }

    const goPrevStep = () => {
        settingsList.unshift({ name: 'price', value: price.toString() });
        props.setSettingsList(settingsList);
        props.setSettingsMap(settingsMap);
        props.setSettingTypes(createSettingTypesList());
        props.handlePreviousStep();
    }

    const hasTypeSelected = () => {
        return Boolean(settingTypes.length === settingsList.length-1);
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle >Formulario de artefactos custom</DialogTitle>
                <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto custom y el valor que tomarán esas propiedades
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
                        render={({ onChange }) => (
                            <TextField
                                error={!isPriceValid()}
                                label="Valor"
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
                </DialogContent>
                <DialogTitle style={{marginBottom: '-14px', paddingTop:1}} >Propiedades: </DialogTitle>
                {settingsList.map((setting: Setting, index: number) => {
                    return (
                        <DialogContent>
                            <FormGroup row>
                            <Grid container spacing={0}>
                                <Grid item xs={5} zeroMinWidth spacing={0} style={{flexBasis:'31%'}}>
                                    <Controller
                                        control={control}
                                        name={`settings[${index}].name`}
                                        rules={{ validate: { isValid: () => !isFieldEmpty(index, "name"), isRepeate: () => !areNamesRepeated(index) } }}
                                        render={({ onChange }) => (
                                            <TextField
                                                error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined' || setting.name === 'price'}
                                                id={`name[${index}]`}
                                                name={`settings[${index}].name`}
                                                label="Nombre"
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
                                </Grid>

                                <Grid item xs={3} zeroMinWidth spacing={0}>
                                    <Controller
                                        control={control}
                                        name={`settings[${index}].value`}
                                        rules={{ validate: { isValid: () => !isFieldEmpty(index, "value") } }}
                                        render={({ onChange }) => (
                                            <TextField
                                                error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.value !== 'undefined' }
                                                id={`value[${index}]`}
                                                name={`settings[${index}].value`}
                                                label="Valor"
                                                helperText={"Requerido*"}
                                                variant="outlined"
                                                defaultValue={setting.value}
                                                value={setting.value}
                                                className={classes.inputF}
                                                onChange={event => { handleInputChange(event, index); onChange(event); }}
                                                autoComplete='off'
                                                type="number"
                                            />
                                        )}
                                    />
                                    </Grid>
                                    <FormControl variant="outlined" className={classes.select} error={!isValid}>
                                        <InputLabel id="settingTypeLabel">Tipo</InputLabel>
                                        <Controller
                                            control={control}
                                            name={`relationalSettings[${index}].type`}
                                            error={!isValid}
                                            render={({ onChange }) => (
                                                <Select
                                                    style={{ paddingTop: 5 }}
                                                    labelId="settingTypeLabel"
                                                    id="settingTypeLabel"
                                                    label="Tipo"
                                                    autoWidth
                                                    value={settingTypes[index]}
                                                    defaultValue={''}
                                                    onChange={event => { handleTypeChange(event, index); onChange(event); }}
                                                >
                                                    {SETTINGTYPES.map((value: string) => {
                                                        return <MenuItem value={value}>{`${value.charAt(0).toUpperCase()}${value.slice(1).replace(/([a-z])([A-Z])/g, '$1 $2')}`}</MenuItem>
                                                    })}
                                                </Select>
                                            )}
                                        />
                                        <FormHelperText>Requerido*</FormHelperText>
                                    </FormControl>
                                <Grid item>
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
                                </Grid>
                                </Grid>
                            </FormGroup>
                        </DialogContent>
                    );
                })}
            </form>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsCustomForm;