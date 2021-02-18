import * as React from 'react';
import { Menu, ProSidebar, SidebarContent, SidebarFooter } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import Project from '../interfaces/Project';
import '../styles/NavMenu.css';
import NavMenuItem from './NavMenuItem';
import ProjectService from '../services/ProjectService';

const NavMenu = () => {

    const [projects, setProjects] = React.useState<Project[]>([]);
    React.useEffect(() => {
        getProjects();
    },
        []);

    const getProjects = async () => {

        const response = await ProjectService.getAll();

        setProjects(response.data);
    }
    return (
        <ProSidebar>
            <SidebarContent>
                <Menu>
                    {Array.isArray(projects)
                        ? projects.map((project) => (
                            <NavMenuItem key={project.id} project={project} />
                        ))
                        : "No hay projectos"}
                </Menu>
            </SidebarContent>
            <SidebarFooter>
                <Link to="/administrarProyectos">Administrar proyectos</Link>
            </SidebarFooter>
        </ProSidebar>
    );
};

export default NavMenu;