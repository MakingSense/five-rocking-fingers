import { Box } from '@material-ui/core';
import * as React from 'react';
import { MenuItem, SubMenu } from 'react-pro-sidebar';
import Project from '../interfaces/Project';

const NavMenuItem = (props: { project: Project }) => {
    return (
        <div>
            <SubMenu title={`Proyecto: ${props.project.name}`}>
                <Box><MenuItem active={false}>
                    <p>{`Creador: ${props.project.owner}`}</p>
                    <p>{`Cliente: ${props.project.client}`}</p>
                    <p>{`Presupuesto: $${props.project.budget}`}</p>
                    <p>{`Fecha Inicio: ${props.project.createdDate}`}</p>
                </MenuItem></Box>
            <MenuItem>Artefactos</MenuItem>
            <MenuItem>Equipo</MenuItem>
            <MenuItem>Presupuesto</MenuItem>
            </SubMenu>
        </div >
    );
};

export default NavMenuItem;
