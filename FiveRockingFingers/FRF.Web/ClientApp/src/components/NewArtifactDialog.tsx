import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { PROVIDERS } from '../Constants';
import ArtifactType from '../interfaces/ArtifactType';
import ArtifactService from '../services/ArtifactService';
import CustomForm from './NewArtifactDialogComponents/CustomForm';
import ProviderForm from './NewArtifactDialogComponents/ProviderForm';
import SettingsCustomForm from './NewArtifactDialogComponents/SettingsCustomForm';

// Once the ArtifactType API and service are running, this should be replaced with a call to that API
// Until then, you might an error if you don't have this 3 types created on your local DataBase before using this
const constArtifactTypes = [
    {
      "id": 5,
      "name": "Atype",
      "description": "ADescription"
    },
    {
      "id": 6,
      "name": "Btype",
      "description": "BDescription"
    },
    {
      "id": 7,
      "name": "Ctype",
      "description": "CDescription"
    }
]

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
        }
    }),
);

const NewArtifactDialog = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, updateList: Function, setOpenSnackbar: Function , setSnackbarSettings: Function }) => {

    const classes = useStyles();

    const [step, setStep] = React.useState<number>(1);

    const { register, handleSubmit, errors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    const [artifactTypes, setArtifactTypes] = React.useState([] as ArtifactType[]);
    const [provider, setProvider] = React.useState<string | null>(null);

    React.useEffect(() => {
        setArtifactTypes(constArtifactTypes);
    }, [step]);

    const handleNextStep = () => {
        setStep(step + 1);
        console.log(step);
    }

    const handlePreviousStep = () => {
        setStep(step - 1);
    }

    const handleConfirmProvider = async (data: { provider: string }) => {
        const artifactToCreate = {
            provider: data.provider
        };

        console.log(artifactToCreate);
    }

    const handleConfirm = async (data: { name: string, provider: string, artifactType: string }) => {
        const artifactToCreate = {
            name: data.name.trim(),
            provider: data.provider,
            artifactTypeId: parseInt(data.artifactType, 10),
            projectId: projectId,
            settings: { empty: "" }
        };

        console.log(artifactToCreate);

        try {
            const response = await ArtifactService.save(artifactToCreate);
            if (response.status === 200) {
                setSnackbarSettings({ message: "El artefacto ha sido creado con éxito", severity: "success" });
                setOpenSnackbar(true);
                updateList();
            } else {
                setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
                setOpenSnackbar(true);
            }
        }
        catch (error) {
            setSnackbarSettings({ message: "Hubo un error al crear el artefacto", severity: "error" });
            setOpenSnackbar(true);
        }
        closeNewArtifactDialog()
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const handleProviderChange = (provider: string) => {
        setProvider(provider);
    }

    switch (step) {
        case 1:
            return (
                <ProviderForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={props.closeNewArtifactDialog}
                    handleNextStep={handleNextStep}
                />
            );

        case 2:
            return (
                <CustomForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={props.closeNewArtifactDialog}
                    handleNextStep={handleNextStep}
                    handlePreviousStep={handlePreviousStep}
                />
            );

        case 3:
            return (
                <SettingsCustomForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={props.closeNewArtifactDialog}
                    projectId={props.projectId}
                    updateList={props.updateList}
                    setOpenSnackbar={props.setOpenSnackbar}
                    setSnackbarSettings={props.setSnackbarSettings}
                    handleNextStep={handleNextStep}
                    handlePreviousStep={handlePreviousStep}
                />
            );

        default:
            return null;
    }
}

export default NewArtifactDialog;
