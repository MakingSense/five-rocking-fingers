import * as React from 'react';
import CustomForm from './NewArtifactDialogComponents/CustomForm';
import AwsForm from './NewArtifactDialogComponents/AwsForm';
import ProviderForm from './NewArtifactDialogComponents/ProviderForm';
import SettingsCustomForm from './NewArtifactDialogComponents/SettingsCustomForm';
import Setting from '../interfaces/Setting';

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function , setSnackbarSettings: Function }) => {

    const [step, setStep] = React.useState<number>(1);
    const [artifactTypeId, setArtifactTypeId] = React.useState<number | null>(null);
    const [name, setName] = React.useState<string | null>(null);
    const [provider, setProvider] = React.useState<string | null>(null);
    const [settingsList, setSettingsList] = React.useState<Setting[]>([{ name: "", value: "" }]);
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>({});

    React.useEffect(() => {
    }, [step]);

    const handleNextStep = () => {
        setStep(step + 1);
    }

    const handlePreviousStep = () => {
        setStep(step - 1);
    }

    const handleCancel = () => {
        setStep(1);
        setArtifactTypeId(null);
        setName(null);
        setProvider(null);
        setSettingsList([{ name: "", value: "" }]);
        setSettingsMap({});
        props.closeNewArtifactDialog()
    }

    switch (step) {
        case 1:
            return (
                <ProviderForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={handleCancel}
                    handleNextStep={handleNextStep}
                    setProvider={setProvider}
                    provider={provider}
                />
            );

        case 2:
            if (provider === "Custom") {
                return (
                    <CustomForm
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        setName={setName}
                        setArtifactTypeId={setArtifactTypeId}
                        name={name}
                        artifactTypeId={artifactTypeId}
                    />
                );
            }
            else {
                return (
                    <AwsForm
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        projectId={props.projectId}
                        updateList={props.updateList}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                        handlePreviousStep={handlePreviousStep}
                    />
                );
            }
            

        case 3:
            return (
                <SettingsCustomForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={handleCancel}
                    provider={provider}
                    name={name}
                    projectId={props.projectId}
                    artifactTypeId={artifactTypeId}
                    updateList={props.updateList}
                    setOpenSnackbar={props.setOpenSnackbar}
                    setSnackbarSettings={props.setSnackbarSettings}
                    handleNextStep={handleNextStep}
                    handlePreviousStep={handlePreviousStep}
                    settingsList={settingsList}
                    setSettingsList={setSettingsList}
                    settingsMap={settingsMap}
                    setSettingsMap={setSettingsMap}
                />
            );

        default:
            return null;
    }
}

export default NewArtifactDialog;
