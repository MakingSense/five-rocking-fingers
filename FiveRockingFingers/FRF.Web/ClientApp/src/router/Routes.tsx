import React from 'react';
import { Switch, Route } from 'react-router';
import Home from '../components/Home';
import ArtifactsDatails from '../components/ArtifactsDetails';


const Routes = () => (
    <Switch>
        <Route exact path='/project/:idProject/artifact/:idArtifact' component={ArtifactsDatails} />
        <Route exact path='/' component={Home} />
    </Switch>
)

export default Routes;

