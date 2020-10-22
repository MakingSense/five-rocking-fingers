import { Box, Typography } from '@material-ui/core';
import * as React from 'react';
import { MenuItem, SubMenu } from 'react-pro-sidebar';
import Project from '../interfaces/Project';

const NavMenuItem = (props: { project: Project }) => {
    return (
        <div>
            <SubMenu title={`Proyecto: ${props.project.name}`}>
            <MenuItem>Artefactos</MenuItem>
            <MenuItem>Equipo</MenuItem>
            <MenuItem>Presupuesto</MenuItem>
            </SubMenu>
        </div >
    );
};

export default NavMenuItem;
