import React from "react";
import { Route, Redirect } from "react-router-dom";
import { useUserContext } from "./contextLib";

const PrivateRoute: React.FC<{
    component: React.FC;
    path: string;
    exact: boolean;
}> = (props) => {
    const { isAuthenticated } = useUserContext();
    return (isAuthenticated != null) ? (<Route path={props.path} exact={props.exact} component={props.component} />) : (<Redirect to="/login" />);
};

export default PrivateRoute;