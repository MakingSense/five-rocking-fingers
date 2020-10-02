import React, { FormEvent, useState } from "react";
import { Form, FormGroup, Label, Col, Input, Row } from "reactstrap";
import { Paper, Button, FormControlLabel, Checkbox } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import { useAppContext } from "../libs/contextLib";
import axios from "axios"
import "./authStyle.css"

interface Props {
}


const buttonStyle = {
    border: "3px solid #16181a",
    width: "225px"
};


const Login: React.FC<Props> = ({}) => {
    const history = useHistory();
    const { userHasAuthenticated } = useAppContext();


    //State
    const [user, saveUser] = useState({
        email: "",
        password: "",
        rememberMe: false
    });
    const handleCheckClick = (e: React.ChangeEvent<HTMLInputElement>) => {
        const isRemember = !rememberMe;
        user.rememberMe = isRemember;
    };
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        saveUser({
            ...user,
            [e.target.name]: e.target.value
        });
        console.log(user);
    };
    ///

    const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log(user);
        axios.post("https://localhost:44346/api/SignIn",
                {
                    email: user.email,
                    password: user.password,
                    rememberMe: user.rememberMe
                })
            .then(response => {
                if (response.status == 200) {
                    userHasAuthenticated(true);
                    history.push("/Home");
                }
            })
            .catch(error => {
                console.log(error.response.request._response);
            });
    };

    const { email, password, rememberMe } = user;

    return (
        <Paper className="paperForm" elevation={6}>

            <h2 className="contenedor-form text-center">
                <strong>Log In</strong>
            </h2>
            <br/>
            <Form className=" d-flex flex-column" autoComplete="off" Validate onSubmit={handleSubmit}>

                <FormGroup className="campo-form">
                    <Label for="userEmail" md={4}>Email</Label>
                    <Col sm={9}>
                        <Input className="border border-secondary" type="email" name="email" id="userEmail" onChange={
handleChange} value={email}/>
                    </Col>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="userPassword" md={4}>Password</Label>
                    <Col sm={9}>
                        <Input className="border border-secondary" type="password" name="password" id="userPassword" onChange={
handleChange} value={password}/>
                    </Col>
                </FormGroup>
                <FormControlLabel className="alinea-centro"
                                  control={
                        <Checkbox
                            onChange={handleCheckClick}
                            color="default"/>
                    }
                                  label="Remember Me"/>
                <Row className="alinea-centro">
                    <Button className="buttonStyle" variant="outlined" size="medium" type="submit" value="Sign In">Sign In</Button>
                </Row>
            </Form >
            <Row className="alinea-centro">
                <Button className="buttonStyle" variant="outlined" href="/signup" size="medium" value="Sign In">Sign Up</Button>
            </Row>
            <br/>
            <br/>
        </Paper>
    );

};

export default Login;