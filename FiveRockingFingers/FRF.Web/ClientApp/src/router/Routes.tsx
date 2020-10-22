import React from 'react';
import { Redirect, Route, Switch } from "react-router-dom";
import PrivateRoute from "../components/auth/authService";
import Login from '../components/auth/Login';
import Signup from '../components/auth/Signup';
import Home from '../components/Home';

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

