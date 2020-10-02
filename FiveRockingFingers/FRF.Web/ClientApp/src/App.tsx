import { useState } from 'react';
import Routes from "./Routes";
import { AppContext } from "./libs/contextLib";
import { NavItem,Button } from 'reactstrap';
import axios from 'axios'
import { useHistory } from 'react-router-dom';

function App() {
    const history = useHistory();
    function handleLogout() {
        axios.get("https://localhost:44346/api/User/logout");
        history.push("/");
        userHasAuthenticated(false);
    }
    
    const [isAuthenticated, userHasAuthenticated] = useState(false);
    return (
        <AppContext.Provider value={{ isAuthenticated, userHasAuthenticated }}>
            {isAuthenticated
                ? <Button onClick={handleLogout}>Logout</Button>
                : <></>
            }
            <Routes />
        </AppContext.Provider>
    );
}

export default App;