import React from 'react';
import { Form, FormGroup, Label, Row } from 'reactstrap';
import { Paper, Button, TextField } from '@material-ui/core';
import { useHistory } from 'react-router-dom';
import { useAppContext } from "../libs/contextLib";
import axios from 'axios'
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from "yup";

import "./authStyle.css"
import { LoadingButton } from "../components/LoadingButton"
import { SnackbarError } from "../components/SnackbarError"

interface userSignUp {
    firstName: string;
    familyName: string;
    email: string;
    password: string;
    confirm: string;
}

const UserSignupSchema = yup.object().shape({
    firstName: yup.string()
        .trim()
        .required('Required.'),
    familyName: yup.string()
        .trim()
        .required('Required.'),
    email: yup.string()
        .trim()
        .required('Required.').email('Must be a valid email.'),
    password: yup.string()
        .trim()
        .min(8, 'Must be at least 8 characters.')
        .max(20, 'Can be no longer than 20 characters')
        .required('Required.'),
    confirm: yup.string()
        .oneOf([yup.ref('password'), ''], 'Passwords must match')
});

const Signup: React.FC<userSignUp> = ({ }) => {
    const history = useHistory();
    const { userHasAuthenticated } = useAppContext();
    const { register, handleSubmit, errors, reset } = useForm<userSignUp>({ resolver: yupResolver(UserSignupSchema) });
    const [loading, setLoading] = React.useState(false);
    const [errorLogin, setErrorLogin] = React.useState<string>("");

    const onSubmit = (e: userSignUp) => {
        setLoading(true);
        axios.post('https://localhost:44346/api/SignUp',
            {
                confirmPassword: e.confirm,
                name: e.firstName,
                familyName: e.familyName,
                email: e.email,
                password: e.password
            })
            .then(response => {
                if (response.status == 200) {
                    userHasAuthenticated(true);
                    sessionStorage.setItem('currentUser', JSON.stringify(response.data));
                    history.push("/Home");
                }
            })
            .catch(error => {
                if (error.response) {
                    setErrorLogin(error.response.data);
                    setLoading(false);
                }
                else {
                    setErrorLogin("Sign Up Failed!");
                    setLoading(false);
                }
                reset();
            });
    }

    return (
        <Paper className="paperForm" elevation={9}>
            <h2 className="text-center">
                <strong>Sign Up</strong>
            </h2>

            <Form className=" d-flex flex-column" autoComplete="off" noValidate onSubmit={handleSubmit(onSubmit)}>
                <FormGroup className="campo-form">
                    <Label for="firstName">Name</Label>
                    <TextField
                        inputRef={register}
                        type="text"
                        name="firstName"
                        inputProps={{
                            style: {
                                height: '2.188em',
                                padding: '0 14px',
                            },
                        }}
                        error={!!errors.firstName}
                        helperText={errors.firstName ? errors.firstName.message : ''} />
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="familyName">Last name</Label>
                    <TextField
                        inputRef={register}
                        type="text"
                        name="familyName"
                        inputProps={{
                            style: {
                                height: '2.188em',
                                padding: '0 14px',
                            },
                        }}
                        error={!!errors.familyName}
                        helperText={errors.familyName ? errors.familyName.message : ''} />
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="userEmail">Email</Label>
                    <TextField
                        inputRef={register}
                        type="email"
                        name="email"
                        inputProps={{
                            style: {
                                height: '2.188em',
                                padding: '0 14px',
                            },
                        }}
                        error={!!errors.email}
                        helperText={errors.email ? errors.email.message : ''} />
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="userPassword">Password</Label>
                    <TextField
                        inputRef={register}
                        type="password"
                        name="password"
                        inputProps={{
                            style: {
                                height: '2.188em',
                                padding: '0 14px',
                            },
                        }}
                        error={!!errors.password}
                        helperText={errors.password ? errors.password.message : ''} />
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="confirmPassword">Password</Label>
                    <TextField
                        inputRef={register}
                        type="password"
                        name="confirm"
                        inputProps={{
                            style: {
                                height: '2.188em',
                                padding: '0 14px',
                            },
                        }}
                        error={!!errors.confirm}
                        helperText={errors.confirm ? errors.confirm.message : ''} />
                </FormGroup>
                <Row className="alinea-centro">
                    <LoadingButton buttonText="Sign Up" loading={loading} />
                </Row>
            </Form >
            <Row className="alinea-centro">
                <Button className="buttonStyle" variant="outlined" href="/" size="small" value="Sign In">Return to sign In</Button>
            </Row>
            <br />
            <br />
            <SnackbarError error={errorLogin} />
        </Paper>
    );
};

export default Signup;