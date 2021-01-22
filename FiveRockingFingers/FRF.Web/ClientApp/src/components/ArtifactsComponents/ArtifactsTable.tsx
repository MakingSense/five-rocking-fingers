import * as React from 'react';
import { Table, Button } from 'reactstrap';
import Artifact from '../../interfaces/Artifact'
import ArtifactsTableRow from './ArtifactsTableRow';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'
import ArtifactRelation from '../../interfaces/ArtifactRelation'
import NewArtifactDialog from '../NewArtifactDialog';
import NewArtifactsRelation from '../NewArtifactsRelation';
import ArtifactService from '../../services/ArtifactService';
import { makeStyles, Theme, createStyles } from '@material-ui/core';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            '& > *': {
                margin: theme.spacing(1),
            },
        },
    }),
);

const ArtifactsTable = (props: { projectId: number }) => {
    const classes = useStyles();
    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewArtifactDialog, setShowNewArtifactDialog] = React.useState(false);
    const [showNewArtifactsRelation, setShowNewArtifactsRelation] = React.useState(false);
    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);

    const getArtifacts = async () => {
        try {
            const response = await ArtifactService.getAllByProjectId(props.projectId);

            if (response.status == 200) {
                setArtifacts(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
        }
    }

    const getRelations = async () => {
        try {
            const response = await ArtifactService.getAllRelationsByProjectId(props.projectId);

            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones entre artefactos", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones entre artefactos", severity: "error" });
        }
    }

    const closeNewArtifactDialog = () => {
        setShowNewArtifactDialog(false);
    }

    const openNewArtifactDialog = () => {
        setShowNewArtifactDialog(true);
    }

    const openNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(true);
    }

    const closeNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(false);
    }

    React.useEffect(() => {
        getArtifacts();
        getRelations();
    }, [props.projectId]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    return (
        <React.Fragment>
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Provedor</th>
                        <th>Tipo</th>
                        <th className={classes.root}>
                            <Button style={{ "min-height": "32px", width: "21%" }} color="success" onClick={openNewArtifactDialog}>Nuevo artefacto</Button>
                            <Button style={{ "min-height": "32px", width: "20%" }} color="success" onClick={openNewArtifactsRelation}>Nueva relación</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(artifacts)
                        ? artifacts.map((artifact) => <ArtifactsTableRow
                            key={artifact.id}
                            artifact={artifact}
                            openSnackbar={manageOpenSnackbar}
                            updateList={getArtifacts}
                        />
                        )
                        : null}
                </tbody>
            </Table>
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
            <NewArtifactDialog
                showNewArtifactDialog={showNewArtifactDialog}
                closeNewArtifactDialog={closeNewArtifactDialog}
                projectId={props.projectId}
                updateList={getArtifacts}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
            />
            <NewArtifactsRelation
                showNewArtifactsRelation={showNewArtifactsRelation}
                closeNewArtifactsRelation={closeNewArtifactsRelation}
                projectId={props.projectId}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
                artifacts={artifacts}
                artifactsRelations={artifactsRelations}
            />
        </React.Fragment>
    );
};

export default ArtifactsTable;