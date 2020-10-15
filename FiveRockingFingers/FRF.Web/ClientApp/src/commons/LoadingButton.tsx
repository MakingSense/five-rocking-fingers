import React from "react";
import Fade from "@material-ui/core/Fade";
import Button from "@material-ui/core/Button";
import CircularProgress from "@material-ui/core/CircularProgress";


export const LoadingButton: React.FC<{
    buttonText: string;
    loading: boolean;
}> = (props) => {
    const timerRef = React.useRef<number>();

    React.useEffect(
        () => () => {
            clearTimeout(timerRef.current);
        },
        []
    );

    return (
        <div><Button className="buttonStyle" variant="outlined" size="medium" type="submit">
            <Fade
                in={props.loading}
                style={{
                    transitionDelay: props.loading ? "200ms" : "0ms"
                }}
                unmountOnExit
            >
                <CircularProgress size={25} />
            </Fade>
            {(props.loading) ? '' : (props.buttonText)}
        </Button>
        </div>
    );
}