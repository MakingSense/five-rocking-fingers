import React from 'react';
import { Switch, Route } from 'react-router';
import ArtifactsDetails from '../components/ArtifactsDetails';
import Home from '../components/Home';


const Routes = () => (
    <Switch>
        <Route exact path='/project/artifacts/:idProject/' component={ArtifactsDetails} />
        <Route exact path='/' component={Home} />
    </Switch>
)

export default Routes;

