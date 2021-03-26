import * as React from 'react';
import CustomForm from './NewArtifactDialogComponents/CustomForm';
import AwsForm from './NewArtifactDialogComponents/AwsForm';
import ProviderForm from './NewArtifactDialogComponents/ProviderForm';
import SettingsCustomForm from './NewArtifactDialogComponents/SettingsCustomForm';
import SettingsAwsForm from './NewArtifactDialogComponents/SettingsAwsForm';
import AwsPricingDimension from './NewArtifactDialogComponents/AwsPricingDimension';
import Confirmation from './NewArtifactDialogComponents/Confirmation';
import Setting from '../interfaces/Setting';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';
import PricingTerm from '../interfaces/PricingTerm';
import { PROVIDERS } from '../Constants';
import ArtifactTypeService from '../services/ArtifactTypeService';
import ArtifactType from '../interfaces/ArtifactType';

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function , setSnackbarSettings: Function }) => {

    const [step, setStep] = React.useState<number>(1);
    const [artifactTypeId, setArtifactTypeId] = React.useState<number | null>(null);
    const [name, setName] = React.useState<string | null>("");
    const [provider, setProvider] = React.useState<string | null>("");
    const [settingsList, setSettingsList] = React.useState<Setting[]>([{ name: "", value: "" }]);
    const [settings, setSettings] = React.useState<object>({});
    const [settingTypes, setSettingTypes] = React.useState<{ [key: string]: string }>({});
    const [settingsMap, setSettingsMap] = React.useState<{ [key: string]: number[] }>({});
    const [awsSettingsList, setAwsSettingsList] = React.useState<KeyValueStringPair[]>([]);
    const [awsPricingTerm, setAwsPricingTerm] = React.useState<PricingTerm | null>(null);

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
        setSettingTypes({});
        props.closeNewArtifactDialog()
    }

    const getArtifactTypes = async (id: number) => {
        try {
            const response = await ArtifactTypeService.getAllByProvider(PROVIDERS[id]);
            if (response.status === 200) {
                return response.data as ArtifactType[];
            } else {
                props.setSnackbarSettings({ message: "Hubo un problema al cargar los tipos de artefactos.", severity: "error" });
                props.setOpenSnackbar(true);
                return [];
            }
        } catch {
            props.setSnackbarSettings({ message: "Hubo un problema al cargar los tipos de artefactos.", severity: "error" });
            props.setOpenSnackbar(true);
            return [];
        }
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
                        getArtifactTypes={getArtifactTypes}
                    />
                );
            }
            else {
                return (
                    <AwsForm
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        projectId={props.projectId}
                        setArtifactTypeId={setArtifactTypeId}
                        updateList={props.updateList}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        setName={setName}
                        getArtifactTypes={getArtifactTypes}
                    />
                );
            }
            

        case 3:
            if (provider === "Custom") {
                return (
                    <SettingsCustomForm
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        updateList={props.updateList}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        settingsList={settingsList}
                        setSettingsList={setSettingsList}
                        settingsMap={settingsMap}
                        setSettingsMap={setSettingsMap}
                        setSettings={setSettings}
                        settingTypes={settingTypes}
                        setSettingTypes={setSettingTypes}
                    />
                );
            }
            else {
                return (
                    <SettingsAwsForm
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        name={name}
                        updateList={props.updateList}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        setAwsSettingsList={setAwsSettingsList}
                        setSettingsList={setSettingsList}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                    />
                );
            }

        case 4:
            if (provider === "AWS") {
                return (
                    <AwsPricingDimension
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        name={name}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        settingsList={settingsList}
                        setSettingsList={setSettingsList}
                        settingsMap={settingsMap}
                        setSettingsMap={setSettingsMap}
                        awsSettingsList={awsSettingsList}
                        settings={settings}
                        setSettings={setSettings}
                        setAwsPricingTerm={setAwsPricingTerm}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                    />
                );
            }
            else {
                return (
                    <Confirmation
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        provider={provider}
                        name={name}
                        projectId={props.projectId}
                        artifactTypeId={artifactTypeId}
                        updateList={props.updateList}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                        handlePreviousStep={handlePreviousStep}
                        settingsList={settingsList}
                        settings={settings}
                        awsPricingTerm={awsPricingTerm}
                        settingTypes={settingTypes}
                    />
                );
            }

        case 5:
            if (provider === "AWS") {
                return (
                    <Confirmation
                        showNewArtifactDialog={props.showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        provider={provider}
                        name={name}
                        projectId={props.projectId}
                        artifactTypeId={artifactTypeId}
                        updateList={props.updateList}
                        setOpenSnackbar={props.setOpenSnackbar}
                        setSnackbarSettings={props.setSnackbarSettings}
                        handlePreviousStep={handlePreviousStep}
                        settingsList={settingsList}
                        settings={settings}
                        awsPricingTerm={awsPricingTerm}
                        settingTypes={settingTypes}
                    />
                );
            }
            else {
                return null;
            }

        default:
            return null;
    }
}

export default NewArtifactDialog;
