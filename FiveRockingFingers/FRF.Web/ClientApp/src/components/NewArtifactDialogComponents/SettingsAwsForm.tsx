import { Button, CircularProgress, Dialog, DialogContent } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import Setting from '../../interfaces/Setting';
import KeyValueStringPair from '../../interfaces/KeyValueStringPair';
import AwsArtifactService from '../../services/AwsArtifactService';
import { withTheme } from "@rjsf/core";
import { Theme as MaterialUITheme } from "@rjsf/material-ui";
import ArtifactType from '../../interfaces/ArtifactType';


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
        },
        error: {
            color: 'red'
        }
    }),
);

const SettingsAwsForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, artifactType: ArtifactType | null, updateList: Function, handleNextStep: Function, handlePreviousStep: Function, setAwsSettingsList: Function, setSettingsList: Function, setSnackbarSettings: Function, setOpenSnackbar: Function }) => {

    const classes = useStyles();
    const Form = withTheme(MaterialUITheme);
    const { showNewArtifactDialog, closeNewArtifactDialog, artifactType } = props;

    //Hook for save the user's settings input
    const [loading, setLoading] = React.useState<Boolean>(true);

    const [schema, setSchema] = React.useState<object>({});

    React.useEffect(() => {
        getRequireFields();
    }, [artifactType]);

    const getRequireFields = async () => {
        let statusOk = 200;
        try {            
            if (artifactType !== null) {
                setLoading(true);
                const response = await AwsArtifactService.GetRequiredFieldsAsync(artifactType?.name);
                if (response.status === statusOk) {
                    setSchema(response.data);
                    setLoading(false);
                }
            }
            else {
                props.setSnackbarSettings({ message: "Ha ocurrido un error al cargar el tipo de artefacto", severity: "error" });
                props.setOpenSnackbar(true);
                closeNewArtifactDialog();
            }
        }
        catch (error) {
            props.setSnackbarSettings({ message: "Fuera de servicio. Por favor intente mas tarde.", severity: "error" });
            props.setOpenSnackbar(true);
            closeNewArtifactDialog();
        }
    }

    //Create the artifact after submit
    const handleConfirm = async (data: any) => {
        let formData: Object = data.formData;
        let awsSettingsListAux: KeyValueStringPair[] = [];

        Object.entries(formData).forEach(([key, value]) => {
            let sectionData: Object = value;
            Object.entries(sectionData).forEach(([key, value], index) => {
                let newSetting = { key: key, value: value };
                awsSettingsListAux.push(newSetting);                
            });
        });
        props.setSettingsList(awsSettingsListToSettingsList(awsSettingsListAux));
        props.setAwsSettingsList(awsSettingsListAux);
        props.handleNextStep();
    }

    const awsSettingsListToSettingsList = (awsSettingsListFiltered: KeyValueStringPair[]) => {
        let settingsList: Setting[] = []
        awsSettingsListFiltered.forEach(awsSetting => {
            let setting: Setting = { name: awsSetting.key, value: awsSetting.value }
            settingsList.push(setting);
        });
        return settingsList;
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogContent>
                {loading ?
                    <div className={classes.circularProgress}>
                        <CircularProgress color="inherit" size={30} />
                    </div> :
                    <Form
                        id="my-form"
                        schema={schema}
                        onSubmit={handleConfirm}
                    >
                        <div>
                            <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                            <Button size="small" color="primary" type="submit">Siguiente</Button>
                            <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                        </div>
                    </Form>
                }
            </DialogContent>
        </Dialog>
    );
}

export default SettingsAwsForm;