import * as React from 'react';
import axios from 'axios';
import NavMenu from '../commons/NavMenu';
import ArtifactsTable from './ArtifactsComponents/ArtifactsTable';
import { RouteComponentProps } from 'react-router';

type TParams = { idProject: string }

const ArtifactsDetails = ({ match }: RouteComponentProps<TParams>) => {

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsTable projectId={+match.params.idProject} />
        </div>
    );
}

export default ArtifactsDetails;