﻿import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import ProviderArtifactSetting from '../../interfaces/ProviderArtifactSetting';
import Setting from '../../interfaces/Setting';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import AwsArtifactService from '../../services/AwsArtifactService';
import { withTheme } from "@rjsf/core";
import { Theme as MaterialUITheme } from "@rjsf/material-ui";


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
    const Form = withTheme(MaterialUITheme);
    const { handleSubmit, errors, setError, clearErrors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, name } = props;

    //Hook for save the user's settings input
    const [loading, setLoading] = React.useState<Boolean>(true);
    const [awsSettingsList, setAwsSettingsList] = React.useState<KeyValueStringPair[]>([]);

    const [schema, setSchema] = React.useState<object>({});

    React.useEffect(() => {
        getRequireFields();
    }, [name]);

    const getRequireFields = async () => {
        if (name) {
            let statusOk = 200;
            try {
                setLoading(true);
                const response = await AwsArtifactService.GetRequiredFieldsAsync(name);
                if (response.status === statusOk) {
                    setSchema(response.data);
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
    const handleConfirm = async (data: any) => {
        /*let awsSettingsListFiltered = awsSettingsList.filter(awsSetting => awsSetting !== undefined && awsSetting !== null && awsSetting.value !== "");
        console.log(awsSettingsListFiltered);
        props.setSettingsList(awsSettingsListToSettingsList(awsSettingsListFiltered));
        props.setAwsSettingsList(awsSettingsListFiltered);
        props.handleNextStep();*/
        console.log(data.formData);
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
            <DialogContent>
                {loading ?
                    <div className={classes.circularProgress}>
                        <CircularProgress color="inherit" size={30} />
                    </div> :
                    <Form
                        id="my-form"
                        schema={schema}
                        onSubmit={handleConfirm}
                    >
                        <div>
                            <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                            <Button size="small" color="primary" type="submit">Siguiente</Button>
                            <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                        </div>
                    </Form>
                }
            </DialogContent>
        </Dialog>
    );
}

export default SettingsAwsForm;