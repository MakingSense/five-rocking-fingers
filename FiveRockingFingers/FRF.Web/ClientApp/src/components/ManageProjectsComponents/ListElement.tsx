import { IconButton, ListItem, ListItemSecondaryAction, ListItemText } from '@material-ui/core';
import { Delete } from '@material-ui/icons';
import * as React from 'react';

const ListElement = (props: { selected: boolean, key: Number, id: Number, selectProject: Function, deleteProject: Function, name: String }) => {
    return (
        <>
            <ListItem button onClick={ () => props.selectProject(props.id) } selected={props.selected}>
                <ListItemText>{props.name}</ListItemText>
                <ListItemSecondaryAction onClick={ () => props.deleteProject(props.id) }>
                    <IconButton edge="end" aria-label="comments">
                        <Delete />
                    </IconButton>
                </ListItemSecondaryAction>
            </ListItem>
        </>
    )
}

export default ListElement