import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'
import './NavMenu.css';
import axios from 'axios';

import NavMenuItem from './NavMenuItem';

import { useSelector, useDispatch } from 'react-redux';
import { actionCreators, ProjectsState, Project } from '../store/Projects';

const FaHome = () => (
    <div>
        <FontAwesomeIcon icon={faHome} />
    </div>
);

interface IProps {
}

interface IProjects {
    projects: undefined[];
}

interface NavMenuProps {
    key: number,
    project: Project
}



const NavMenu  = () => {

    const dispatch = useDispatch();

    React.useEffect(() => {
        const loadProjects = () => dispatch(actionCreators);
        loadProjects();
    }, []);

    const projects = useSelector((state: ProjectsState) => state.projects)

    return (
        <ProSidebar>
            <SidebarHeader>
                FiveRockingFingers
                    <Link to="/" />
            </SidebarHeader>
            <SidebarContent>
                {projects? projects.map((project) => (
                    <NavMenuItem project={project}/>
                )) : "No hay projectos"}
            </SidebarContent>
            <SidebarFooter>
                Hola
                </SidebarFooter>
        </ProSidebar>
    );
};

export default NavMenu;
