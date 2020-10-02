import React, { FormEvent, useState } from 'react';
import { Form, FormGroup, Label, Col, Input, Row } from 'reactstrap';
import { Paper, Button } from '@material-ui/core';
import { Link, useHistory } from 'react-router-dom';
import { useAppContext } from "../libs/contextLib";
import axios from 'axios'
import "./authStyle.css"

interface Props {
}

const Signup: React.FC<Props> = ({}) => {
    const history = useHistory();
    const { userHasAuthenticated } = useAppContext();

    //State
    const [user, saveUser] = useState({
        firstName: '',
        familyName: '',
        email: '',
        password: '',
        confirm: '',
    });
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        saveUser({
            ...user,
            [e.target.name]: e.target.value
        })
    };
    ///

    const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        //validar vacios

        console.log(user);
        axios.post('https://localhost:44346/api/SignUp',
                {
                    confirmPassword: user.confirm,
                    name: user.firstName,
                    familyName: user.familyName,
                    email: user.email,
                    password: user.password
                })
            .then(response => {
                if (response.status == 200) {
                    userHasAuthenticated(true);
                    history.push("/Home");
                }
            })
            .catch(error => {
                console.log(error.response.request._response)
            });
    }

    const { firstName, familyName, email, password, confirm } = user;

    return (
        <Paper className="paperForm" elevation={6}>
            <br></br>
            <h2 className="text-center">
                <strong>Sign Up</strong>
            </h2>

            <Form className=" d-flex flex-column" autoComplete="off" Validate onSubmit={handleSubmit}>
                <FormGroup className="campo-form">
                    <Label for="firstName">Name</Label>
                    <Input className="border border-secondary" type="text" name="firstName" id="firstName" onChange={
handleChange} value={firstName}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="familyName">Family name</Label>
                    <Input className="border border-secondary" type="text" name="familyName" id="familyName" onChange={
handleChange} value={familyName}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="userEmail">Email</Label>
                    <Input className="border border-secondary" type="email" name="email" id="userEmail" onChange={
handleChange} value={email}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="userPassword">Password</Label>
                    <Input className="border border-secondary" type="password" name="password" id="userPassword" onChange={
handleChange} value={password}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="confirmPassword">Password</Label>
                    <Input className="border border-secondary" type="password" name="confirm" id="confirmPassword" onChange={
handleChange} value={confirm}/>
                </FormGroup>
                <Row className="alinea-centro">
                    <Button className="buttonStyle" variant="outlined" size="medium" type="submit" value="Sign In">Sign In</Button>
                </Row>
            </Form >
            <Row className="alinea-centro">
                <Button className="buttonStyle" variant="outlined" href="/" size="medium" value="Sign In">Return to sign In</Button>
            </Row>
            <br/>
            <br/>
        </Paper>
    );
};

export default Signup;