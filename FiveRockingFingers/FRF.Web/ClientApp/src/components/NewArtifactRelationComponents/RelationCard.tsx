import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
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

const RelationCard = (props: { Relation: ArtifactRelation, index: number, deleteRelation: Function, isDeletable: boolean }) => {

    const classes = useStyles();
    const [deletable, setDeletable] = React.useState(props.isDeletable);

    const mapRelationTypeId = (id: number) => {
        switch (+id) {
            case (0):
                return <ArrowForwardIcon />;
            case (1):
                return <ArrowBackIcon />;
            default:
                return null;
        }
    }

    return (
        <div>
            <Typography className={classes.inline} gutterBottom>
                {props.Relation.artifact1.name}: {props.Relation.artifact1Property} {mapRelationTypeId(props.Relation.relationTypeId)} {props.Relation.artifact2.name}: {props.Relation.artifact2Property}
            </Typography>
            {
                deletable ?
                    <IconButton className={classes.inline} onClick={event => props.deleteRelation(props.index)} aria-label="delete" color="secondary">
                        <DeleteIcon />
                    </IconButton> : 
                    <></>
            }        
            </div>
    );
}

export default RelationCard;