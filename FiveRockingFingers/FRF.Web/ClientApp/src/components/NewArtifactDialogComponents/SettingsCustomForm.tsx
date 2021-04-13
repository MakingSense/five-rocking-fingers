import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import * as React from 'react';
import { useForm, FormProvider } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import { CUSTOM_REQUIRED_FIELD } from '../../Constants';
import { useArtifact } from '../../commons/useArtifact';
import SettingsEntries from '../../commons/SettingsEntries';

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function }) => {

    const { settingsList, setSettingsList, price, setPrice, settingsMap, setSettingsMap, createSettingsObject, settingTypes, setSettingTypes, setSettings } = useArtifact();
    const methods = useForm();
    const { handleSubmit } = methods;
    const { showNewArtifactDialog, closeNewArtifactDialog } = props;

    //Create the artifact after submit
    const handleConfirm = async () => {
        if (!settingsList.find(s => s.name === CUSTOM_REQUIRED_FIELD)) {
            settingsList.unshift({ name: CUSTOM_REQUIRED_FIELD, value: price.toString() });
        }
        setSettingsList(settingsList);
        setSettingsMap(settingsMap);
        setSettings({ settings: createSettingsObject() });
        setSettingTypes(settingTypes);
        props.handleNextStep();        
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        settingsList.unshift({ name: CUSTOM_REQUIRED_FIELD, value: price.toString() });
        setSettingsList(settingsList);
        setSettingsMap(settingsMap);
        setSettingTypes(settingTypes);
        props.handlePreviousStep();
    }

    React.useEffect(() => {
        let price = 0;
        let settingsListAux = [...settingsList];
        let index = settingsListAux.findIndex(s => s.name === CUSTOM_REQUIRED_FIELD);
        if (index != -1) {

            let priceFromList = settingsListAux[index];
            settingsListAux.splice(index, 1);
            price = parseFloat(priceFromList.value);
        }
        setPrice(price);
        setSettingsList(settingsListAux);
    }, []);

    return (
        <FormProvider {...methods}>
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
                <SettingsEntries />
                <DialogActions>
                    <Button size="small" color="primary" onClick={() => goPrevStep()}>Atrás</Button>
                    <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                    <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                </DialogActions>
            </Dialog>
        </FormProvider>
    );
}

export default SettingsCustomForm;