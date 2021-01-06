import { yupResolver } from '@hookform/resolvers/yup';
import { Button, Checkbox, FormControlLabel, Paper, TextField } from "@material-ui/core";
import axios from "axios";
import React from "react";
import { useForm } from "react-hook-form";
import { useHistory } from "react-router-dom";
import { Col, Form, FormGroup, Label, Row } from "reactstrap";
import * as yup from "yup";
import { LoadingButton } from '../../commons/LoadingButton';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import "./authStyle.css";
import { useUserContext } from "./contextLib";

interface userLogin {
    email: string;
    password: string;
    rememberMe: boolean;
}

const UserLoginSchema = yup.object().shape({
    email: yup.string()
        .trim()
        .required('Requerido.').email('Debe ser un email valido.'),
    password: yup.string()
        .trim()
        .min(8, 'Debe tener al menos 8 caracteres.')
        .max(20, 'No puede ser mayor a 20 caracteres.')
        .matches(/^(?=.*\d)(?=.*[a-z])(?=.*[@#$%^&+=.\-_*])(?=.*[A-Z]).{8,}$/, 'Debe incluir al menos un numero, un caracter en mayuscula y un simbolo.')
        .required('Requerido.'),
});

const Login: React.FC<userLogin> = () => {
    const history = useHistory();
    const { setCurrentUser } = useUserContext();
    const { register, handleSubmit, errors, reset } = useForm<userLogin>({ resolver: yupResolver(UserLoginSchema) });
    const [loading, setLoading] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [openSnackbar, setOpenSnackbar] = React.useState(false);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const onSumit = (e: userLogin) => {
        setLoading(true);
        axios.post("https://localhost:44346/api/SignIn",
            {
                email: e.email,
                password: e.password,
                rememberMe: e.rememberMe
            })
            .then(response => {
                if (response.status === 200) {
                    setCurrentUser(response.data);
                    history.push("/home");
                }
            })
            .catch(error => {
                if (error.response.status === 400) {
                    manageOpenSnackbar({ message: "Error al iniciar sesion! Correo electrónico o contraseña no válidos.", severity: "error" });
                    setLoading(false);
                    reset();
                }
                else {
                    manageOpenSnackbar({ message: "Error al iniciar sesion!", severity: "error" });
                    setLoading(false);
                    reset();
                }
            });
    };

    return (
        <Paper className="paperForm" elevation={9} id="login">
            <h2 className="contenedor-form text-center">
                <strong>Iniciar sesión</strong>
            </h2>
            <Form className=" d-flex flex-column" id="loginForm" autoComplete="off" noValidate onSubmit={handleSubmit(onSumit)}>
                <FormGroup className="campo-form">
                    <Label for="email" md={4}>Email</Label>
                    <Col sm={9}>
                        <TextField
                            inputRef={register}
                            type="email"
                            name="email"
                            size="small"
                            error={!!errors.email}
                            helperText={errors.email ? errors.email.message : ''} />
                    </Col>
                </FormGroup>
                <FormGroup className="campo-form">
                    <Label for="password" md={4}>Password</Label>
                    <Col sm={9}>
                        <TextField
                            inputRef={register}
                            type="password"
                            name="password"
                            size="small"
                            error={!!errors.password}
                            helperText={errors.password ? errors.password.message : ''} />
                    </Col>
                </FormGroup>
                <FormControlLabel className="alinea-centro"
                    control={
                        <Checkbox
                            inputRef={register({ required: 'This is required' })}
                            color="primary" name="rememberMe"
                        />
                    }
                    label="Recuerdame" />
                <Row className="alinea-centro">
                    <LoadingButton buttonText="Acceder" loading={loading} />
                </Row>
            </Form >
            <Row className="alinea-centro">
                <Button className="buttonStyle" variant="outlined" href="/signup" size="small">Registrarse</Button>
            </Row>
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

export default Login;