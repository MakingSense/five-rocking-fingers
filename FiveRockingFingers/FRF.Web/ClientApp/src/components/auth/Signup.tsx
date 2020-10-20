import React from 'react';
import { Form, FormGroup, Label, Row } from 'reactstrap';
import { Paper, Button, TextField, Box } from '@material-ui/core';
import { useHistory } from 'react-router-dom';
import { useUserContext } from "./contextLib";
import axios from 'axios'
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from "yup";
import { LoadingButton } from '../../commons/LoadingButton';
import { SnackbarError } from '../../commons/SnackbarError';
import "./authStyle.css"


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

const Signup: React.FC<userSignUp> = ({}) => {
    const history = useHistory();
    const { userHasAuthenticated } = useUserContext();
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
                if (response.status === 200) {
                    userHasAuthenticated(response.data);
                    history.push("/Home");
                }
                if (response.status === 400) {
                    setErrorLogin("Sign Up Failed!");
                    setLoading(false);
                    reset();
                }
            })
            .catch(error => {
                setErrorLogin("Sign Up Failed!");
                setLoading(false);
                reset();
            });
    }

    return (
        <Paper className="paperForm" elevation={9}>
            <h2 className="text-center">
                <strong>Register</strong>
            </h2>

            <Form className=" d-flex flex-column" autoComplete="off" noValidate onSubmit={handleSubmit(onSubmit)}>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="firstName">Name</Label>
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
                        helperText={errors.firstName ? errors.firstName.message : ''}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="familyName">Last name</Label>
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
                        helperText={errors.familyName ? errors.familyName.message : ''}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="userEmail">Email</Label>
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
                        helperText={errors.email ? errors.email.message : ''}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="userPassword">Password</Label>
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
                        helperText={errors.password ? errors.password.message : ''}/>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="confirmPassword">Password</Label>
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
                        helperText={errors.confirm ? errors.confirm.message : ''}/>
                </FormGroup>
                <Row className="alinea-centro">
                    <LoadingButton buttonText="Sign Up" loading={loading}/>
                </Row>
            </Form >

            <Box display="flex" alignItems="baseline" justifyContent="space" mr="12.5rem">
                <p style={{ marginRight: "1rem" }}>Already have an Account?</p>
                <Button className="buttonStyle" variant="outlined" href="/" size="small" value="Sign In">Log in</Button>
            </Box>


            <br/>
            <br/>
            <SnackbarError error={errorLogin}/>
        </Paper>
    );
};

export default Signup;