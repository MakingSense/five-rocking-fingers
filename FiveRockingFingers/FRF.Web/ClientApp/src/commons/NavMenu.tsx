import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'
import '../styles/NavMenu.css';
import axios from 'axios';

import NavMenuItem from './NavMenuItem';
import Project from '../interfaces/Project';

const FaHome = () => (
    <div className='d-inline-block m-2'>
        <FontAwesomeIcon icon={faHome} />
    </div>
);

const NavMenu = () => {

    const [projects, setProjects] = React.useState<Project[]>([]);

    React.useEffect(() => {
        getProjects();
    }, [projects.length]);

    const getProjects = async () => {
        const response = await axios.get("https://localhost:44346/api/Projects/GetAll");
        setProjects(response.data);
        console.log(response.data);
    }

    return (
        <ProSidebar>
            <SidebarHeader>
                {FaHome()}
                FiveRockingFingers
                    <Link to="/" />
            </SidebarHeader>
            <SidebarContent>
                <Menu>
                    {projects ? projects.map((project) => (
                        <NavMenuItem key={project.id} project={project} />
                    )) : "No hay projectos"}
                </Menu>                
            </SidebarContent>
            <SidebarFooter>
                <Link to="/administrarProyectos">Administrar proyectos</Link>
            </SidebarFooter>
        </ProSidebar>
    );
};

export default NavMenu;
