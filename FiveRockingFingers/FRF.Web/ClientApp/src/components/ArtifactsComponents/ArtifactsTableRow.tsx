import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';

const ArtifactsTableRow = (props: { artifact: Artifact, deleteArtifact: Function }) => {

    const deleteButtonClick = () => {
        props.deleteArtifact(props.artifact.id)
    }

    return (
        <tr>
            <td>{props.artifact.name}</td>
            <td>{props.artifact.provider}</td>
            <td>{props.artifact.artifactType.name}</td>
            <td>
                <Button color="danger" onClick={deleteButtonClick}>Borrar</Button>
            </td>
        </tr>
    );
};

export default ArtifactsTableRow;