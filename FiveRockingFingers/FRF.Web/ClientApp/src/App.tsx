import axios from 'axios';
import React, { useState } from 'react';
import { useHistory } from 'react-router-dom';
import Button from '@material-ui/core/Button';
import ExitToAppOutlinedIcon from '@material-ui/icons/ExitToAppOutlined';
import { UserContext } from "./components/auth/contextLib";
import './custom.css';
import Routes from './router/Routes';
import UserService from './services/UserService';

function App() {

    const History = useHistory();

    function handleLogout() {
        UserService.logout();
        History.push("/");
        setCurrentUser(null);
    }

    const [currentUser, setCurrentUser] = useState<string | null>(null);
    return (
        <UserContext.Provider value={{ currentUser, setCurrentUser }}>
            {currentUser
                ? <Button
                variant="contained"
                color="default"
                startIcon={<ExitToAppOutlinedIcon />}
                onClick={handleLogout}>Logout</Button>: <></>
            }
            <Routes/>
        </UserContext.Provider>
    );
}

export default App;
