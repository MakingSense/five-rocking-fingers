import React from 'react';
import { Switch, Route } from 'react-router';
import Home from '../components/Home';
import ManageProjects from '../components/ManageProjects';


const Routes = () => (
    <Switch>
        <Route exact path='/' component={Home} />
        <Route exact path='/administrarProyectos' component={ManageProjects} />
    </Switch>
)

export default Routes;

