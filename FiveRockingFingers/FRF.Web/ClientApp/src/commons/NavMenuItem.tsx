import * as React from 'react';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import { Link } from 'react-router-dom';
import Project from '../interfaces/Project';

const NavMenuItem = (props: { project: Project }) => {
    return (
        <div>
            <SubMenu title={`Proyecto: ${props.project.name}`}>
                <Link to={`/project/${props.project.id}/artifact/1`}>
                    <MenuItem>
                            Artefactos
                    </MenuItem>
                </Link>
                <MenuItem>Equipo</MenuItem>
                <MenuItem>Presupuesto</MenuItem>
            </SubMenu>
        </div>   
        );    
};

export default NavMenuItem;
