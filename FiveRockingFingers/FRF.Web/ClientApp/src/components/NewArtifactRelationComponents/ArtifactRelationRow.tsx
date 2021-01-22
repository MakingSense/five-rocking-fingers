import * as React from 'react';
import ArtifactRelation from '../../interfaces/ArtifactRelation'
import { Button } from 'reactstrap';
import EditArtifactRelation from './EditArtifactRelation';
import SyncAltIcon from '@material-ui/icons/SyncAlt';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import Artifact from '../../interfaces/Artifact';
import DeleteArtifactsRelation from './DeleteArtifactsRelation';
import { createStyles, makeStyles, Theme } from '@material-ui/core';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            '& > *': {
                margin: theme.spacing(1),
            },
        },
    }),
);

const ArtifactRelationRow = (props: { artifactRelation: ArtifactRelation, artifactId: number, openSnackbar: Function, artifacts: Artifact[], updateList: Function, artifactsRelations: ArtifactRelation[]}) => {

    const classes = useStyles();
    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const [openEditArtifactRelation, setOpenEditArtifactRelation] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const editRelationClick = () => {
        setOpenEditArtifactRelation(true);
    }

    const closeEditArtifactsRelation = () => {
        setOpenEditArtifactRelation(false);
    }

    const relationType = (id: number) => {
        switch (props.artifactRelation.relationTypeId) {
            case 0:
                return <ArrowForwardIcon />
            case 1:
                return <ArrowBackIcon />
            case 2:
                return <SyncAltIcon />
            default:
                return <ArrowForwardIcon />
        };
    }

    return (
        <>
            <tr key={props.artifactRelation.id}>
                <td>{props.artifactRelation.artifact1.name}</td>
                <td>{props.artifactRelation.artifact1Property}</td>
                <td>{relationType(props.artifactRelation.relationTypeId)}</td>
                <td>{props.artifactRelation.artifact2.name}</td>
                <td>{props.artifactRelation.artifact2Property}</td>
                <td className={classes.root}>
                    <Button style={{ "min-height": "32px", width: "37%" }} color="danger" onClick={deleteButtonClick}>Borrar</Button>
                    <Button style={{ "min-height": "32px", width: "37%" }} color="warning" onClick={editRelationClick}>Modificar</Button>
                </td>
            </tr>
            <EditArtifactRelation
                open={openEditArtifactRelation}
                closeEditArtifactsRelation={closeEditArtifactsRelation}
                artifactId={props.artifactId}
                artifactRelations={props.artifactRelation}
                openSnackbar={props.openSnackbar}
                artifacts={props.artifacts}
                updateList={props.updateList} 
                artifactsRelations= {props.artifactsRelations}/>

            <DeleteArtifactsRelation
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                artifactRelationToDelete={props.artifactRelation}
                openSnackbar={props.openSnackbar}
                updateList={props.updateList} />
        </>
    );
};
export default ArtifactRelationRow;