import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormHelperText, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { number } from 'yup';
import { PROVIDERS } from '../Constants';
import ArtifactType from '../interfaces/ArtifactType';
import ArtifactService from '../services/ArtifactService';
import CustomForm from './NewArtifactDialogComponents/CustomForm';
import ProviderForm from './NewArtifactDialogComponents/ProviderForm';
import SettingsCustomForm from './NewArtifactDialogComponents/SettingsCustomForm';

// Once the ArtifactType API and service are running, this should be replaced with a call to that API
// Until then, you might an error if you don't have this 3 types created on your local DataBase before using this

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
    const [artifactTypeId, setArtifactTypeId] = React.useState<number>(0);
    const [name, setName] = React.useState<string>("");

    const { register, handleSubmit, errors, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    React.useEffect(() => {
    }, [step]);

    const handleNextStep = () => {
        setStep(step + 1);
        console.log(step);
    }

    const handlePreviousStep = () => {
        setStep(step - 1);
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
                    setName={setName}
                    setArtifactTypeId={setArtifactTypeId}
                />
            );

        case 3:
            return (
                <SettingsCustomForm
                    showNewArtifactDialog={props.showNewArtifactDialog}
                    closeNewArtifactDialog={props.closeNewArtifactDialog}
                    name={name}
                    projectId={props.projectId}
                    artifactTypeId={artifactTypeId}
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
