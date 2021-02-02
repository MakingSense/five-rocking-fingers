import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Setting from '../../interfaces/Setting';
import PricingTerm from '../../interfaces/PricingTerm';
import ArtifactService from '../../services/ArtifactService';


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

const Confirmation = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, provider: string | null, name: string | null, projectId: number, artifactTypeId: number | null, setOpenSnackbar: Function, setSnackbarSettings: Function, handlePreviousStep: Function, settingsList: Setting[], settings: object, updateList: Function, awsPricingTerm: PricingTerm | null }) => {

    const classes = useStyles();
    const { handleSubmit } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, provider, name, projectId, artifactTypeId, setOpenSnackbar, setSnackbarSettings, settingsList, updateList } = props;

    //Create the artifact after submit
    const handleConfirm = async () => {
        const artifactToCreate = {
            name: name,
            provider: provider,
            artifactTypeId: artifactTypeId,
            projectId: projectId,
            settings: props.settings
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
                    Revise las características de su nuevo artefacto y se está de acuerdo haga click en confirmar
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
                {provider !== 'Custom' && props.awsPricingTerm !== null ?
                    <React.Fragment>
                        {createPropertieLabel("Unit", props.awsPricingTerm.pricingDimensions[0].unit)}
                        {createPropertieLabel("Lease Contract Length", props.awsPricingTerm.leaseContractLength)}
                        {createPropertieLabel("Purchase Option", props.awsPricingTerm.purchaseOption)}
                        {createPropertieLabel("Description", props.awsPricingTerm.pricingDimensions[0].description)}
                        {createPropertieLabel("Currency", props.awsPricingTerm.pricingDimensions[0].currency)}
                        {createPropertieLabel("Price per unit", props.awsPricingTerm.pricingDimensions[0].pricePerUnit.toFixed(10))}
                        {createPropertieLabel("Term", props.awsPricingTerm.term)}
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