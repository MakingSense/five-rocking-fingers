import * as React from 'react';
import { Redirect, Route, Switch } from "react-router-dom";
import PrivateRoute from "../components/auth/authService";
import Login from '../components/auth/Login';
import Signup from '../components/auth/Signup';
import ArtifactsDetails from '../components/ArtifactsDetails';
import Home from '../components/Home';
import ManageProjects from '../components/ManageProjects';
import ArtiactsRelation from '../components/NewArtifactRelationComponents/ArtifactsRelation';
import ResourcesTable from '../components/ResourcesComponents/ResourcesTable';;


const Routes = () => (
    <Switch>
        <Redirect exact from="/" to="/home" />
        <Route exact path="/login" component={Login} />
        <Route exact path="/signup" component={Signup} />
        <PrivateRoute exact path="/home" component={Home} />
        <PrivateRoute exact path='/administrarProyectos' component={ManageProjects} />
        <PrivateRoute exact path='/projects/:projectId/artifacts/' component={ArtifactsDetails} />
        <PrivateRoute exact path='/projects/:projectId/artifacts/:artifactId' component={ArtiactsRelation} />
        <PrivateRoute exact path='/resources' component={ResourcesTable} />
    </Switch>
)

export default Routes;
