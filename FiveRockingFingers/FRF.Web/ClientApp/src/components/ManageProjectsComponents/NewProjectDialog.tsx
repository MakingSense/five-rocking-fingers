import {
    Button, Checkbox, Dialog, DialogActions, DialogContent, DialogContentText,
    DialogTitle, FormControl, FormControlLabel, FormGroup, IconButton, InputAdornment, TextField, TextFieldProps
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import axios from 'axios';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import * as yup from "yup";
import Category from '../../interfaces/Category';
import ProjectCategory from '../../interfaces/ProjectCategory';
import UserByProject from '../../interfaces/UserByProject';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import ProjectService from './ProjectService';

const useStyles = makeStyles({
    inputF: {
        padding: 2,
        marginTop: 10
    }
});
const emailSchema = yup.object().shape({
    email: yup.string()
        .trim()
        .email('Debe ser un email valido.'),
});

const NewProjectDialog = (props: { create: boolean, categories: Category[], finishCreation: Function, openSnackbar: Function }) => {
    const email = React.useRef<TextFieldProps>(null);
    const [fieldEmail, setFieldEmail] = React.useState<string | null>("")
    const classes = useStyles();
    const [isValid, setIsValid] = React.useState<boolean>(true);

    const { register, handleSubmit, errors, reset } = useForm();

    const [state, setState] = React.useState({
        name: "",
        client: "",
        owner: "",
        budget: -1,
        projectCategories: [] as ProjectCategory[],
        userByProject: [] as UserByProject[]
    });

    const clearState = () => {
        setState({
            name: "",
            client: "",
            owner: "",
            budget: -1,
            projectCategories: [],
            userByProject: []
        });
    }

    const handleCancel = () => {
        clearState();
        props.finishCreation();
    }

    const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setState({ ...state, [event.target.id]: event.target.value });
    }

    const handleChangeCategory = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.checked) {
            const aux: ProjectCategory = {
                category: {
                    name: event.target.name,
                    id: parseInt(event.target.id),
                    description: ""
                }
            }
            var auxState = state.projectCategories;
            auxState.push(aux);
            setState({ ...state, projectCategories: auxState });
        } else {
            const aux = state.projectCategories.filter(c => c.category.id !== parseInt(event.target.id));
            setState({ ...state, projectCategories: aux });
        }
    }

    const handleAddUser = async () => {
        let userEmail: string | null = "";
        if (typeof email.current?.value === "string") userEmail = email.current?.value;

        if (!emailSchema.isValidSync({ email: userEmail })) {
            setIsValid(false);
            props.openSnackbar("Formato de email invalido!", "warning");
            setFieldEmail("");
        }
        else {
            setIsValid(true);
            const response = await ProjectService.searchUser(userEmail);

            switch (response.status) {
                case 200:
                    const aux: UserByProject = {
                        id: 0,
                        userId: response.data,
                        projectId: 0
                    }
                    var auxState = state.userByProject;
                    auxState.push(aux);
                    setState({ ...state, userByProject: auxState });
                    props.openSnackbar("Usuario asignado correctamente!", "success");
                    setFieldEmail("");
                    break;
                case 404:
                    props.openSnackbar("Usuario no encontrado", "warning");
                    setFieldEmail("");
                    break;
                case 400:
                    props.openSnackbar("Ocurri\u00F3 un error al asignar un usuario", "error");
                    setFieldEmail("");
                    break;
                default:
                    props.openSnackbar("Ocurri\u00F3 un error al asignar un usuario", "error");
                    setFieldEmail("");
                    break;
            }
        }
    }

    const handleConfirm = async () => {
        const { name, client, owner, budget, projectCategories, userByProject } = state;
        const project = { name, client, owner, budget, projectCategories, userByProject }

        const response = await ProjectService.save(project);
        if (response.status === 200) {
            props.openSnackbar("Creaci\u00F3n del proyecto exitosa", "success");
        } else {
            props.openSnackbar("Ocurri\u00F3 un error al crear el proyecto", "warning");
        }
        props.finishCreation();
        clearState();
    }

    return (
        <Dialog
            disableBackdropClick
            disableEscapeKeyDown
            open={props.create}
        >
            <form onSubmit={handleSubmit(handleConfirm)}>
                <DialogTitle>{"Nuevo Proyecto"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Nombre del proyecto:
                    </DialogContentText>
                    <TextField
                        inputRef={register({ required: true })}
                        error={errors.name ? true : false}
                        id="name"
                        name="name"
                        label="Nombre del proyecto"
                        helperText="Requerido*"
                        variant="outlined"
                        onChange={handleChange}
                        className={classes.inputF}
                        fullWidth
                    />
                    <DialogContentText>
                        Detalles del proyecto:
                    </DialogContentText>
                    <TextField
                        id="client"
                        name="client"
                        label="Cliente"
                        variant="outlined"
                        onChange={handleChange}
                        className={classes.inputF}
                        fullWidth
                    />
                    <TextField
                        id="owner"
                        name="owner"
                        label="Owner del proyecto"
                        variant="outlined"
                        onChange={handleChange}
                        className={classes.inputF}
                        fullWidth
                    />
                    <TextField
                        inputRef={register({ validate: { isValid: value => value == null || parseInt(value, 10) >= 0 } })}
                        error={errors.budget ? true : false}
                        id="budget"
                        name="budget"
                        label="Presupuesto"
                        helperText="Requerido* (0 o entero positivo)"
                        variant="outlined"
                        onChange={handleChange}
                        type="number"
                        className={classes.inputF}
                        InputProps={{
                            startAdornment: <InputAdornment position="start">$</InputAdornment>,
                        }}
                        fullWidth
                    />
                    <TextField
                        error={isValid === false}
                        inputRef={email}
                        value={fieldEmail}
                        type="email"
                        id="email"
                        name="email"
                        label="Permitir acceso a:"
                        helperText="Ingrese el email del usuario al que desea otorgarle acceso"
                        variant="outlined"
                        className={classes.inputF}
                        onChange={event => {
                            setFieldEmail(event.target.value);
                        }}
                    />
                    <IconButton type="button" onClick={handleAddUser} >
                        <PersonAddIcon />
                    </IconButton>
                    <DialogContentText>
                        Categorías:
                    </DialogContentText>
                    <FormControl component="fieldset">
                        <FormGroup>
                            {props.categories.map((category: Category) =>
                                <FormControlLabel
                                    key={category.id}
                                    control={
                                        <Checkbox
                                            checked={state.projectCategories.filter(stateC => stateC.category.id === category.id).length > 0}
                                            onChange={handleChangeCategory}
                                            key={category.id}
                                            id={category.id.toString()}
                                            name={category.name}
                                        />}
                                    label={category.name}
                                />
                            )}
                        </FormGroup>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button size="small" type="submit"> Aceptar</Button>
                    <Button size="small" color="secondary" onClick={handleCancel}>Cancelar</Button>
                </DialogActions>
            </form>
        </Dialog>
    )
}
export default NewProjectDialog