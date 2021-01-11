import HomeIcon from '@material-ui/icons/Home';
import axios from 'axios';
import * as React from 'react';
import { Menu, ProSidebar, SidebarContent, SidebarHeader, SidebarFooter } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import Project from '../interfaces/Project';
import '../styles/NavMenu.css';
import NavMenuItem from './NavMenuItem';
import ProjectService from '../services/ProjectService';
import UserProfile from '../interfaces/UserProfile'

const FaHome = () => (
        <HomeIcon />
);

const NavMenu = () => {

    const [projects, setProjects] = React.useState<Project[]>([]);
    const [user, setUser] = React.useState<UserProfile | null>(null);
    React.useEffect(() => {
            getProjects();
            getUser();
        },
        []);

    const getProjects = async () => {

        const response = await ProjectService.getAll();

        setProjects(response.data);
    }
    const getUser = async () => {
        const response = await axios.get("https://localhost:44346/api/User");
        setUser({
            fullName : response.data["fullname"],
            email : response.data["email"],
            userId : response.data["userId"],
            avatar : response.data["avatar"]
        });
    }
    return (
        <ProSidebar>
            <SidebarHeader>
                {FaHome()}
                {user?.fullName}
                <Link to="/"/>
            </SidebarHeader>
            <SidebarContent>
                <Menu>
                    {projects
                        ? projects.map((project) => (
                            <NavMenuItem key={project.id} project={project}/>
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