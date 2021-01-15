import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import SyncAltIcon from '@material-ui/icons/SyncAlt';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import ArrowForwardIcon from '@material-ui/icons/ArrowForward';
import { IconButton } from '@material-ui/core';
import DeleteIcon from '@material-ui/icons/Delete';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        inline: {
            display: 'inline'
        }
    }),
);

const RelationCard = (props: { Relation: ArtifactRelation, index: number, deleteRelation: Function }) => {

    const classes = useStyles();

    const mapRelationTypeId = (id: number) => {
        console.log(id);
        switch (+id) {
            case (0):
                return <ArrowForwardIcon />;
            case (1):
                return <ArrowBackIcon />;
            case (2):
                return <SyncAltIcon />;
            default:
                return null;
        }
    }
       
    return (
        <div>
            <Typography className={classes.inline} gutterBottom>
                {props.Relation.artifact1.name}: {props.Relation.setting1.key} {mapRelationTypeId(props.Relation.relationTypeId)} {props.Relation.artifact2.name}: {props.Relation.setting2.key}
            </Typography>
            <IconButton className={classes.inline} onClick={event => props.deleteRelation(props.index)} aria-label="delete" color="secondary">
                <DeleteIcon />
            </IconButton>
        </div>
    );
}

export default RelationCard;