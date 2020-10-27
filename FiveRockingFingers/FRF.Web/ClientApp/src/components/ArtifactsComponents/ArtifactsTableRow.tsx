import * as React from 'react';
import Artifact from '../../interfaces/Artifact'

const ArtifactsTableRow = (props: { artifact: Artifact }) => {

    return (
        <tr>
            <td>{props.artifact.name}</td>
            <td>{props.artifact.provider}</td>
            <td>{props.artifact.artifactType.name}</td>
        </tr>
    );
};

export default ArtifactsTableRow;