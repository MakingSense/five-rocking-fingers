import * as React from 'react';
import Table from 'bootstrap';
import Artifact from '../../interfaces/Artifact'
import ArtifactsTableRow from './ArtifactsTableRow';
import axios from 'axios';

const ArtifactsTable = () => {

    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);

    const getArtifacts = async () => {
        const response = await axios.get("http://localhost:4000/artifacts");
        setProjects(response.data);
        console.log(response.data);
    }

    React.useEffect(() => {
        getArtifacts();
    }, [artifacts.length]);

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
                    ? artifacts.map((artifact) => <ArtifactsTableRow key={artifact.id} artifact={artifact} />)
                    : null}
            </tbody>
        </Table>
    );
};

export default ArtifactsTable;