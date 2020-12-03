import {
    Button, Checkbox, Chip, Dialog, DialogActions, DialogContent, DialogContentText,
    DialogTitle, FormControl, FormControlLabel, FormGroup, IconButton, InputAdornment, Paper, TextField, TextFieldProps
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Category from '../../interfaces/Category';
import ProjectCategory from '../../interfaces/ProjectCategory';
import UserProfile from '../../interfaces/UserProfile';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import ProjectService from '../../services/ProjectService';
import Project from '../../interfaces/Project';
import { ValidateEmail } from "./ValidateEmail";
import { HelperAddUser } from "./HelperAddUser";

const useStyles = makeStyles({
    inputF: {
        padding: 2,
        marginTop: 10
    },
    addUser: {
        display: 'flex',
        flexWrap: 'wrap',
        listStyle: 'none',
        padding: 2,
        margin: 0,
    },
    chip: {
        margin: 2
    }
});

const NewProjectDialog = (props: { create: boolean, categories: Category[], finishCreation: Function, openSnackbar: Function }) => {
    const email = React.useRef<TextFieldProps>(null);
    const [fieldEmail, setFieldEmail] = React.useState<string | null>("")
    const classes = useStyles();

    const { register, handleSubmit, errors } = useForm();

    const [state, setState] = React.useState({
        name: "",
        client: "",
        owner: "",
        budget: -1,
        projectCategories: [] as ProjectCategory[],
        users: [] as UserProfile[]
    });

    const clearState = () => {
        setState({
            name: "",
            client: "",
            owner: "",
            budget: -1,
            projectCategories: [],
            users: []
        });
    }

    const emailField = () => {
        setFieldEmail("");
    }

    const handleCancel = () => {
        clearState();
        props.finishCreation();
    }

    const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setState({ ...state, [event.target.id]: event.target.value });
    }

    const handleDelete = (user: UserProfile) => () => {
        let auxState: UserProfile[] = state.users.filter(c => c.userId !== user.userId);
        setState({ ...state, users: auxState });
        props.openSnackbar("Usuario desvinculado correctamente!", "info");
        return { state };
    };

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
        userEmail = ValidateEmail(email.current?.value as string, emailField, props.openSnackbar);
        if (userEmail != null) {
            const response = await ProjectService.searchUser(userEmail);
            switch (response.status) {
                case 200:
                    let newUserList: UserProfile[] | null;
                    newUserList = HelperAddUser(response.data, state.users, emailField, props.openSnackbar);
                    if (newUserList != null) setState({ ...state, users: newUserList });
                    break;
                case 404:
                    props.openSnackbar("Usuario no encontrado", "warning");
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
        const { name, client, owner, budget, projectCategories, users } = state;
        const project = { name, client, owner, budget, projectCategories, users }
        const response = await ProjectService.save(project as Project);
        if (response.status === 200) {
            props.openSnackbar("Creaci\u00F3n del proyecto exitosa", "success");
        } else {
            props.openSnackbar("Ocurri\u00F3 un error al crear el proyecto", "warning");
        }
        clearState();
        props.finishCreation();
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
                        inputRef={register({ validate: { isValid: value => value == null || parseInt(value, 10) >= 1 } })}
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
                    <FormGroup>
                        <Paper component="ul" className={classes.addUser} >
                            {state.users.map((user,index) => {
                                return (
                                    <li key={index}>
                                        <Chip label={user.email} className={classes.chip} onDelete={handleDelete(user)} />
                                    </li>
                                )
                            })}
                        </Paper>
                        <span><TextField
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
                            </IconButton></span>
                    </FormGroup>
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