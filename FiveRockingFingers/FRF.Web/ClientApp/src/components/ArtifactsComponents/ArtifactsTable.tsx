﻿import * as React from 'react';
import { Table } from 'reactstrap';
import Artifact from '../../interfaces/Artifact'
import ArtifactsTableRow from './ArtifactsTableRow';
import axios from 'axios';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'

const ArtifactsTable = (props: { projectId: number }) => {

    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });

    const getArtifacts = async () => {
        const response = await axios.get("https://localhost:44346/api/Artifacts/GetAllByProjectId/" + props.projectId);
        setArtifacts(response.data);
    }

    const deleteArtifact = async (artifactId: number) => {
        var route = "https://localhost:44346/api/Artifacts/Delete/" + artifactId.toString();
        await axios.delete(route);
        getArtifacts();
    }

    React.useEffect(() => {
        getArtifacts();
    }, [props.projectId]);

    return (
        <React.Fragment>
            <Table striped bordered hover>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Provedor</th>
                        <th>Tipo</th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(artifacts)
                        ? artifacts.map((artifact) => <ArtifactsTableRow
                            key={artifact.id}
                            artifact={artifact}
                            deleteArtifact={deleteArtifact}
                            setOpenSnackbar = {setOpenSnackbar }
                            setSnackbarSettings={setSnackbarSettings}
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

        </React.Fragment>
    );
};

export default ArtifactsTable;