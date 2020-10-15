import React from 'react';
import { Route, Switch, Redirect } from "react-router-dom";
import Home from '../components/Home';
import Login from '../components/auth/Login';
import Signup from '../components/auth/Signup';
import PrivateRoute from "../components/auth/authService"

export default function Routes() {
    return (
        <Switch>
            <Redirect exact from="/" to="/home" />
            <Route exact path="/login" component={Login} />
            <Route exact path="/signup" component={Signup} />
            <PrivateRoute exact path="/home" component={Home} />
        </Switch>
    );
}