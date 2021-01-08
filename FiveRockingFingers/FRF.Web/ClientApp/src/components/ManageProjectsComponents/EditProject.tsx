import {
    Button, Card, CardActions, CardContent,
    Chip, FormControl, FormGroup, IconButton, InputAdornment, Paper, TextField, TextFieldProps, Typography
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import Autocomplete, { createFilterOptions } from '@material-ui/lab/Autocomplete';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Category from '../../interfaces/Category';
import Project from '../../interfaces/Project';
import UserProfile from '../../interfaces/UserProfile';
import ProjectService from '../../services/ProjectService';
import { HelperAddUser } from './HelperAddUser';
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

const filter = createFilterOptions<Category>();

const EditProject = (props: { project: Project, cancelEdit: any, categories: Category[], openSnackbar: Function, updateProjects: Function, updateCategories: Function, fillProjectCategories: Function }) => {
    const classes = useStyles();
    const email = React.useRef<TextFieldProps>(null);
    const [fieldEmail, setFieldEmail] = React.useState<string | null>("")
    const { register, handleSubmit, errors } = useForm();
    const [isValid] = React.useState<boolean>(true);
    const [tempCategories, setTempCategories] = React.useState([...props.categories]);
    const [state, setState] = React.useState({
        name: props.project.name,
        client: props.project.client,
        owner: props.project.owner,
        budget: props.project.budget,
        createdDate: props.project.createdDate,
        id: props.project.id,
        projectCategories: props.project.projectCategories,
        users: props.project.users,
        selectedCategories: [] as Category[]
    });

    React.useEffect(() => {
        setTempCategories([...props.categories]);
        setState({ ...state, selectedCategories: [] });
        state.projectCategories.forEach(pc => {
            state.selectedCategories.push(pc.category);
        });
    }, [props.categories.length])

    const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        setState({ ...state, [event.target.id]: event.target.value });
    }

    const emailField = () => {
        setFieldEmail("");
    }

    const handleAddUser = async () => {
        let userEmail: string | null = "";
        userEmail = ValidateEmail(email.current?.value as string, emailField, props.openSnackbar);
        if (userEmail != null) {
            const response = await ProjectService.searchUser(userEmail);
            switch (response.status) {
                case 200:
                    let newUsersList: UserProfile[] | null;
                    newUsersList = HelperAddUser(response.data, state.users, emailField, props.openSnackbar,"");
                    if (newUsersList != null) setState({ ...state, users: newUsersList });
                    break;
                case 404:
                    props.openSnackbar({ message: "Usuario no encontrado", severity: "warning" });
                    setFieldEmail("");
                    break;
                default:
                    props.openSnackbar({ message: "Ocurrió un error al asignar un usuario", severity: "error" });
                    setFieldEmail("");
                    break;
            }
        }
    }

    const handleDelete = (user: UserProfile) => () => {
        if (state.users.length > 1) {
            let auxState: UserProfile[] = state.users.filter(c => c.userId !== user.userId);
            setState({ ...state, users: auxState });
            props.openSnackbar({ message: "Usuario desvinculado correctamente!", severity: "info" });
        }
        else props.openSnackbar({ message: "No puede eliminar todos los usuarios de un proyecto!", severity: "error" });
        return { state };
    };

    const handleChangeCategories = (event: React.ChangeEvent<{}>, value: Category[]) => {
        if (value.length === 0) {
            setState({ ...state, selectedCategories: [] });
            return
        }
        if (tempCategories.filter(c => c.name === value[value.length - 1].name).length === 0) {
            let aux = [...tempCategories];
            aux.push(value[value.length - 1]);
            setTempCategories(aux);
        }
        setState({ ...state, selectedCategories: value });
    }

    const handleConfirm = async () => {
        var projectCategories = await props.fillProjectCategories(state.selectedCategories);
        const { name, client, owner, budget, id, createdDate, users } = state;
        const project = { name, client, owner, budget, id, createdDate, projectCategories, users }
        const response = await ProjectService.update(id, project as Project);
        if (response.status === 200) {
            props.openSnackbar({ message: "El proyecto ha sido modificado con éxito", severity: "success" });
            props.updateProjects();
        } else {
            props.openSnackbar({ message: "Ocurrió un error al modificar el proyecto", severity: "error" });
        }
        props.updateCategories();
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
                        <Autocomplete
                            multiple
                            id="tags-standard"
                            options={tempCategories}
                            fullWidth
                            autoHighlight
                            onChange={handleChangeCategories}
                            defaultValue={state.projectCategories.map(pc => pc.category)}
                            filterOptions={(options, params) => {
                                var filtered = filter(options, params);

                                if (params.inputValue !== '' && tempCategories.find(c => c.name === params.inputValue) === undefined) {
                                    filtered.unshift({
                                        id: -1,
                                        name: params.inputValue,
                                        description: ""
                                    });
                                }
                                return filtered;
                            }}
                            getOptionLabel={(option) => {
                                return option.name
                            }}
                            getOptionSelected={(option, value) => option.name === value.name}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    variant="outlined"
                                    label="Categorías"
                                    placeholder="Escriba el nombre de la categoría"
                                />
                            )}
                        />
                    </Typography>
                    <Typography className={classes.title} color="textSecondary" gutterBottom>
                        Usuarios
                    </Typography>
                    <FormControl component="fieldset">
                        <FormGroup>
                            <Paper component="ul" className={classes.categoryList} >
                                {state.users.map((user, index) => {
                                    return (
                                        <li key={index}>
                                            <Chip label={user.email} className={classes.chip} onDelete={handleDelete(user)} />
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