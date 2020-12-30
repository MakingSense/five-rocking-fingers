import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import ProviderArtifactSetting from '../../interfaces/ProviderArtifactSetting';
import Setting from '../../interfaces/Setting';
import AwsArtifactSetting from '../../interfaces/AwsArtifactSetting';
import ArtifactService from '../../services/ArtifactService';
import AwsArtifactService from '../../services/AwsArtifactService';
import { Settings } from '@material-ui/icons';


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
        }
    }),
);

const SettingsAwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, provider: string | null, name: string | null, projectId: number, artifactTypeId: number | null, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function, setAwsSettingsList: Function, setSettingsList: Function }) => {

    const classes = useStyles();
    const { handleSubmit, register, errors, setError, clearErrors, control, getValues } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, provider, name, projectId, artifactTypeId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    //Hook for save the user's settings input
    const [awsSettingsValuesList, setAwsSettingsValuesList] = React.useState<ProviderArtifactSetting[]>([]);
    const [loading, setLoading] = React.useState<Boolean>(true);
    const [awsSettingsList, setAwsSettingsList] = React.useState<AwsArtifactSetting[]>([]);

    React.useEffect(() => {
        getArtifactSettings();
    }, [name]);

    const getArtifactSettings = async () => {
        if (name) {
            setLoading(true);
            const response = await AwsArtifactService.GetAttibutesAsync(name);
            setAwsSettingsValuesList(response.data);
            setLoading(false);
        }
    }

    //Create the artifact after submit
    const handleConfirm = async () => {
        let awsSettingsListFiltered = awsSettingsList.filter(awsSetting => awsSetting !== undefined && awsSetting !== null);
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

    const awsSettingsListToSettingsList = (awsSettingsListFiltered: AwsArtifactSetting[]) => {
        let settingsList: Setting[] = []
        awsSettingsListFiltered.forEach(awsSetting => {
            let setting: Setting = { name: awsSetting.key, value: awsSetting.value }
            settingsList.push(setting);
        });
        return settingsList;
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos custom</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto custom y el valor que tomarán esas propiedades
                </Typography>
                {loading ?
                    <div className={classes.circularProgress}>
                        <CircularProgress color="inherit" size={30} />
                    </div> :
                    <form className={classes.container}>
                        {awsSettingsValuesList.map((awsSetting: ProviderArtifactSetting, index: number) => {
                            console.log(awsSetting);
                            return (
                                <FormControl key={index} className={classes.formControl} error={Boolean(errors.setting)}>
                                    <InputLabel htmlFor={`provider-select-label${index}`}>{awsSetting.name.value}</InputLabel>
                                    <Controller
                                        render={({ onChange }) => (
                                            <Select
                                                labelId={`provider-select-label${index}`}
                                                inputProps={{
                                                    name: `Valor de la setting${index}`,
                                                    id: 'provider-select'
                                                }}
                                                onChange={event => handleInputChange(awsSetting.name.key, event.target.value as string, index)}
                                            >
                                                <MenuItem value="">
                                                    None
                                                </MenuItem>
                                                {awsSetting.values.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
                                            </Select>
                                        )}
                                        name={`setting[${index}]`}
                                        control={control}
                                        defaultValue=""
                                        value={awsSettingsList[index] !== undefined ? awsSettingsList[index] : ''}
                                    />
                                    <FormHelperText>Requerido*</FormHelperText>
                                </FormControl>
                            );
                        })}
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