import * as React from 'react';
import { Route, Router, Switch } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import configureStore from './store/configureStore';
import { createBrowserHistory } from 'history';

import ProjectPreview from './components/ProjectPreview';

import './custom.css'

const history = createBrowserHistory();

export default () => (
    <Router history={history}>
        <Switch>
            <Route path='/preview/:id' component={ProjectPreview} />
            <Route exact path='/' component={Home} />
        </Switch>
    </Router>
    
);
