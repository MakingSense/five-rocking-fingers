import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, IconButton } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import ArtifactService from '../../services/ArtifactService';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';
import DeleteIcon from '@material-ui/icons/Delete';


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

interface Setting {
    settingName: string;
    settingValue: string;
    [key: string]: string;
}

const SettingsCustomForm = (props: { showNewArtifactDialog: boolean, closeNewArtifactDialog: Function, provider: string | null, name: string | null, projectId: number, artifactTypeId: number | null, updateList: Function, setOpenSnackbar: Function, setSnackbarSettings: Function, handleNextStep: Function, handlePreviousStep: Function }) => {

    const classes = useStyles();

    const { handleSubmit, register, errors, setError, clearErrors } = useForm();
    const { showNewArtifactDialog, closeNewArtifactDialog, provider, name, projectId, artifactTypeId, updateList, setOpenSnackbar, setSnackbarSettings } = props;

    const [settings, setSettings] = React.useState<{}>({});
    const [settingsList, setSettingsList] = React.useState<Setting[]>([{ settingName: "", settingValue: "" }]);

    const [areNamesRepeate, setAreNameRepeate] = React.useState<boolean>(false);

    React.useEffect(() => {
        console.log(settings);
    }, [settings]);

    const handleConfirm = async () => {

        const artifactToCreate = {
            name: name,
            provider: provider,
            artifactTypeId: artifactTypeId,
            projectId: projectId,
            settings: { settings: createSettingsObject()}
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
        closeNewArtifactDialog();
    }

    const handleError = () => {

    }

    const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>, index: number) => {
        checkSettingNameRepeat(event);
        const { name, value } = event.target;
        const list = [...settingsList];
        list[index][name] = value;
        setSettingsList(list);
    }

    const checkSettingNameRepeat = (event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
        if (event.target.name === "settingName") {
            if (settingsList.find(setting => setting.settingName === event.target.value)) {
                setError("settingName", {
                    type: "manual",
                    message: "Los nombres no pueden repetirse"
                });
                setAreNameRepeate(true);
            }
            else {
                clearErrors("settingName");
                setAreNameRepeate(false);
            }
        }
    }

    const handleCancel = () => {
        closeNewArtifactDialog();
    }

    const handleAddSetting = () => {
        setSettingsList([...settingsList, { settingName: "", settingValue: "" }]);
    }

    const handleDeleteSetting = (index: number) => {
        const list = [...settingsList];
        list.splice(index, 1);
        setSettingsList(list);
    }

    const createSettingsObject = () => {
        let settingsObject: { [key: string]: string } = {};

        for (let i = 0; i < settingsList.length; i++) {
            settingsObject[settingsList[i].settingName] = settingsList[i].settingValue
            console.log(settingsObject);
        }

        return JSON.parse(JSON.stringify(settingsObject));
    }

    const goPrevStep = () => {
        props.handlePreviousStep();
    }

    return (
        <Dialog open={showNewArtifactDialog}>
            <DialogTitle id="alert-dialog-title">Bienvenido al asistente para la creación de un nuevo artefacto</DialogTitle>
            <DialogContent>
                <Typography gutterBottom>
                    A continuación ingrese las propiedades de su nuevo artefacto y el valor que tomarán esas propiedades
                </Typography>
                <form className={classes.container}>
                    {settingsList.map((setting: Setting, index: number) => {
                        return (
                            <React.Fragment>
                                <TextField
                                    inputRef={register({ required: true, validate: { isValid: value => value.trim() != "", isRepeate: () => !areNamesRepeate } })}
                                    error={errors.settingName ? true : false}
                                    id="settingName"
                                    name="settingName"
                                    label="Nombre de la setting"
                                    helperText={errors.settingName ? errors.settingName.message : "Requerido*"}
                                    variant="outlined"
                                    value={setting.settingName}
                                    required
                                    className={classes.inputF}
                                    onChange={event => handleInputChange(event, index)}
                                />

                                <TextField
                                    inputRef={register({ required: true, validate: { isValid: value => value.trim() != "" } })}
                                    error={errors.name ? true : false}
                                    id="settingValue"
                                    name="settingValue"
                                    label="Valor de la setting"
                                    helperText="Requerido*"
                                    variant="outlined"
                                    value={setting.settingValue}
                                    required
                                    className={classes.inputF}
                                    onChange={event => handleInputChange(event, index)}
                                />
                                {settingsList.length - 1 === index &&
                                    <IconButton onClick={handleAddSetting } aria-label="add" color="primary">
                                        <AddCircleIcon />
                                    </IconButton>
                                }

                                {settingsList.length !== 1 &&
                                    <IconButton onClick={event => handleDeleteSetting(index)} aria-label="delete" color="secondary">
                                        <DeleteIcon />
                                    </IconButton>
                                }
                        </React.Fragment>
                        ); 
                    })}
                    
                </form>
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" onClick={event => goPrevStep()}>Atrás</Button>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm, handleError)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SettingsCustomForm;