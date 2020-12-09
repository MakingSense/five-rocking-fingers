import { IconButton, ListItem, ListItemSecondaryAction, ListItemText } from '@material-ui/core';
import { Delete } from '@material-ui/icons';
import * as React from 'react';

const ListElement = (props: { selected: boolean, key: Number, id: Number, selectProject: Function, deleteProject: Function, name: String }) => {

    const handleOnClick = () => {
        props.selectProject(props.id);
    }

    const handleDelete = () => {
        props.deleteProject(props.id);
    }

    return (
        <>
            <ListItem button onClick={handleOnClick} selected={props.selected}>
                <ListItemText>{props.name}</ListItemText>
                <ListItemSecondaryAction onClick={handleDelete}>
                    <IconButton edge="end" aria-label="comments">
                        <Delete />
                    </IconButton>
                </ListItemSecondaryAction>
            </ListItem>
        </>
    )
}

export default ListElement