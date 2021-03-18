import React from "react";
import { Route, Redirect, RouteComponentProps } from "react-router-dom";
import {useUser} from '../../commons/useUser';

const PrivateRoute: React.FC<{
    component: React.FC | (({ match }: RouteComponentProps<any>) => JSX.Element);
    path: string;
    exact: boolean;
}> = (props) => {
    const {isLogged} = useUser();
    return (isLogged) ? (<Route path={props.path} exact={props.exact} component={props.component} />) : (<Redirect to="/login" />);
};

export default PrivateRoute;