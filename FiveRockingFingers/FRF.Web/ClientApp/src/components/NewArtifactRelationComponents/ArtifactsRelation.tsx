import * as React from 'react';
import NavMenu from '../../commons/NavMenu';
import ArtifactsRelationTable from './ArtifactsRelationTable';
import { RouteComponentProps } from 'react-router';

type TParams = {
    artifactId: string,
    projectId: string
}

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    let projectId = parseInt(match.params.projectId, 10);
    let artifactId = parseInt(match.params.artifactId, 10);

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsRelationTable artifactId={artifactId} projectId={projectId} />
        </div>
    );
}

export default ArtifactsDetails;