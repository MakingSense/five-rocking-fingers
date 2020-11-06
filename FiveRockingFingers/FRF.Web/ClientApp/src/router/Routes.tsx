import React from 'react';
import { Redirect, Route, Switch } from "react-router-dom";
import PrivateRoute from "../components/auth/authService";
import Login from '../components/auth/Login';
import Signup from '../components/auth/Signup';
import ArtifactsDetails from '../components/ArtifactsDetails';
import Home from '../components/Home';
import ManageProjects from '../components/ManageProjects';


const Routes = () => (
    <Switch>
        <Redirect exact from="/" to="/home" />
        <Route exact path='/' component={Home} />
        <Route exact path="/login" component={Login} />
        <Route exact path="/signup" component={Signup} />
        <PrivateRoute exact path="/home" component={Home} />
        <PrivateRoute exact path='/administrarProyectos' component={ManageProjects} />
        <PrivateRoute exact path='/projects/:idProject/artifacts/' component={ArtifactsDetails} />
    </Switch>
)

export default Routes;
