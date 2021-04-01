import { yupResolver } from '@hookform/resolvers/yup';
import { Box, Button, Paper, TextField, Tooltip, Typography } from '@material-ui/core';
import axios from 'axios';
import React from 'react';
import { useForm } from "react-hook-form";
import { useHistory } from 'react-router-dom';
import { Form, FormGroup, Label, Row } from 'reactstrap';
import * as yup from "yup";
import { LoadingButton } from '../../commons/LoadingButton';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import "./authStyle.css";
import { useUser } from '../../commons/useUser'
import { BASE_URL } from '../../Constants';

interface userSignUp {
    firstName: string;
    familyName: string;
    email: string;
    password: string;
    confirm: string;
}

const SIGNUP_URL = `${BASE_URL}SignUp`;

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
        .required('Requerido.')
        .matches(/(?=.*[@#$%^&+=.\-_*])/, 'Debe incluir al menos un simbolo distinto de + o =')
        .test("hasPlusOrEqual","No puede incluir los simbolos + o =",(password)=> !/(\+|=)/.test(password!))
        .test("hasAnyNumber","Debe incluir al menos un numero.",(password)=> /(?=.*\d)/.test(password!))
        .test("hasAnyUppercase","Debe incluir al menos un caracter en mayuscula.",(password)=> /(?=.*[A-Z])/.test(password!))
        .test("hasMinLenght",'Debe tener al menos 8 caracteres.',(password)=> /(.{8,}$)/.test(password!)),
    confirm: yup.string()
        .oneOf([yup.ref('password'), ''], 'El password no coincide.')
});

const Signup = ({ }) => {
    const history = useHistory();
    const { storeUser, cleanUserStorage } = useUser();
    const { register, handleSubmit, errors, reset } = useForm<userSignUp>({ mode: 'onChange',
    reValidateMode: 'onChange',resolver: yupResolver(UserSignupSchema) });
    const [loading, setLoading] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [openSnackbar, setOpenSnackbar] = React.useState(false);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const onSubmit = (e: userSignUp) => {
        setLoading(true);
        axios.post(SIGNUP_URL,
            {
                confirmPassword: e.confirm,
                name: e.firstName,
                familyName: e.familyName,
                email: e.email,
                password: e.password
            })
            .then(response => {
                if (response.status === 200) {
                    storeUser(JSON.stringify(response.data), false);
                    history.push("/Home");
                }
            })
            .catch(function (error) {
                if (error.response.status === 400) {
                    cleanUserStorage();
                    manageOpenSnackbar({ message: "Error al registrarse! Verifique que los datos sean correctos.", severity: "error" });
                    setLoading(false);
                    reset();
                }
                else {
                    cleanUserStorage();
                    manageOpenSnackbar({ message: "Error al registrarse! Intente nuevamente más tarde.", severity: "error" });
                    setLoading(false);
                    reset();
                }
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
                    <Tooltip title={
                    <Typography variant="body2">Debe tener al menos 8 caracteres, un numero, un caracter en mayuscula y un simbolo distinto de + o =
                    </Typography>} placement="right" arrow>
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
                        autoComplete="current-password"
                        error={!!errors.password}
                        helperText={errors.password ? errors.password.message : ''} />
                    </Tooltip>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label className="text-center" for="confirmPassword">Confirmar Password</Label>
                    <Tooltip title={
                    <Typography variant="body2">Debe tener al menos 8 caracteres, un numero, un caracter en mayuscula y un simbolo distinto de + o =
                    </Typography>} placement="right" arrow>
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
                    </Tooltip>
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
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
        </Paper>
    );
};

export default Signup;