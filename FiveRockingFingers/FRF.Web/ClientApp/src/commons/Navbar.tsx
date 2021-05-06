import { AppBar, Box, makeStyles, Toolbar, Typography } from '@material-ui/core';
import * as React from 'react';
import { LinkProps as RouterLinkProps, NavLink, Link } from 'react-router-dom';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import { Omit } from '@material-ui/types';

const useStyles = makeStyles(theme => ({
    offset: theme.mixins.toolbar,
    home: {
        textDecoration: 'none',
        '&:active': {
            textDecoration: 'none',
        },
        '&:focus': {
            textDecoration: 'none',
        },
    },
    title: {
        flexGrow: 1,
        display: 'inline-flex',
    },
    appBar: {
        backgroundColor: "#1d1d1d"
    },
    navItem: {
        display: 'flex',
        alignItems: 'flex-end',
        color: 'white',
        margin: '10px',
        textDecoration: 'none',
        '&:hover': {
            textDecoration: 'none',
            color: 'white'
        }
    }
}));

const FaHome = () => (
    <HomeOutlinedIcon />
);

interface navBarProps {
    userName: string | null;
    logoutComponent: React.ReactNode | null;
}

const LinkBehavior = React.forwardRef<any, Omit<RouterLinkProps, 'to'>>((props, ref) => (
    <NavLink to="/" {...props} style={{ color: 'inherit', textDecoration: 'inherit' }} />
));

const Navbar: React.FC<navBarProps> = (props) => {
    const classes = useStyles();
    const { logoutComponent, userName } = props;

    return (
      <AppBar position="relative" className={classes.appBar}>
        <Toolbar variant="dense">
          <Typography className={classes.home} component={LinkBehavior}>
            {FaHome()}
          </Typography>
          <Typography variant="subtitle1" className={classes.title}>
            <Box display="flex" alignItems="flex-end">
              {userName}
            </Box>
                </Typography>
            <Typography variant="subtitle1" className={classes.navItem}>
                <Box display="flex" alignItems="flex-end">
                    <Link className={classes.navItem} to={`/resources`}>
                            Recursos
                    </Link>
                </Box>
            </Typography>
          <Box display="flex" alignItems="flex-end">
            {logoutComponent}
          </Box>
        </Toolbar>
      </AppBar>
    );
}

export default Navbar