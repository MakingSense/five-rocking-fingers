import * as React from 'react';
import { Project } from '../store/Projects';
import { ProSidebar, Menu, MenuItem, SubMenu, SidebarHeader, SidebarFooter, SidebarContent } from 'react-pro-sidebar';
import { Link } from 'react-router-dom';

import Preview from './ProjectPreview';

interface Props {
    project: Project
}

const handleChange = () => {
    
}

const NavMenuItem = (prop: { project: Project }) => {
    return (
        <div>
            <SubMenu title={`Proyecto: ${prop.project.name}`}>
                <SubMenu title="Infraestructura">
                    <MenuItem>
                        <Link to={`/preview/${prop.project.id}`}>
                            Preview
                        </Link>
                    </MenuItem>
                    <MenuItem>Entornos</MenuItem>
                </SubMenu>
                <MenuItem>Equipo</MenuItem>
                <MenuItem>Presupuesto</MenuItem>
            </SubMenu>
        </div>   
        );    
};

export default NavMenuItem;
