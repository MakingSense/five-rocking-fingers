import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import 'react-pro-sidebar/dist/css/styles.css';
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome } from '@fortawesome/free-solid-svg-icons'
import './NavMenu.css';
import axios from 'axios';

import NavMenuItem from './NavMenuItem';
import { type } from 'os';

type Project = {
    id: number;
    name: string;
    owner: string;
    client: string;
    budget: number;
    createdDate: Date;
    modifiedDate: Date;
    projectCategories: ProjectCategory[];
}

type ProjectCategory = {
    projectCategory: Category;
}

type Category = {
    id: number;
    name: string;
    description: string;
}



const NavMenu = (props: { projects: Project[] }) => {

    return (
        <ProSidebar>
            <SidebarHeader>
                FiveRockingFingers
                    <Link to="/" />
            </SidebarHeader>
            <SidebarContent>
                <Menu>
                    {props.projects ? props.projects.map((project) => (
                        <NavMenuItem key={project.id} project={project} />
                    )) : "No hay projectos"}
                </Menu>                
            </SidebarContent>
        </ProSidebar>
    );
};

export default NavMenu;
