import React from "react";
import { Route, Redirect, RouteComponentProps } from "react-router-dom";
import ManageProjects from '../../components/ManageProjects';
import {useUser} from '../../commons/useUser';
import NavMenu from "../../commons/NavMenu";

const PrivateRoute: React.FC<{
    component: React.FC | (({ match }: RouteComponentProps<any>) => JSX.Element);
    path: string;
    exact: boolean;
}> = (props) => {
    const { isLogged } = useUser();
    const RouteComponent =  <Route path={props.path} exact={props.exact} component={props.component} />;

    return isLogged ? (
        props.component === ManageProjects ? (
            RouteComponent
        ) : (
            <div className="content">
                <NavMenu />
                {RouteComponent}
            </div>
        )
    ) : (
        <Redirect to="/login" />
    );
};

export default PrivateRoute;