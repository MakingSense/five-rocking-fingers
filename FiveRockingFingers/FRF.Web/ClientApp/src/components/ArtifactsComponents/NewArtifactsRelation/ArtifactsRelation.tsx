import * as React from 'react';
import NavMenu from '../../../commons/NavMenu';
import ArtifactsRelationTable from './ArtifactsRelationTable';
import { RouteComponentProps } from 'react-router';

type TParams = { artifactId: string }

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    const artifactId = match.params.artifactId;

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsRelationTable artifactId={artifactId} />
        </div>
    );
}

export default ArtifactsDetails;