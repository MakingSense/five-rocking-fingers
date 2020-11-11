import * as React from 'react';
import Category from '../interfaces/Category';
import Project from '../interfaces/Project';
import { useUserContext } from './auth/contextLib';
import Navbar from './ManageProjectsComponents/Navbar';
import ProjectService from '../services/ProjectService';
import ProjectsList from './ManageProjectsComponents/ProjectsList';

// Categorias de prueba, una vez que este listo el servicio y su API
// deberian reemplazarlas
const mockCategories = [
    {
        id: 1,
        name: "CatNom1",
        description: "CatDesc1"
    },
    {
        id: 2,
        name: "CatNom2",
        description: "CatDesc2"
    },
    {
        id: 3,
        name: "CatNom3",
        description: "CatDesc3"
    }
];

export default function ManageProjects() {

    const [projects, setProjects] = React.useState([] as Project[]);
    const [categories, setCategories] = React.useState([] as Category[]);
    const { isAuthenticated } = useUserContext();
    const getProjectList = async () => {
        const response = await ProjectService.getAll(isAuthenticated);
        setProjects(response.data);
    }

    const getCategoryList = () => {
        setCategories(mockCategories);
    }

    React.useEffect(() => {
        getProjectList();
        getCategoryList();
    }, [])

    return (
        <div className="App">
            <Navbar />
            <ProjectsList projects={projects} categories={categories} />
        </div>
    )
}
