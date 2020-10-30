import { faHome } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import axios from 'axios';
import * as React from 'react';
import { Menu, ProSidebar, SidebarContent, SidebarHeader, SidebarFooter } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import Project from '../interfaces/Project';
import '../styles/NavMenu.css';
import NavMenuItem from './NavMenuItem';
import { useUserContext } from "../components/auth/contextLib";

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
    }, []);

    const { isAuthenticated } = useUserContext();
    const getProjects = async () => {
       //const response = await axios.get("https://localhost:44346/api/Projects/GetAll");
        console.log(isAuthenticated);
       const response = await axios.get("https://localhost:44346/api/Projects/GetAllByUserId/"+isAuthenticated);
        setProjects(response.data);
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
            <SidebarFooter>
                <Link to="/administrarProyectos">Administrar proyectos</Link>
            </SidebarFooter>
        </ProSidebar>
    );
};

export default NavMenu;
