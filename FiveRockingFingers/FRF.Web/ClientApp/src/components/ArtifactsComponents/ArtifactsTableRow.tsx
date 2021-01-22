import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';
import { Link } from 'react-router-dom';
import { makeStyles, Theme, createStyles } from '@material-ui/core';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            '& > *': {
                margin: theme.spacing(1),
            },
        },
    }),
);

const ArtifactsTableRow = (props: { artifact: Artifact, openSnackbar: Function, updateList: Function }) => {
    const classes = useStyles();
    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    return (
      <>
        <tr>
          <td>{props.artifact.name}</td>
          <td>{props.artifact.provider}</td>
          <td>{props.artifact.artifactType.name}</td>
          <td className={classes.root}>
            <Button
              style={{ "min-height": "32px", width: "20%" }}
              color="danger"
              onClick={deleteButtonClick}
            >
              Borrar
            </Button>

            <Button
              style={{ "min-height": "32px", width: "20%" }}
              color="info"
              tag={Link}
              to={`/projects/${props.artifact.projectId}/artifacts/${props.artifact.id}`}
            >
              Relaciones
            </Button>
          </td>
        </tr>
        <ConfirmationDialog
          open={openConfirmDialog}
          setOpen={setOpenConfirmDialog}
          artifactToDelete={props.artifact}
          openSnackbar={props.openSnackbar}
          updateList={props.updateList}
        />
      </>
    );
};

export default ArtifactsTableRow;