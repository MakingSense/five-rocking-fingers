import * as React from 'react';
import { Snackbar } from '@material-ui/core';
import MuiAlert, { AlertProps } from '@material-ui/lab/Alert';

function Alert(props: AlertProps) {
    return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const SnackbarMessage = (props: { message: string, severity: "success" | "info" | "warning" | "error" | undefined, open: boolean, setOpen: Function }) => {

    const handleCloseSnackbar = (event?: React.SyntheticEvent, reason?: string) => {
        if (reason === 'clickaway') {
            return;
        }
        props.setOpen(false);
    };

    return (
        <Snackbar open={props.open} autoHideDuration={4000} onClose={handleCloseSnackbar}>
            <Alert onClose={handleCloseSnackbar} severity={props.severity}>
                {props.message}
            </Alert>
        </Snackbar>
    );
};

export default SnackbarMessage;