import React from "react";
import { Form, FormGroup, Label, Col, Row } from "reactstrap";
import { Paper, Button, FormControlLabel, Checkbox, TextField } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import { useUserContext } from "./contextLib";
import axios from "axios"
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from "yup";
import { LoadingButton } from '../../commons/LoadingButton';
import { SnackbarError } from '../../commons/SnackbarError';
import "./authStyle.css"

interface userLogin {
    email: string;
    password: string;
    rememberMe: boolean;
}

const UserLoginSchema = yup.object().shape({
    email: yup.string()
        .trim()
        .required('Required.').email('Must be a valid email.'),
    password: yup.string()
        .trim()
        .min(8, 'Must be at least 8 characters.')
        .max(20, 'Can be no longer than 20 characters')
        .required('Required.'),
});

const Login: React.FC<userLogin> = () => {
    const history = useHistory();
    const { userHasAuthenticated } = useUserContext();
    const { register, handleSubmit, errors, reset } = useForm<userLogin>({ resolver: yupResolver(UserLoginSchema) });
    const [loading, setLoading] = React.useState(false);
    const [errorLogin, setErrorLogin] = React.useState<string>("");

    const onSumit = (e: userLogin) => {
        console.log(e);
        setLoading(true);
        axios.post("https://localhost:44346/api/SignIn",
            {
                email: e.email,
                password: e.password,
                rememberMe: e.rememberMe
            })
            .then(response => {
                if (response.status === 200) {
                    userHasAuthenticated(response.data);
                    history.push("/home");
                }
                if (response.status === 400) {
                    setErrorLogin("Login Failed! Invalid email or password.");
                    setLoading(false);
                    reset();
                }
            })
            .catch(error => {
                setErrorLogin("Login Failed!");
                setLoading(false);
                reset();
            });
    };

    return (
        <Paper className="paperForm" elevation={9}>
            <h2 className="contenedor-form text-center">
                <strong>Log In</strong>
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
                    label="Remember Me" />
                <Row className="alinea-centro">
                    <LoadingButton buttonText="Sign In" loading={loading} />
                </Row>
            </Form >
            <Row className="alinea-centro">
                <Button className="buttonStyle" variant="outlined" href="/signup" size="small">Sign Up</Button>
            </Row>
            <br />
            <br />
            <SnackbarError error={errorLogin} />
        </Paper>
    );
};

export default Login;