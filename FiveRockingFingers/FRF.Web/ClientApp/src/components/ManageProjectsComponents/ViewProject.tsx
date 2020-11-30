import { Button, Card, CardActions, CardContent, Chip, Paper, Typography } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import * as React from 'react';
import Project from '../../interfaces/Project';

const useStyles = makeStyles({
    root: {
        minWidth: 275,
    },
    title: {
        fontSize: 14,
    },
    categoryList: {
        display: 'flex',
        flexWrap: 'wrap',
        listStyle: 'none',
        padding: 2,
        margin: 0,
    },
    chip: {
        margin: 2
    }
});

const ViewProject = (props: { project: Project, changeEdit: any }) => {
    const classes = useStyles();

    return (
        <Card className={classes.root}>
            <CardContent>
                <Typography className={classes.title} color="textSecondary" gutterBottom>
                    Nombre del proyecto
                </Typography>
                <Typography variant="h5" component="h2">
                    {props.project.name}
                </Typography>
                <br />
                <Typography className={classes.title} color="textSecondary" gutterBottom>
                    Detalles del proyecto
                </Typography>
                <Typography variant="h6" component="h3">
                    Cliente: {props.project.client} <br />
                    Owner: {props.project.owner} <br />
                    Presupuesto: {props.project.budget}
                </Typography>
                <Typography className={classes.title} color="textSecondary" gutterBottom>
                    Categorias
                </Typography>
                <Paper component="ul" className={classes.categoryList} >
                    {props.project.projectCategories.map((pc) => {
                        return (
                            <li key={pc.category.id}>
                                <Chip label={pc.category.name} className={classes.chip} />
                            </li>
                        )
                    })}
                </Paper>
                <Typography className={classes.title} color="textSecondary" gutterBottom>
                    Usuarios:
                </Typography>
                <Paper component="ul" className={classes.categoryList} >
                    {props.project.usersByProject.map((up) => {
                        return (
                            <li key={up.id}>
                                <Chip label={up.email} className={classes.chip} />
                            </li>
                        )
                    })}
                </Paper>
            </CardContent>
            <CardActions>
                <Button size="small" onClick={props.changeEdit}>Editar</Button>
            </CardActions>
        </Card>
    );
}

export default ViewProject