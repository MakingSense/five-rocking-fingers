import * as React from 'react';
import { MenuItem, SubMenu } from 'react-pro-sidebar';
import Project from '../interfaces/Project';
import { Link } from 'react-router-dom';
import { Box } from '@material-ui/core';
import { ToFormatedDate } from './Helpers';

const NavMenuItem = (props: { project: Project }) => {
    return (
        <div>
            <SubMenu title={`Proyecto: ${props.project.name}`}>
		<Box><MenuItem active={false}>
                    <p>{`Creador: ${props.project.owner}`}</p>
                    <p>{`Cliente: ${props.project.client}`}</p>
                    <p>{`Presupuesto: $${props.project.budget}`}</p>
                    <p>{`Fecha Inicio: ${ToFormatedDate(props.project.createdDate)}`}</p>
                </MenuItem></Box>
                
                <MenuItem>
                    <Link to={`/projects/${props.project.id}/artifacts`}>
                        Artefactos
                    </Link>
                </MenuItem>
                
                <MenuItem>
                    <Link to={`/projects/${props.project.id}/resources`}>
                        Equipo
                    </Link>
                </MenuItem>
                <MenuItem>Presupuesto</MenuItem>
                <MenuItem>
                    <Link to={`/projects/${props.project.id}/resources`}>
                        Recursos
                    </Link>
                </MenuItem>
            </SubMenu>
        </div >
    );
};

export default NavMenuItem;
