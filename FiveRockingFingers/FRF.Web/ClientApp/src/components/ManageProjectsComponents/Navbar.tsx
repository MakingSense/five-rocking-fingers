import { AppBar, Button, makeStyles, Toolbar, Typography } from '@material-ui/core';
import * as React from 'react';
import { Link } from 'react-router-dom';

const useStyles = makeStyles(theme => ({
    offset: theme.mixins.toolbar,
    title: {
        flexGrow: 1
    },
    appBar: {
        zIndex: theme.zIndex.drawer + 1,
        backgroundColor: "#212121"
    }
}));

const Navbar = () => {

    const classes = useStyles();

    return (
        <div>
            <AppBar position="fixed" className={classes.appBar}>
                <Toolbar>
                    <Typography variant="h6" className={classes.title}>
                        Administrar proyectos
                    </Typography>
                    <Button variant="text" color="inherit" component={Link} to={"/"}>
                        Volver
                    </Button>
                </Toolbar>
            </AppBar>
            <div className={classes.offset}></div>
        </div>
    )
}

export default Navbar