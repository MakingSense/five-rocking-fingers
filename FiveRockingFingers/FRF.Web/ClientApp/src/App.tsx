import { createBrowserHistory } from 'history';
import * as React from 'react';
import { Router } from 'react-router';
import './custom.css';
import Routes from './router/Routes';

const history = createBrowserHistory();

export default () => (
    <Router history={history}>
        <Routes />
    </Router>
    
);
