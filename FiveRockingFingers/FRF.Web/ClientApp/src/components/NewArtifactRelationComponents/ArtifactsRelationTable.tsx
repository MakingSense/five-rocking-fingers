import * as React from 'react';
import { Table, Button } from 'reactstrap';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelationRow from './ArtifactRelationRow';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import Artifact from '../../interfaces/Artifact';
import Typography from '@material-ui/core/Typography/Typography';
import { Link } from 'react-router-dom';
import NewArtifactsRelation from '../NewArtifactsRelation';

const ArtifactsRelationTable = (props: { artifactId: number, projectId: number }) => {
    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);
    const [updateList, setUpdateList] = React.useState(false);
    const [showNewArtifactsRelation, setShowNewArtifactsRelation] = React.useState(false);

    const getArtifactsRelations = async () => {
        try {
            const response = await ArtifactService.getRelations(props.artifactId);

            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones", severity: "error" });
        }
    }

    const getArtifacts = async () => {
        try {
            const response = await ArtifactService.getAllByProjectId(props.projectId);

            if (response.status == 200) {
                setArtifacts(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar los artefactos", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los artefactos", severity: "error" });
        }
    }
    const openNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(true);
    }
    const closeNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(false);
    }
    React.useEffect(() => {
        getArtifactsRelations();
        getArtifacts();
    }, [updateList]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const handleUpdateList = () => {
        getArtifactsRelations();
        getArtifacts();
    }

    return (
        <React.Fragment>
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>Artefacto 1</th>
                        <th>Setting 1</th>
                        <th>Direccion</th>
                        <th>Artefacto 2</th>
                        <th>Setting 2</th>
                        <th >
                            <Button className="mx-3" style={{ "min-height": "32px", width: "37%" }} color="primary" tag={Link} to={`/projects/${props.projectId}/artifacts/`}>Volver</Button>
                            <Button style={{ "min-height": "32px", width: "38%" }} color="success" onClick={openNewArtifactsRelation}>Nueva relación</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(artifactsRelations)
                        ? artifactsRelations.length > 0 ?
                            artifactsRelations.map((relation, index) =>
                                <ArtifactRelationRow
                                    key={index}
                                    artifactId={props.artifactId}
                                    artifactRelation={relation}
                                    openSnackbar={manageOpenSnackbar}
                                    artifacts={artifacts}
                                    updateList={handleUpdateList} 
                                    artifactsRelations={artifactsRelations}/>
                            )
                            : <td colSpan={6}><Typography align="center" variant="h5" gutterBottom>
                                Artefacto sin relaciones
                      </Typography></td> : null}
                </tbody>
            </Table>
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
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

export default ArtifactsRelationTable;