import * as React from 'react';
import Category from '../interfaces/Category';
import Project from '../interfaces/Project';
import { useUserContext } from './auth/contextLib';
import Navbar from './ManageProjectsComponents/Navbar';
import ProjectService from '../services/ProjectService';
import CategoryService from '../services/CategoryService';
import ProjectsList from './ManageProjectsComponents/ProjectsList';

export default function ManageProjects() {

    const [projects, setProjects] = React.useState([] as Project[]);
    const [categories, setCategories] = React.useState([] as Category[]);
    const { isAuthenticated } = useUserContext();
    const getProjectList = async () => {
        const response = await ProjectService.getAll(isAuthenticated);
        setProjects(response.data);
    }

    const getCategoryList = async () => {
        const response = await CategoryService.getAll();
        setCategories(response.data);
    }

    React.useEffect(() => {
        getProjectList();
        getCategoryList();
    }, [])

    return (
        <div className="App">
            <Navbar />
            <ProjectsList projects={projects} categories={categories} updateProjects={getProjectList} updateCategories={getCategoryList}/>
        </div>
    )
}
