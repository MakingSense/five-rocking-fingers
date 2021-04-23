import * as React from 'react';
import CustomForm from './NewArtifactDialogComponents/CustomForm';
import AwsForm from './NewArtifactDialogComponents/AwsForm';
import ProviderForm from './NewArtifactDialogComponents/ProviderForm';
import SettingsCustomForm from './NewArtifactDialogComponents/SettingsCustomForm';
import SettingsAwsForm from './NewArtifactDialogComponents/SettingsAwsForm';
import AwsPricingDimension from './NewArtifactDialogComponents/AwsPricingDimension';
import Confirmation from './NewArtifactDialogComponents/Confirmation';
import { PROVIDERS } from '../Constants';
import ArtifactTypeService from '../services/ArtifactTypeService';
import ArtifactType from '../interfaces/ArtifactType';
import { useArtifact } from './../commons/useArtifact';
import { handleErrorMessage } from '../commons/Helpers';

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function , setSnackbarSettings: Function }) => {

    const [step, setStep] = React.useState<number>(1);

    const { resetStateOnClose, provider } = useArtifact();

    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

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
        resetStateOnClose();
        closeNewArtifactDialog()
    }

    const getArtifactTypes = async (id: number) => {
        try {
            const response = await ArtifactTypeService.getAllByProvider(PROVIDERS[id]);
            if (response.status === 200) {
                return response.data as ArtifactType[];
            } else {
                handleErrorMessage(
                    response.data,
                    "Hubo un problema al cargar los tipos de artefactos.",
                    props.setSnackbarSettings,
                    props.setOpenSnackbar
                );
                return [];
            }
        } catch {
            setSnackbarSettings({ message: "Hubo un problema al cargar los tipos de artefactos.", severity: "error" });
            setOpenSnackbar(true);
            return [];
        }
    }

    switch (step) {
        case 1:
            return (
                <ProviderForm
                    showNewArtifactDialog={showNewArtifactDialog}
                    closeNewArtifactDialog={handleCancel}
                    handleNextStep={handleNextStep}
                />
            );

        case 2:
            if (provider === "Custom") {
                return (
                    <CustomForm
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        getArtifactTypes={getArtifactTypes}
                    />
                );
            }
            else {
                return (
                    <AwsForm
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        projectId={projectId}
                        updateList={updateList}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        getArtifactTypes={getArtifactTypes}
                    />
                );
            }
            

        case 3:
            if (provider === "Custom") {
                return (
                    <SettingsCustomForm
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        setOpenSnackbar={setOpenSnackbar}
                        setSnackbarSettings={setSnackbarSettings}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                    />
                );
            }
            else {
                return (
                    <SettingsAwsForm
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        setOpenSnackbar={setOpenSnackbar}
                        setSnackbarSettings={setSnackbarSettings}
                    />
                );
            }

        case 4:
            if (provider === "AWS") {
                return (
                    <AwsPricingDimension
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        handleNextStep={handleNextStep}
                        handlePreviousStep={handlePreviousStep}
                        setOpenSnackbar={setOpenSnackbar}
                        setSnackbarSettings={setSnackbarSettings}
                    />
                );
            }
            else {
                return (
                    <Confirmation
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        projectId={projectId}
                        updateList={updateList}
                        setOpenSnackbar={setOpenSnackbar}
                        setSnackbarSettings={setSnackbarSettings}
                        handlePreviousStep={handlePreviousStep}
                    />
                );
            }

        case 5:
            if (provider === "AWS") {
                return (
                    <Confirmation
                        showNewArtifactDialog={showNewArtifactDialog}
                        closeNewArtifactDialog={handleCancel}
                        projectId={projectId}
                        updateList={updateList}
                        setOpenSnackbar={setOpenSnackbar}
                        setSnackbarSettings={setSnackbarSettings}
                        handlePreviousStep={handlePreviousStep}
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
