import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Typography from '@material-ui/core/Typography';
import Artifact from '../../interfaces/Artifact';
import ArtifactService from '../../services/ArtifactService';


const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        title: {
            fontWeight: 'bold'
        },
        settingName: {
            fontStyle: 'italic'
        },
        listStyleNone: {
            listStyle: 'none'
        }
    }),
);


const EditArtifactConfirmation = (props: {
    artifactToEdit: Artifact,
    namesOfSettingsChanged: string[],
    closeEditArtifactDialog: Function,
    setOpenSnackbar: Function,
    setSnackbarSettings: Function,
    updateArtifacts: Function,
     }) => {

    const classes = useStyles();
    const { handleSubmit } = useForm();

    //Create the artifact after submit
    const handleConfirm = async () => {

        const artifactEdited = {
            name: props.artifactToEdit.name,
            provider: props.artifactToEdit.provider,
            artifactTypeId: props.artifactToEdit.artifactType.id,
            projectId: props.artifactToEdit.projectId,
            settings: {
                settings: props.artifactToEdit.settings
            }
        };

        try {
            const response = await ArtifactService.update(props.artifactToEdit.id, artifactEdited);
            if (response.status === 200) {
                props.setSnackbarSettings({ message: "El artefacto ha sido modificado con éxito", severity: "success" });
                props.setOpenSnackbar(true);
            } else {
                props.setSnackbarSettings({ message: "Hubo un error al modificar el artefacto", severity: "error" });
                props.setOpenSnackbar(true);
            }
        }
        catch (error) {
            props.setSnackbarSettings({ message: "Hubo un error al modificar el artefacto", severity: "error" });
            props.setOpenSnackbar(true);
        }
        props.updateArtifacts();
        props.closeEditArtifactDialog();
    }

    const handleCancel = () => {
        props.closeEditArtifactDialog();
    }

    return (
        <>
            <DialogTitle id="alert-dialog-title">Confirmación</DialogTitle>
            <DialogContent>
                {props.namesOfSettingsChanged.length > 0 ?
                    <Typography gutterBottom color="secondary" className={classes.title}>
                        Tenga en cuenta que al modificar el nombre de alguna de las settings se borrará
                        las relaciones asociadas.
                        Las settings cuyos nombres serán modificados son:
                        <br />
                        <br />
                        <li className={classes.listStyleNone}>
                            {props.namesOfSettingsChanged.map(name => {
                                return (<ul>{name}</ul>);
                            })}
                        </li>
                    </Typography> :
                    null
                }
                <hr/>
                <Typography gutterBottom>
                    Revise las características con las que quedará su artefacto modificado
                    y si está de acuerdo haga click en confirmar
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Nombre:</span> {props.artifactToEdit.name}
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Provedor:</span> {props.artifactToEdit.artifactType.name}
                </Typography>
                <Typography gutterBottom>
                    <span className={classes.title}>Propiedades:</span>
                </Typography>
                {
                    Object.entries(props.artifactToEdit.settings).map(([key, value], index) =>  (
                            <Typography gutterBottom>
                                <span className={classes.settingName}>{key}</span>: {value}
                            </Typography>
                        )
                    )
                }
            </DialogContent>
            <DialogActions>
                <Button size="small" color="primary" type="submit" onClick={handleSubmit(handleConfirm)}>Finalizar</Button>
                <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
            </DialogActions>
        </>
    );
}

export default EditArtifactConfirmation;