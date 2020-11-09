import {
    Button, Card, CardActions, CardContent,
    Checkbox, Chip, FormControl, FormControlLabel, FormGroup, IconButton, InputAdornment, Paper, TextField, TextFieldProps, Typography
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Category from '../../interfaces/Category';
import Project from '../../interfaces/Project';
import ProjectCategory from '../../interfaces/ProjectCategory';
import UserByProject from '../../interfaces/UserByProject';
import UserPublicProfile from '../../interfaces/UserPublicProfile';
import ProjectService from '../ManageProjectsComponents/ProjectService';
import { ValidateEmail } from "./ValidateEmail";

const useStyles = makeStyles({
    root: {
        minWidth: 275,
    },
    title: {
        fontSize: 14,
    },
    inputF: {
        padding: 2,
        marginTop: 10
    },
    categoryList: {
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

const EditProject = (props: { project: Project, cancelEdit: any, categories: Category[], openSnackbar: Function }) => {
    const classes = useStyles();
    const email = React.useRef<TextFieldProps>(null);
    const [fieldEmail, setFieldEmail] = React.useState<string | null>("")
    const { register, handleSubmit, errors } = useForm();
    const [isValid] = React.useState<boolean>(true);
    const [state, setState] = React.useState({
        name: props.project.name,
        client: props.project.client,
        owner: props.project.owner,
        budget: props.project.budget,
        createdDate: props.project.createdDate,
        id: props.project.id,
        projectCategories: props.project.projectCategories,
        usersByProject: props.project.usersByProject
    });

    const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setState({ ...state, [event.target.id]: event.target.value });
    }

    const emailField = () => {
        setFieldEmail("");
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
        userEmail = ValidateEmail(email.current?.value as string, emailField, props.openSnackbar);
        if (userEmail != null) {
            const response = await ProjectService.searchUser(userEmail);

            switch (response.status) {
                case 200:
                    const User: UserPublicProfile = response.data;
                    const aux: UserByProject = {
                        id: 0,
                        userId: User.userId,
                        projectId: 0,
                    }
                    var auxState = state.usersByProject;
                    auxState.push(aux);
                    setState({ ...state, usersByProject: auxState });
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
        const { name, client, owner, budget, id, createdDate, projectCategories, usersByProject } = state;
        const project = { name, client, owner, budget, id, createdDate, projectCategories, usersByProject }

        const response = await ProjectService.update(id, project);
        if (response.status === 200) {
            props.openSnackbar("Se modific\u00F3 el proyecto de manera correcta", "success");
        } else {
            props.openSnackbar("Ocurri\u00F3 un error al modificar el proyecto", "warning");
        }
        props.cancelEdit();
    }

    return (
        <form onSubmit={handleSubmit(handleConfirm)}>
            <Card className={classes.root}>
                <CardContent>
                    <Typography className={classes.title} color="textSecondary" gutterBottom>
                        Nombre del proyecto
                    </Typography>
                    <Typography variant="h5" component="h2">
                        <TextField
                            inputRef={register({ required: true })}
                            error={errors.name ? true : false}
                            id="name"
                            name="name"
                            label="Nombre del proyecto"
                            defaultValue={props.project.name}
                            helperText="Requerido*"
                            variant="outlined"
                            onChange={handleChange}
                            className={classes.inputF}
                        />
                    </Typography>
                    <br />
                    <Typography className={classes.title} color="textSecondary" gutterBottom>
                        Detalles del proyecto
                    </Typography>
                    <Typography variant="h6" component="h3">
                        <TextField
                            id="client"
                            name="client"
                            label="Cliente"
                            defaultValue={props.project.client}
                            variant="outlined"
                            onChange={handleChange}
                            className={classes.inputF}
                        />
                        <TextField
                            id="owner"
                            name="owner"
                            label="Owner del proyecto"
                            defaultValue={props.project.owner}
                            variant="outlined"
                            onChange={handleChange}
                            className={classes.inputF}
                        />
                        <TextField
                            inputRef={register({ validate: { isValid: value => value == null || parseInt(value, 10) >= 0 } })}
                            error={errors.budget ? true : false}
                            id="budget"
                            name="budget"
                            label="Presupuesto"
                            defaultValue={props.project.budget}
                            helperText="Requerido* (0 o entero positivo)"
                            variant="outlined"
                            onChange={handleChange}
                            type="number"
                            className={classes.inputF}
                            InputProps={{
                                startAdornment: <InputAdornment position="start">$</InputAdornment>,
                            }}
                        />
                    </Typography>
                    <Typography className={classes.title} color="textSecondary" gutterBottom>
                        Categorias
                    </Typography>
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
                    <Typography className={classes.title} color="textSecondary" gutterBottom>
                        Usuarios
                    </Typography>
                    <FormControl component="fieldset">
                        <FormGroup>
                            <Paper component="ul" className={classes.categoryList} >
                                {props.project.usersByProject.map((up) => {
                                    return (
                                        <li key={up.id}>
                                            <Chip label={up.userId} className={classes.chip} />
                                        </li>
                                    )
                                })}
                            </Paper>
                            <span><TextField
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
                                </IconButton></span>
                        </FormGroup>
                    </FormControl>
                </CardContent>
                <CardActions>
                    <Button size="small" type="submit"> Aceptar</Button>
                    <Button size="small" color="secondary" onClick={props.cancelEdit}>Cancelar</Button>
                </CardActions>
            </Card>
        </form>
    );
}

export default EditProject