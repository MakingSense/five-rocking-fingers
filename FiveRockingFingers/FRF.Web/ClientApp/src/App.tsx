import React from 'react';
import './custom.css';
import Routes from './router/Routes';
import { UserContextProvider } from './commons/UserContext';

function App() {
    return (
        <UserContextProvider>
            <Routes/>
        </UserContextProvider>
    );
}

export default App;
