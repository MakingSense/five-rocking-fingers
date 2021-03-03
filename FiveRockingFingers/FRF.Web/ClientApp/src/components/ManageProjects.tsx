import * as React from 'react';
import Category from '../interfaces/Category';
import Project from '../interfaces/Project';
import Navbar from './ManageProjectsComponents/Navbar';
import ProjectService from '../services/ProjectService';
import CategoryService from '../services/CategoryService';
import ProjectsList from './ManageProjectsComponents/ProjectsList';

export default function ManageProjects() {

    const [projects, setProjects] = React.useState([] as Project[]);
    const [categories, setCategories] = React.useState([] as Category[]);
    const [projectsFiltered, setProjectsFiltered] = React.useState<Project[]>([]);
    const [categoriesFilter, setCategoriesFilter] = React.useState<Category[]>([]);

    const getProjectList = async () => {
        try {
          const response = await ProjectService.getAll();
          if (response.status === 200) {
            setProjects(response.data);
          }
        } catch {
          setProjects([]);
        }
    }

    const getCategoryList = async () => {
        try {
          const response = await CategoryService.getAll();
          if (response.status === 200) setCategories(response.data);
        } catch {
          setCategories([]);
        }
    }

    React.useEffect(() => {
        getProjectList();
        getCategoryList();
    }, [])

    const filterProjects = () => {
        const filterProjects = projects.filter((p) => {
          return p.projectCategories.some((pc) => {
            return categoriesFilter.some((cf) => {
              return cf.id === pc.category.id;
            });
          });
        });
        setProjectsFiltered(filterProjects);
      };

      const handleUpdateProjectList = () => {
        getProjectList();
        filterProjects();
      };

    React.useEffect(() => {
        filterProjects();
    }, [categoriesFilter,projects,categories]);

    const handleFilterCleaner = () => {
        setCategoriesFilter([]);
        setProjectsFiltered([]);
    };

    return (
        <div className="App">
            <Navbar />
            <ProjectsList projects={projects} categories={categories} updateProjects={handleUpdateProjectList} updateCategories={getCategoryList} projectsFiltered={projectsFiltered} categoriesFilter={categoriesFilter} handleFilterCleaner={handleFilterCleaner} setCategoriesFilter={setCategoriesFilter}/>
        </div>
    )
}
