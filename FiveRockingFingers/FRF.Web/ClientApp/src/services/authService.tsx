import React, { useState } from "react";
import { Route, Redirect } from "react-router-dom";


const PrivateRoute: React.FC<{
    component: React.FC;
    path: string;
    exact: boolean;
}> = (props) => {
    let isLogged: string  | null = '';
    isLogged=sessionStorage.getItem('currentUser');
    console.log(isLogged);
    return (isLogged!=null || isLogged =='') ? (<Route path={props.path} exact={props.exact} component={props.component} />) :(<Redirect to="/login" />);
};

export  default  PrivateRoute;