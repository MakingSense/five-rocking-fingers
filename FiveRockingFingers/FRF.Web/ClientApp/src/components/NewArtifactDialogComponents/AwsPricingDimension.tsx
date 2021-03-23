import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, FormControlLabel, FormHelperText, InputLabel, MenuItem, Radio, RadioGroup, Select } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import * as React from 'react';
import { Controller, useForm } from 'react-hook-form';
import ProviderArtifactSetting from '../../interfaces/ProviderArtifactSetting';
import Setting from '../../interfaces/Setting';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import PricingTerm from '../../interfaces/PricingTerm';
import ArtifactService from '../../services/ArtifactService';
import AwsArtifactService from '../../services/AwsArtifactService';
import PricingDimension from '../../interfaces/PricingDimension';
import { useSettingsCreator } from './useSettingsCreator';


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
        }
    }),
);

const AwsPricingDimension = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, name: string | null, handleNextStep: Function, handlePreviousStep: Function, settingsList: Setting[], setSettingsList: Function, settingsMap: { [key: string]: number[] }, setSettingsMap: Function, awsSettingsList: KeyValueStringPair[], settings: object, setSettings: Function, setAwsPricingTerm: Function, setSnackbarSettings: Function, setOpenSnackbar: Function }) => {

    const classes = useStyles();
    const { handleSubmit, control } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, name, awsSettingsList } = props;

    const { createPricingTermSettings } = useSettingsCreator();

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
                    console.log(response.data);
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
            <Typography gutterBottom>
                {propertie}: {value}
            </Typography>
        );
    }

    const pricingTermToString = (pricingTerm: PricingTerm) => {
        return (
            <React.Fragment>
                {createPropertieLabel("Product", pricingTerm.product)}
                {createPropertieLabel("Unit", pricingTerm.pricingDimensions[0].unit)}
                {createPropertieLabel("Lease Contract Length", pricingTerm.leaseContractLength)}
                {createPropertieLabel("Purchase Option", pricingTerm.purchaseOption)}
                {createPropertieLabel("Description", pricingTerm.pricingDimensions[0].description)}
                {createPropertieLabel("Currency", pricingTerm.pricingDimensions[0].currency)}
                {createPropertieLabel("Price per unit", pricingTerm.pricingDimensions[0].pricePerUnit.toFixed(10))}
                {createPropertieLabel("Term", pricingTerm.term)}
                <hr/>
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
                    A continuación ingrese alguna de las opciones para adquirir su artefacto AWS
                </Typography>
                <form className={classes.container}>
                    {loading ?
                        <div className={classes.circularProgress}>
                            <CircularProgress color="inherit" size={30} />
                        </div> :
                        awsPricingDimensionList.length === 0 ?
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