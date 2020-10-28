﻿import { Button, CssBaseline, Divider, Drawer, List, Snackbar, Toolbar } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import AddIcon from '@material-ui/icons/Add';
import * as React from 'react';
import { Alert } from 'reactstrap';
import Category from '../../interfaces/Category';
import Project from '../../interfaces/Project';
import ConfirmationDialog from './ConfirmationDialog';
import EditProject from './EditProject';
import ListElement from './ListElement';
import NewProjectDialog from './NewProjectDialog';
import ViewProject from './ViewProject';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
    root: {
        display: 'flex',
    },
    drawer: {
        width: drawerWidth,
        flexShrink: 0,
    },
    drawerPaper: {
        width: drawerWidth,
    },
    drawerContainer: {
        overflow: 'auto',
    },
    content: {
        flexGrow: 1,
        padding: theme.spacing(3),
    },
    button: {
        background: "#212121",
        color: 'white',
        borderRadius: 0,
        '&:hover': {
            backgroundColor: "#323232",
            color: '#FFF'
        }
    }
}));

const ProjectsList = (props: { projects: Project[], categories: Category[] }) => {

    const classes = useStyles();

    const [edit, setEdit] = React.useState(false);
    const [create, setCreate] = React.useState(false);
    const [openDelete, setOpenDelete] = React.useState(false);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [projectToDelete, setProjectToDelete] = React.useState<Project | null>(null);
    const [selectedIndex, setSelectedIndex] = React.useState(-1);

    const [snackbarMessage, setSnackbarMessage] = React.useState("");
    const [snackbarType, setSnackbarType] = React.useState("");

    const handleClose = () => {
        setOpenDelete(false);
    };

    const selectProject = (id: number) => {
        setEdit(false);
        if (selectedIndex === id) {
            setSelectedIndex(-1);
        } else {
            setSelectedIndex(id);
        }
    }

    const resetView = (id: Number) => {
        if (selectedIndex === id) {
            setSelectedIndex(-1);
            setEdit(false);
        }
    }

    const deleteProject = (id: number) => {
        const aux = props.projects.filter(p => p.id === id);
        if (aux.length === 0) {
            setProjectToDelete(null);
        } else {
            setProjectToDelete(aux[0]);
            setOpenDelete(true);
        }
    }

    // Turn edit on/off
    const changeEdit = () => {
        setEdit(true);
    }

    const cancelEdit = () => {
        setEdit(false);
    }

    // Show dialog to create project
    const createProject = () => {
        setCreate(true);
    }

    const finishCreation = () => {
        setCreate(false);
    }

    // Snackbar open/close
    const handleOpenSnackbar = (message: string, type: string) => {
        setSnackbarMessage(message);
        setSnackbarType(type);
        setOpenSnackbar(true);
    }

    const closeSnackbar = () => {
        setOpenSnackbar(false);
    }

    return (
        <div className={classes.root}>
            <CssBaseline />
            <Drawer className={classes.drawer} variant="permanent" classes={{ paper: classes.drawerPaper }}>
                <Toolbar />
                <div className={classes.drawerContainer}>
                    <Button size="large" variant="contained" className={classes.button} fullWidth={true} endIcon={<AddIcon />} onClick={createProject}>
                        Nuevo Proyecto
                    </Button>
                    <NewProjectDialog create={create} finishCreation={finishCreation} categories={props.categories} openSnackbar={handleOpenSnackbar} />
                    <Divider />
                    <List>
                        {props.projects.map((project: Project) =>
                            <ListElement selected={selectedIndex === project.id} key={project.id} id={project.id} selectProject={selectProject} deleteProject={deleteProject} name={project.name} />
                        )}
                        <ConfirmationDialog keepMounted open={openDelete} onClose={handleClose} project={projectToDelete} resetView={resetView} openSnackbar={handleOpenSnackbar} />
                    </List>
                </div>
            </Drawer>
            <main className={classes.content}>
                {
                    selectedIndex === -1 ?
                        (<h1>Seleccione un proyecto para ver sus detalles</h1>)
                        : (edit ?
                            (<EditProject project={props.projects.filter(p => p.id === selectedIndex)[0]} cancelEdit={cancelEdit} categories={props.categories} openSnackbar={handleOpenSnackbar} />)
                            : (<ViewProject project={props.projects.filter(p => p.id === selectedIndex)[0]} changeEdit={changeEdit} />))
                }
                <Snackbar open={openSnackbar} autoHideDuration={4000} onClose={closeSnackbar}>
                    <Alert onClose={closeSnackbar} color={snackbarType}>
                        {snackbarMessage}
                    </Alert>
                </Snackbar>
            </main>
        </div>
    )
}

export default ProjectsList