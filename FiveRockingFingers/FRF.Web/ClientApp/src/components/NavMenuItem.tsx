import * as React from 'react';
import { Project } from '../store/Projects';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';

interface Props {
    project: Project
}

const NavMenuItem = (prop: { project: Project }) => {
    return (<MenuItem>{prop.project.name}</MenuItem>);    
};

export default NavMenuItem;
