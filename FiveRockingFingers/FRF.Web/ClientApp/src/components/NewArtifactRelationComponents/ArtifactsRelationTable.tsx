import * as React from 'react';
import { Table, Button } from 'reactstrap';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'
import ArtifactService from '../../services/ArtifactService';
import NewArtifactsRelation from '../NewArtifactsRelation';
import ArtifactRelationRow from './ArtifactRelationRow';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import Artifact from '../../interfaces/Artifact';

const ArtifactsRelationTable = (props: { artifactId: string, projectId: string }) => {

    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewArtifactDialog, setShowNewArtifactDialog] = React.useState(false);
    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);

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
            const response = await ArtifactService.getAllByProjectId(parseInt(props.projectId, 10));

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

    const closeNewArtifactDialog = () => {
        setShowNewArtifactDialog(false);
    }

    const openNewArtifactDialog = () => {
        setShowNewArtifactDialog(true);
    }

    React.useEffect(() => {
        getArtifactsRelations();
        getArtifacts();
    }, [props.artifactId]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    return (
        <React.Fragment>
            <Table striped bordered hover>
                <thead>
                    <tr>
                        <th>Artefacto 1</th>
                        <th>Artefacto 2</th>
                        <th>Setting 1</th>
                        <th>Setting 2</th>
                        <th>Direccion</th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(artifactsRelations)
                        ? artifactsRelations.map((relation, index) =>
                            <ArtifactRelationRow key={index} artifactRelation={relation} openSnackbar={manageOpenSnackbar} artifacts={artifacts} />
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
            <Button />

        </React.Fragment>
    );
};

export default ArtifactsRelationTable;