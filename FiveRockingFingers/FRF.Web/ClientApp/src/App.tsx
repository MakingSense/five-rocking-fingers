import * as React from 'react';
import { Route, Router, Switch } from 'react-router';
import { createBrowserHistory } from 'history';
import './custom.css';
import Routes from './router/Routes';

const history = createBrowserHistory();

export default () => (
    <Router history={history}>
        <Routes />
    </Router>
    
);
