import * as React from 'react';
import axios from 'axios';
import NavMenu from '../commons/NavMenu';
import ArtifactsTable from './ArtifactsComponents/ArtifactsTable'

const ArtifactsDetails = () => {

    return (
        <div className='content'>
            <NavMenu />
            <ArtifactsTable />
        </div>
    );
}

export default ArtifactsDetails;