import Alert from '@material-ui/lab/Alert/Alert';
import Snackbar from '@material-ui/core/Snackbar';
import React from "react";

export const SnackbarError: React.FC<{
    error: string;
}> = (props) => {

    const [open, setOpen] = React.useState(true);

    const handleClose = (event?: React.SyntheticEvent, reason?: string) => {
        if (reason === 'clickaway') {
            return;
        }
        setOpen(false);
    };

    return ((props.error) ?
        <Snackbar anchorOrigin={{ vertical: "bottom", horizontal: "center" }} open={open} autoHideDuration={6000} onClose={handleClose} >
            <Alert severity="error">{props.error}</Alert>
        </Snackbar> : null
    );
}