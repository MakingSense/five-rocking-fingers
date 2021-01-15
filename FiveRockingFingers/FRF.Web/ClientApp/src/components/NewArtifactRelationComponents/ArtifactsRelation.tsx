import * as React from 'react';
import NavMenu from '../../commons/NavMenu';
import ArtifactsRelationTable from './ArtifactsRelationTable';
import { RouteComponentProps } from 'react-router';

type TParams = { 
    artifactId: string,
    projectId: string }

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsRelationTable artifactId={match.params.artifactId} projectId={match.params.projectId} />
        </div>
    );
}

export default ArtifactsDetails;