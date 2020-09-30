import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';

import { Provider } from 'react-redux';
import configureStore from './store/configureStore';

import './custom.css'

let store = configureStore();

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
    </Layout>
);
