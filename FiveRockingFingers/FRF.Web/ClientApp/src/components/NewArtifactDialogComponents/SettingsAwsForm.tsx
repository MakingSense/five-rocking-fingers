import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import ProviderArtifactSetting from '../../interfaces/ProviderArtifactSetting';
import Setting from '../../interfaces/Setting';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import AwsArtifactService from '../../services/AwsArtifactService';


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
        circularProgress: {
            width: '30%',
            margin: 'auto'
        },
        error: {
            color: 'red'
        }
    }),
);

const SettingsAwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, name: string | null, updateList: Function, handleNextStep: Function, handlePreviousStep: Function, setAwsSettingsList: Function, setSettingsList: Function, setSnackbarSettings: Function, setOpenSnackbar: Function }) => {

    const classes = useStyles();
    const { handleSubmit, errors, setError, clearErrors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, name } = props;

    //Hook for save the user's settings input
    const [awsSettingsValuesList, setAwsSettingsValuesList] = React.useState<ProviderArtifactSetting[]>([]);
    const [loading, setLoading] = React.useState<Boolean>(true);
    const [awsSettingsList, setAwsSettingsList] = React.useState<KeyValueStringPair[]>([]);

    React.useEffect(() => {
        getArtifactSettings();
    }, [name]);

    const getArtifactSettings = async () => {
        if (name) {
            let statusOk = 200;
            try {
                setLoading(true);
                const response = await AwsArtifactService.GetAttibutesAsync(name);
                if (response.status === statusOk) {
                    setAwsSettingsValuesList(response.data);
                    setLoading(false);
                }
            }
            catch (error) {
                props.setSnackbarSettings({ message: "Fuera de servicio. Por favor intente mas tarde.", severity: "error" });
                props.setOpenSnackbar(true);
                closeNewArtifactDialog();
            }           
        }
    }

    //Create the artifact after submit
    const handleConfirm = async () => {
        let awsSettingsListFiltered = awsSettingsList.filter(awsSetting => awsSetting !== undefined && awsSetting !== null && awsSetting.value !== "");
        console.log(awsSettingsListFiltered);
        props.setSettingsList(awsSettingsListToSettingsList(awsSettingsListFiltered));
        props.setAwsSettingsList(awsSettingsListFiltered);
        props.handleNextStep();
    }

    //Handle changes in the inputs fields
    const handleInputChange = (artifactSettingKey: string, artifactSettingValue: string, index: number) => {
        let awsSettingListCopy = [...awsSettingsList];
        awsSettingListCopy[index] = { key: artifactSettingKey, value: artifactSettingValue };
        setAwsSettingsList(awsSettingListCopy);
    }

    const awsSettingsListToSettingsList = (awsSettingsListFiltered: KeyValueStringPair[]) => {
        let settingsList: Setting[] = []
        awsSettingsListFiltered.forEach(awsSetting => {
            let setting: Setting = { name: awsSetting.key, value: awsSetting.value }
            settingsList.push(setting);
        });
        return settingsList;
    }

    const isOneValueCompleted = () => {
        let awsSettingsListFiltered = awsSettingsList.filter(awsSetting => awsSetting !== undefined && awsSetting !== null && awsSetting.value !== "");
        if (awsSettingsListFiltered.length !== 0) {
            return true;
        }
        return false;
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos AWS</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto AWS
                </Typography>
                {loading ?
                    <div className={classes.circularProgress}>
                        <CircularProgress color="inherit" size={30} />
                    </div> :
                    <form className={classes.container}>
                        {awsSettingsValuesList.map((awsSetting: ProviderArtifactSetting, index: number) => {
                            return (
                                <FormControl key={index} className={classes.formControl}>
                                    <InputLabel htmlFor={`settings[${index}].name`}>{awsSetting.name.value}</InputLabel>
                                    <Controller
                                        render={({ onChange, value }) => (
                                            <Select
                                                labelId={`settings[${index}].name`}
                                                inputProps={{
                                                    name: `settings[${index}].name`,
                                                    id: `settings[${index}].name`
                                                }}
                                                onChange={event => handleInputChange(awsSetting.name.key, event.target.value as string, index)}
                                                name={`settings[${index}].name`}
                                                defaultValue={''}
                                                value={awsSettingsList[index] === undefined ? '' : awsSettingsList[index].value}
                                                error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined'}
                                            >
                                                <MenuItem value={''}>
                                                    None
                                                </MenuItem>
                                                {awsSetting.values.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
                                            </Select>
                                        )}
                                        name={`settings[${index}].name`}
                                        rules={{ validate: { isValid: () => isOneValueCompleted() } }}
                                        control={control}
                                        defaultValue={''}
                                        value={awsSettingsList[index] === undefined ? '' : awsSettingsList[index].value}
                                        error={errors.settings && errors.settings[index] && typeof errors.settings[index]?.name !== 'undefined'}
                                    />
                                </FormControl>
                            );
                        })}
                        {errors.settings ?
                            <Typography gutterBottom className={classes.error}>
                                Al menos un campo debe ser completado
                            </Typography> : null
                        }
                    </form>
                }
                
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsAwsForm;