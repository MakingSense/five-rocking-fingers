import * as React from 'react';
import axios from 'axios';
import NavMenu from '../commons/NavMenu';

const ArtifactsDetails = () => {

    return (
        <div className='content'>
            <NavMenu />
            <h1>Vamos a mostrar los artifacts de los proyectos!!</h1>
        </div>
    );
}

export default ArtifactsDetails;