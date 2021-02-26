import { Badge, Button, CssBaseline, Drawer, List, Toolbar, Typography } from '@material-ui/core';
import TuneTwoToneIcon from "@material-ui/icons/TuneTwoTone";
import { makeStyles,withStyles, createStyles } from "@material-ui/core/styles";
import AddIcon from '@material-ui/icons/Add';
import * as React from 'react';
import { SubMenu } from 'react-pro-sidebar';
import NavMenuFilter from '../../commons/NavMenuFilter';
import SnackbarMessage from '../../commons/SnackbarMessage';
import Category from '../../interfaces/Category';
import Project from '../../interfaces/Project';
import ProjectCategory from '../../interfaces/ProjectCategory';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import CategoryService from '../../services/CategoryService';
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
        alignContent: 'center',
        '&:hover': {
            backgroundColor: "#323232",
            color: '#FFF'
        }
    },
    subMenu: {
        listStyleType: "none",
        paddingLeft: "7vh",
        paddingTop: "1vh",
        paddingBottom: "1vh",
        color: "#fafafa",
        backgroundColor: "#212121",
        "& .pro-inner-list-item":{marginLeft: '-9vh'},
    }
}));

const StyledBadge = withStyles(() =>
  createStyles({
    badge: {
      right: -3,
      top: 0,
      fontSize: 13,
      border: `2px solid #f44336`,
      padding: "0 4px",
      color: "#ffebee",
      backgroundColor: "#f44336"
    },
  })
)(Badge);

const ProjectsList = (props: { projects: Project[], categories: Category[], updateProjects: Function, updateCategories: Function, projectsFiltered: Project[],categoriesFilter: Category[],setCategoriesFilter: Function, handleFilterCleaner: Function }) => {

    const classes = useStyles();
    const {projects, categories, updateProjects, updateCategories,projectsFiltered ,categoriesFilter, setCategoriesFilter, handleFilterCleaner} = props;
    const [edit, setEdit] = React.useState(false);
    const [create, setCreate] = React.useState(false);
    const [openDelete, setOpenDelete] = React.useState(false);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [projectToDelete, setProjectToDelete] = React.useState<Project | null>(null);
    const [selectedIndex, setSelectedIndex] = React.useState(-1);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });

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
        const aux = projects.filter(p => p.id === id);
        if (aux.length === 0) {
            setProjectToDelete(null);
        } else {
            setProjectToDelete(aux[0]);
            setOpenDelete(true);
        }
    }

    const fillProjectCategories = async (selectedCategories: Category[]) => {
        let projectCategories = [] as ProjectCategory[];
        for (const category of selectedCategories) {
            let categoryToAdd = categories.find(c => c.name === category.name);
            if (categoryToAdd === undefined) {
                const response = await CategoryService.save(category);
                if (response.status === 200) {
                    let projectCategory: ProjectCategory = { category: response.data };
                    projectCategories.push(projectCategory);
                }
            } else {
                let projectCategory: ProjectCategory = { category: categoryToAdd };
                projectCategories.push(projectCategory);
            }
        };
        return projectCategories;
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

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const handleListElement = () => {
        const hasFilteredProjects = projectsFiltered.length !== 0;
        const hasCategoryFilters = categoriesFilter.length !== 0;

        return !hasFilteredProjects && !hasCategoryFilters ?
            projects.map((project) => (
                <ListElement selected={selectedIndex === project.id} key={project.id} id={project.id} selectProject={selectProject} deleteProject={deleteProject} name={project.name} />
            ))
            :
            !hasFilteredProjects ?
                <Typography align="center" variant="h6" gutterBottom>Categoría sin proyectos</Typography>
                :
                projectsFiltered.map((project) => (
                    <ListElement selected={selectedIndex === project.id} key={project.id} id={project.id} selectProject={selectProject} deleteProject={deleteProject} name={project.name} />
                ));
      };

    return (
        <div className={classes.root}>
            <CssBaseline />
            <Drawer className={classes.drawer} variant="permanent" classes={{ paper: classes.drawerPaper }}>
                <Toolbar />
                <div className={classes.drawerContainer}>
                    <Button size="large" variant="contained" className={classes.button} fullWidth={true} endIcon={<AddIcon />} onClick={createProject}>
                        Nuevo Proyecto
                    </Button>
                    <NewProjectDialog
                        create={create}
                        finishCreation={finishCreation}
                        categories={categories}
                        openSnackbar={manageOpenSnackbar}
                        updateProjects={updateProjects}
                        updateCategories={updateCategories}
                        fillProjectCategories={fillProjectCategories} />
                    <SubMenu
                        className={classes.subMenu}
                        title={"Filtros"}
                        icon={
                            <StyledBadge badgeContent={categoriesFilter.length}>
                                <TuneTwoToneIcon />
                            </StyledBadge>
                        }
                    >
                        <NavMenuFilter
                            categories={categories}
                            setCategoriesFilter={setCategoriesFilter}
                            cleaner={handleFilterCleaner}
                        />
                    </SubMenu>
                    <List>
                        {Array.isArray(projects) ? handleListElement() : "No hay projectos"}
                        <ConfirmationDialog keepMounted open={openDelete} onClose={handleClose} project={projectToDelete} resetView={resetView} openSnackbar={manageOpenSnackbar} updateProjects={updateProjects} />
                    </List>
                </div>
            </Drawer>
            <main className={classes.content}>
                {
                    selectedIndex === -1 ?
                        (<h1>Seleccione un proyecto para ver sus detalles</h1>)
                        : projects.filter(p => p.id === selectedIndex)[0] ?
                            (edit ?
                                (<EditProject
                                    project={projects.filter(p => p.id === selectedIndex)[0]}
                                    cancelEdit={cancelEdit}
                                    categories={categories}
                                    openSnackbar={manageOpenSnackbar}
                                    updateProjects={updateProjects}
                                    updateCategories={updateCategories}
                                    fillProjectCategories={fillProjectCategories} />)
                                : (<ViewProject project={projects.filter(p => p.id === selectedIndex)[0]} changeEdit={changeEdit} />))
                            : setSelectedIndex(-1)
                }
                <SnackbarMessage
                    message={snackbarSettings.message}
                    severity={snackbarSettings.severity}
                    open={openSnackbar}
                    setOpen={setOpenSnackbar}
                />
            </main>
        </div>
    )
}

export default ProjectsList