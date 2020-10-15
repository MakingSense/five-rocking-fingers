import React, { useState } from 'react';
import { UserContext } from "./components/auth/contextLib";
import { Button } from 'reactstrap';
import axios from 'axios'
import { useHistory } from 'react-router-dom';
import Routes from "./router/Routers";

function App() {

    const History = useHistory();

    function handleLogout() {
        axios.get("https://localhost:44346/api/User/logout");
        History.push("/");
        userHasAuthenticated(null);
    }

    const [isAuthenticated, userHasAuthenticated] = useState<string | null>(null);
    return (
        <UserContext.Provider value={{ isAuthenticated, userHasAuthenticated }}>
            {isAuthenticated
                ? <Button onClick={handleLogout}>Logout</Button>
                : <></>
            }
            <Routes/>
        </UserContext.Provider>
    );
}

export default App;