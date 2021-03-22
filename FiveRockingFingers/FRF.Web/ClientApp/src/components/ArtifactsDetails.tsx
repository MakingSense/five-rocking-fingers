import * as React from 'react';
import ArtifactsTable from './ArtifactsComponents/ArtifactsTable';
import { RouteComponentProps } from 'react-router';

type TParams = { projectId: string }

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    const projectId = parseInt(match.params.projectId, 10);

    return (
        <ArtifactsTable projectId={projectId} />
    );
}

export default ArtifactsDetails;