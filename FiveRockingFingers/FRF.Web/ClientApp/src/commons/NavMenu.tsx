import * as React from 'react';
import { ProSidebar, Menu, SidebarHeader, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'
<<<<<<< HEAD
import '../styles/NavMenu.css';
=======

>>>>>>> origin/FIVE-4
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
    const [user, setUser] = React.useState<string|null>("");
    React.useEffect(() => {
        getProjects();
        getUser();
    }, [projects.length]);

    const getProjects = async () => {
        const response = await axios.get("https://localhost:44346/api/Projects/GetAll");
        setProjects(response.data);
        console.log(response.data);
    }
    const getUser = async () => {
        const response = await axios.get("https://localhost:44346/api/User/getfullname");
        setUser(response.data);
    }
    return (
        <ProSidebar>
            <SidebarHeader>
                {FaHome()}
                {user}
                    <Link to="/" />
            </SidebarHeader>
            <SidebarContent>
                <Menu>
                    {projects ? projects.map((project) => (
                        <NavMenuItem key={project.id} project={project} />
                    )) : "No hay projectos"}
                </Menu>
            </SidebarContent>
        </ProSidebar>
    );
};

export default NavMenu;
