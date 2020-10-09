import * as React from 'react';
import NavMenu from '../commons/NavMenu';

import axios from 'axios';

const Home = () => {
    return (

        <div className='content'>
            <NavMenu />
            <div>
                <h1>{"Lets estimate some projects"}</h1>
            </div>
        </div>
    );
}

export default Home;
