import axios from 'axios';
import React, { useState } from 'react';
import { useHistory } from 'react-router-dom';
import { Button } from 'reactstrap';
import { UserContext } from "./components/auth/contextLib";
import './custom.css';
import Routes from './router/Routes';
function App() {

    const History = useHistory();

    function handleLogout() {
        axios.get("https://localhost:44346/api/User/logout");
        History.push("/");
        setCurrentUser(null);
    }

    const [currentUser, setCurrentUser] = useState<string | null>(null);
    return (
        <UserContext.Provider value={{ currentUser, setCurrentUser }}>
            {currentUser
                ? <Button display= "flex" flexDirection="row-reverse" onClick={handleLogout}>Logout</Button>
                : <></>
            }
            <Routes/>
        </UserContext.Provider>
    );
}

export default App;
