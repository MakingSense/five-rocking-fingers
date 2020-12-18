import {
    Button, Chip, Dialog, DialogActions, DialogContent, DialogContentText,
    DialogTitle, FormGroup, IconButton, InputAdornment, Paper, TextField, TextFieldProps
} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import Autocomplete, { createFilterOptions } from '@material-ui/lab/Autocomplete';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import Category from '../../interfaces/Category';
import Project from '../../interfaces/Project';
import ProjectCategory from '../../interfaces/ProjectCategory';
import UserProfile from '../../interfaces/UserProfile';
import CategoryService from "../../services/CategoryService";
import ProjectService from '../../services/ProjectService';
import { HelperAddUser } from "./HelperAddUser";
import { ValidateEmail } from "./ValidateEmail";

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

const filter = createFilterOptions<Category>();

const NewProjectDialog = (props: { create: boolean, categories: Category[], finishCreation: Function, openSnackbar: Function, updateProjects: Function, updateCategories: Function }) => {

    const email = React.useRef<TextFieldProps>(null);
    const [fieldEmail, setFieldEmail] = React.useState<string | null>("")
    const classes = useStyles();
    const [tempCategories, setTempCategories] = React.useState([...props.categories]);

    const { register, handleSubmit, errors } = useForm();

    const [state, setState] = React.useState({
        name: "",
        client: "",
        owner: "",
        budget: -1,
        users: [] as UserProfile[],
        selectedCategories: [] as Category[]
    });

    React.useEffect(() => {
        setTempCategories([...props.categories]);
    }, [props.categories.length])

    const clearState = () => {
        setState({
            name: "",
            client: "",
            owner: "",
            budget: -1,
            users: [],
            selectedCategories: []
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
        props.openSnackbar({ message: "Usuario desvinculado correctamente!", severity: "info" });
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
                    props.openSnackbar({ message: "Usuario no entontrado", severity: "warning" });
                    setFieldEmail("");
                    break;
                default:
                    props.openSnackbar({ message: "Ocurrió un error al asignar un usuario", severity: "error" });
                    setFieldEmail("");
                    break;
            }
        }
    }

    const fillProjectCategories = async () => {
        let aux = [] as ProjectCategory[];
        for (const category of state.selectedCategories) {
            let categoryToAdd = props.categories.find(c => c.name === category.name);
            if (categoryToAdd === undefined) {
                const response = await CategoryService.save(category);
                if (response.status === 200)
                {
                    let aux2: ProjectCategory = { category: response.data };
                    aux.push(aux2);
                }
            } else {
                let aux2: ProjectCategory = { category: categoryToAdd };
                aux.push(aux2);
            }
        };
        return aux;
    }

    const handleConfirm = async () => {
        var projectCategories = await fillProjectCategories();
        const { name, client, owner, budget, users } = state;
        const project = { name, client, owner, budget, projectCategories, users }
        const response = await ProjectService.save(project as Project);
        if (response.status === 200) {
            props.openSnackbar({ message: "El proyecto ha sido creado con éxito", severity: "success" });
            props.updateProjects();
        } else {
            props.openSnackbar({ message: "Ocurrió un error al crear el proyecto", severity: "error" });
        }
        clearState();
        props.updateCategories();
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
                    <Autocomplete
                        multiple
                        id="tags-standard"
                        options={tempCategories}
                        fullWidth
                        onChange={handleChangeCategories}
                        autoHighlight
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