import React from 'react';
import { Route, Switch, Redirect } from "react-router-dom";
import Layout from '../components/Layout';
import Login from '../auth/Login';
import Signup from '../auth/Signup';
import PrivateRoute from "../services/authService"

export default function Routes() {
    return (
        <Switch>
            <Redirect from="/" to="/home" exact />
            <Route exact path="/login" component={Login} />
            <Route exact path="/signup" component={Signup} />
            <PrivateRoute  path="/home"  component={Layout}  exact  />
        </Switch>
    );
}
