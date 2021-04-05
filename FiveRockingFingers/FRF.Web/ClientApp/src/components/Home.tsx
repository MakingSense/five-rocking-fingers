import {
  createMuiTheme,
  createStyles,
  Grid,
  makeStyles,
  Theme,
  ThemeProvider,
  Typography,
} from "@material-ui/core";
import { green } from "@material-ui/core/colors";
import * as React from "react";
import { Link } from "react-router-dom";
import { Button } from "reactstrap";
import { useUser } from "../commons/useUser";

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      flexGrow: 1,
    },
  })
);

const theme = createMuiTheme({
  typography: {
    fontFamily: ["Josefin Sans", "sans-serif"].join(","),
  },
  palette: {
    primary: green,
  },
});

const Home = () => {
  const classes = useStyles();
  const { user } = useUser();
  return (
    <ThemeProvider theme={theme}>
      <Grid container spacing={1} justify='center' style={{margin:0}}>
        <Grid item xs={12}  style={{alignSelf:'end', paddingTop:'60px'}}>
          <Typography align='center' variant='h2' >{`Bienvenido`}</Typography>
          <Typography align='center' variant='h3' >{`${user?.fullname}🎉`}</Typography>
        </Grid>
        <hr/>
        <Grid item xs={12} >
          <Typography align='center' variant='h4'>{"Estimemos algunos proyectos"}</Typography>
        </Grid>
        <Grid item xs={12}  style={{alignSelf:'start', flexBasis:'auto'}}>
        <Typography align='center' variant='body2'>Quieres crear un nuevo proyecto?</Typography>
        <Typography align='center' variant='body2'>has click en el siguiente boton:</Typography>
        
          <Button style={{marginTop:'35px'}} color="success" component={Link} to='/administrarProyectos'>Administrar Proyectos</Button>
        </Grid>
        <Grid item xs={12} alignContent='center' style={{marginLeft:'74vh', marginBottom:'33vh'}}>
          
        </Grid>
      </Grid>
    </ThemeProvider>
  );
};
export default Home;
