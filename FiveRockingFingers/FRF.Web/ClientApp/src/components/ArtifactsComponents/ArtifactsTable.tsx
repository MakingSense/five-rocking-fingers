import * as React from 'react';
import { Table } from 'reactstrap';
import Artifact from '../../interfaces/Artifact'
import ArtifactsTableRow from './ArtifactsTableRow';
import axios from 'axios';

const ArtifactsTable = (props: { projectId: number }) => {

    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);

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
                                                        deleteArtifact={deleteArtifact} />)
                    : null}
            </tbody>
        </Table>
    );
};

export default ArtifactsTable;