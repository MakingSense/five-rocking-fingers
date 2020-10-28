import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Button, Paper, TextField } from '@material-ui/core';
import axios from 'axios';
import React from 'react';
import { useForm } from "react-hook-form";
import { useHistory } from 'react-router-dom';
import { Form, FormGroup, Label, Row } from 'reactstrap';
import * as yup from "yup";
import { LoadingButton } from '../../commons/LoadingButton';
import { SnackbarError } from '../../commons/SnackbarError';
import "./authStyle.css";
import { useUserContext } from "./contextLib";


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
        .required('Requerido.'),
    familyName: yup.string()
        .trim()
        .required('Requerido.'),
    email: yup.string()
        .trim()
        .required('Requerido.').email('Debe ser un email valido.'),
    password: yup.string()
        .trim()
        .min(8, 'Debe tener al menos 8 caracteres.')
        .max(20, 'No puede ser mayor a 20 caracteres.')
        .required('Requerido.'),
    confirm: yup.string()
        .oneOf([yup.ref('password'), ''], 'El password no coincide.')
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
        <Paper className="paperForm" elevation={9} id="signup">
            <h2 className="text-center">
                <strong>Registrarse</strong>
            </h2>

            <Form className=" d-flex flex-column" autoComplete="off" noValidate onSubmit={handleSubmit(onSubmit)}>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="firstName">Nombre</Label>
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
                    <Label className="text-center" for="familyName">Apellido</Label>
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
                        helperText={errors.email ? errors.email.message : ''} />
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
                        helperText={errors.password ? errors.password.message : ''} />
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
                        helperText={errors.confirm ? errors.confirm.message : ''} />
                </FormGroup>
                <Row className="alinea-centro">
                    <LoadingButton buttonText="Registrarse" loading={loading} />
                </Row>
            </Form >

            <Box display="flex" alignItems="baseline" justifyContent="space" mr="12.5rem">
                <p style={{ marginRight: "2.5rem" }}>¿Ya tienes una cuenta?</p>
                <Button className="buttonStyle" variant="outlined" href="/" size="small" value="Sign In">Acceder</Button></Box>
            <br />
            <br />
            <SnackbarError error={errorLogin} />
        </Paper>
    );
};

export default Signup;