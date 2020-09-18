import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import Home from './Home';

export default (props: { children?: React.ReactNode }) => (
    <div className="contenido">
        <NavMenu />
        <Home />
    </div>
);
