import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Setting from '../../interfaces/Setting';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import PricingTerm from '../../interfaces/PricingTerm';
import AwsArtifactService from '../../services/AwsArtifactService';
import { useSettingsCreator } from './useSettingsCreator';
import { usePricingDimensionsValidator } from './usePricingDimensionsValidator';


const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        container: {
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
        setting: {
            display: 'block',
            width: '100%'
        }
    }),
);

const AwsPricingDimension = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, name: string | null, handleNextStep: Function, handlePreviousStep: Function, settingsList: Setting[], setSettingsList: Function, settingsMap: { [key: string]: number[] }, setSettingsMap: Function, awsSettingsList: KeyValueStringPair[], settings: object, setSettings: Function, setAwsPricingTerm: Function, setSnackbarSettings: Function, setOpenSnackbar: Function }) => {

    const classes = useStyles();
    const { handleSubmit } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, name, awsSettingsList } = props;

    const { createPricingTermSettings } = useSettingsCreator();
    const { areValidPricingDimensions } = usePricingDimensionsValidator();

    const [loading, setLoading] = React.useState<Boolean>(true);
    const [awsPricingDimensionList, setPricingDimensionList] = React.useState<PricingTerm[]>([]);

    React.useEffect(() => {
        getPricingDimensions();
    }, [name, awsSettingsList]);

    const getPricingDimensions = async () => {
        if (name) {
            let statusOk = 200;
            try {
                setLoading(true);
                const response = await AwsArtifactService.GetProductsAsync(name, awsSettingsList);
                if (response.status === statusOk) {
                    setPricingDimensionList(response.data);
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
    const handleConfirm = async () => {
        if (!props.name) return;
        props.setSettings({ settings: createSettings(props.name) });
        props.handleNextStep();
    }

    const createSettings = (serviceCode: string) => {
        let settingFinalObject: object = {};
        let settingsObject: { [key: string]: string } = {};

        for (let i = 0; i < props.settingsList.length; i++) {
            settingsObject[props.settingsList[i].name] = props.settingsList[i].value;
        }
        Object.assign(settingFinalObject, settingsObject, createPricingTermSettings(serviceCode, awsPricingDimensionList));

        return settingFinalObject;
    }

    const createPropertieLabel = (propertie: string, value: string | null) => {
        if (value === null) return null;
        return (
            <Typography gutterBottom className={classes.setting}>
                {propertie}: {value}
            </Typography>            
        );
    }

    const pricingTermToString = (pricingTerm: PricingTerm) => {
        return (
            <React.Fragment>
                {createPropertieLabel("Product", pricingTerm.product)}
                {createPropertieLabel("Term", pricingTerm.term)}
                {createPropertieLabel("Purchase Option", pricingTerm.purchaseOption)}
                {createPropertieLabel("Lease Contract Length", pricingTerm.leaseContractLength)}
                {pricingTerm.pricingDimensions.map(pricingDimension => {
                    return (
                        <>
                            { createPropertieLabel("Unit", pricingDimension.unit)}
                            { createPropertieLabel("Description", pricingDimension.description)}
                            { createPropertieLabel("Currency", pricingDimension.currency)}
                            { createPropertieLabel("Price per unit", pricingDimension.pricePerUnit.toFixed(10))}
                            <hr />
                        </>
                        );                    
                })}               
                <hr style={{
                    borderWidth: "7px",
                }} />
            </React.Fragment>
        );
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Formulario de artefactos AWS</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación se muestra la opción que provee AWS, según sus especificaciones
                </Typography>
                <form className={classes.container}>
                    {loading ?
                        <div className={classes.circularProgress}>
                            <CircularProgress color="inherit" size={30} />
                        </div> :
                        props.name !== null && areValidPricingDimensions(props.name, awsPricingDimensionList) ?
                            <Typography gutterBottom>
                                Lo sentimos, no hay productos según las especificaciones seleccionadas
                            </Typography> :
                            awsPricingDimensionList.map((awsPricingTerm: PricingTerm, index: number) => {
                                return (
                                    pricingTermToString(awsPricingTerm)
                                    );
                            })                
                    }
                </form>

            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Siguiente</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default AwsPricingDimension;