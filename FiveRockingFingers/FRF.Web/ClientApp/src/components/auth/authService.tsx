import React from "react";
import { Route, Redirect, match, RouteComponentProps } from "react-router-dom";
import { useUserContext } from "./contextLib";


const PrivateRoute: React.FC<{
    component: React.FC | (({ match }: RouteComponentProps<any>) => JSX.Element);
    path: string;
    exact: boolean;
}> = (props) => {
    const { currentUser } = useUserContext();
    return (currentUser != null) ? (<Route path={props.path} exact={props.exact} component={props.component} />) : (<Redirect to="/login" />);
};

export default PrivateRoute;