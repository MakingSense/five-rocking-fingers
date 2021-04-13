import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Setting from '../../interfaces/Setting';
import ArtifactService from '../../services/ArtifactService';
import { useArtifact } from '../../commons/useArtifact';


const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        title: {
            fontWeight: 'bold'
        },
        settingName: {
            fontStyle: 'italic'
        }
    }),
);

const Confirmation = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, projectId: number, setOpenSnackbar: Function, setSnackbarSettings: Function, handlePreviousStep: Function, updateList: Function }) => {

    const classes = useStyles();
    const { handleSubmit } = useForm();
    const { name, provider, artifactType, settings, settingTypes, settingsList, awsPricingTerm } = useArtifact();
    const { showNewArtifactDialog, closeNewArtifactDialog, projectId, setOpenSnackbar, setSnackbarSettings, updateList } = props;

    //Create the artifact after submit
    const handleConfirm = async () => {
        const artifactToCreate = {
            name: name,
            provider: provider,
            artifactTypeId: artifactType?.id,
            projectId: projectId,
            settings: settings,
            relationalFields: settingTypes
        };

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
        closeNewArtifactDialog();
    }

    const createPropertieLabel = (propertie: string, value: string | null) => {
        if (value !== null) {
            return (
                <Typography gutterBottom>
                    <span className={classes.settingName}>{propertie}</span>: {value}
                </Typography>
            );
        } else {
            return null;
        }
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Confirmación</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    Revise las características de su nuevo artefacto y se está de acuerdo haga click en Finalizar
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Nombre:</span> {name}
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Provedor:</span> {provider}
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Propiedades:</span>                    
                </Typography>
                {
                    settingsList.map((setting: Setting, index: number) => {
                        return (
                            <Typography gutterBottom>
                                <span className={classes.settingName}>{setting.name}</span>: {setting.value}
                            </Typography>
                        );
                    })
                }
                {provider !== 'Custom' && awsPricingTerm !== null ?
                    <React.Fragment>
                        {createPropertieLabel("Unit", awsPricingTerm.pricingDimensions[0].unit)}
                        {createPropertieLabel("Lease Contract Length", awsPricingTerm.leaseContractLength)}
                        {createPropertieLabel("Purchase Option", awsPricingTerm.purchaseOption)}
                        {createPropertieLabel("Description", awsPricingTerm.pricingDimensions[0].description)}
                        {createPropertieLabel("Currency", awsPricingTerm.pricingDimensions[0].currency)}
                        {createPropertieLabel("Price per unit", awsPricingTerm.pricingDimensions[0].pricePerUnit.toFixed(10))}
                        {createPropertieLabel("Term", awsPricingTerm.term)}
                    </React.Fragment>
                    : null
                }
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default Confirmation;
