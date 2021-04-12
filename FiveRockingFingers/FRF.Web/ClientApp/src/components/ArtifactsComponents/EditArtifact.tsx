import { Button, DialogActions, DialogContent, DialogTitle, TextField } from '@material-ui/core';
import { createMuiTheme, createStyles, makeStyles, Theme, ThemeProvider } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm, Controller, FormProvider } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Setting from '../../interfaces/Setting';
import Artifact from '../../interfaces/Artifact';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import { CUSTOM_REQUIRED_FIELD } from '../../Constants';
import { useArtifact } from '../../commons/useArtifact';
import SettingsEntries from '../../commons/SettingsEntries';

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
        dialog: {
        "& .MuiDialogContent":{
            padding:0
        }
    },
    error: {
        color: 'red'
    }
    }),
);
const theme = createMuiTheme({
    overrides: {
        MuiDialogContent: {
        root: {
            padding:"0px 5px 0px 5px"
        },
      },
    },
  });

const EditArtifact = (props: {
    artifactToEdit: Artifact,
    closeEditArtifactDialog: Function,
    setIsArtifactEdited: Function,
    setArtifactEdited: Function,
    setNamesOfSettingsChanged: Function,
    artifactsRelations: ArtifactRelation[], 
    settingTypes: { [key: string]: string }, 
    setSettingTypes: Function  }) => {

    const classes = useStyles();
    const methods = useForm();
    const { register, handleSubmit, errors, control } = methods;
    const { settingsList, setSettingsList, price, setPrice, setSettingsMap, createSettingsObject, setSettings } = useArtifact();
    const { artifactToEdit, artifactsRelations, closeEditArtifactDialog, settingTypes, setSettingTypes } = props;

    

    const [artifactName, setArtifactName] = React.useState(props.artifactToEdit.name);

    const createSettingsMapFromArtifact = () => {
        let settingsMapFromArtifact: { [key: string]: number[] } = {};
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            if (key !== CUSTOM_REQUIRED_FIELD) {
                settingsMapFromArtifact[key] = [index];
            }            
        });
        return settingsMapFromArtifact;
    }

    React.useEffect(() => {
        setSettingsList(createSettingsListFromArtifact());
    }, []);

    React.useEffect(() => {
        setSettingsMap(createSettingsMapFromArtifact());
    }, []);

    const createSettingsListFromArtifact = () => {
        let settingsListFromArtifact: Setting[] = [];
        if (props.artifactToEdit.id === 0) {
            return settingsListFromArtifact;
        }
        Object.entries(props.artifactToEdit.settings).forEach(([key, value], index) => {
            let settingFromArtifact: Setting = Object.assign({ name: key, value: value });
            settingsListFromArtifact.push(settingFromArtifact);
        });

        let price = 0;

        let index = settingsListFromArtifact.findIndex(s => s.name === CUSTOM_REQUIRED_FIELD);

        if (index != -1) {
            let priceFromList = settingsListFromArtifact[index];
            settingsListFromArtifact.splice(index, 1);
            price = parseFloat(priceFromList.value);
        }
        setPrice(price);
        return settingsListFromArtifact;
    }

    const createSettingTypesList = () => {
        let settingsObject: { [key: string]: string } = {};
        settingsObject[CUSTOM_REQUIRED_FIELD] = 'decimal';
        for (let i = 1; i < settingsList.length; i++) {
            settingsObject[settingsList[i].name] = settingTypes[settingsList[i].name];
        }
        return settingsObject;
    }

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (!settingsList.find(s => s.name === CUSTOM_REQUIRED_FIELD)) {
            settingsList.unshift({ name: CUSTOM_REQUIRED_FIELD, value: price.toString() });
            }
        let artifactEdited : Artifact = Object.create(props.artifactToEdit);
        artifactEdited.name = artifactName;
        artifactEdited.settings = createSettingsObject();
        artifactEdited.relationalFields = createSettingTypesList();
        props.setArtifactEdited(artifactEdited);
        props.setNamesOfSettingsChanged(getNamesOfSettingsChanged());
        props.setIsArtifactEdited(true);
    }

    const getNamesOfSettingsChanged = () => {
        let namesOfSettingsChanged : string[] = [];
        let originalSettingsList = createSettingsListFromArtifact();
        let index = originalSettingsList.findIndex(s => s.name === CUSTOM_REQUIRED_FIELD);
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

    const handleCancel = () => {
        closeEditArtifactDialog();
    }    

    return (
        <FormProvider {...methods}>
            <ThemeProvider theme={theme}>
                <DialogTitle >Formulario de actualización de artefactos custom</DialogTitle>
                <DialogContent>
                    <Controller
                        control={control}
                        name={'name'}
                        rules={{ validate: { isValid: value => value.trim() != "" } }}
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
                </DialogContent>
                <SettingsEntries
                    artifactToEdit={artifactToEdit}
                    artifactsRelations={artifactsRelations}
                />
                <DialogActions>
                    <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Listo</Button>
                    <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                </DialogActions>
            </ThemeProvider>
        </FormProvider>        
    );
}

export default EditArtifact;