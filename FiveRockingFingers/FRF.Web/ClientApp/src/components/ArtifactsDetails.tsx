import * as React from 'react';
import NavMenu from '../commons/NavMenu';
import ArtifactsTable from './ArtifactsComponents/ArtifactsTable';
import { RouteComponentProps } from 'react-router';

type TParams = { projectId: string }

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    const projectId = parseInt(match.params.projectId, 10);

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsTable projectId={projectId} />
        </div>
    );
}

export default ArtifactsDetails;