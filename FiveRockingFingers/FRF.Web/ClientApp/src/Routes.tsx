import { Route, Switch } from "react-router-dom";
import Layout from './components/Layout';
import Login from './auth/Login';
import Signup from './auth/Signup';

export default function Routes() {
    return (
        <Switch>
            <Route exact path="/" component={Login} />
            <Route exact path="/login" component={Login} />
            <Route exact path="/signup" component={Signup} />
            <Route exact path="/home" component={Layout} />
        </Switch>
    );
}
