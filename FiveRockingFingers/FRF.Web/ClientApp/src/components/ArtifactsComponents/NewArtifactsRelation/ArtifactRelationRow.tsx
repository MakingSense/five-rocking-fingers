import * as React from 'react';
import ArtifactRelation from '../../../interfaces/ArtifactRelation'
import { Button } from 'reactstrap';
import EditArtifactRelation from './EditArtifactRelation';
import DeleteArtifactsRelation from './DeleteArtifactsRelation';

const ArtifactRelationRow = (props: { artifactRelation: ArtifactRelation, openSnackbar: Function }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const [openEditArtifactRelation, setOpenEditArtifactRelation] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const editRelationClick = () => {
        setOpenEditArtifactRelation(true);
    }

    return (
        <>
            <tr>
                <td>{props.artifactRelation.artifact1Id}</td>
                <td>{props.artifactRelation.artifact2Id}</td>
                <td>{props.artifactRelation.artifact1Property}</td>
                <td>{props.artifactRelation.artifact2Property}</td>
                <td>{props.artifactRelation.relationTypeId}</td>
                <td>
                    <Button color="danger" onClick={deleteButtonClick}>Borrar</Button>
                </td>
                <td>
                    <Button color="warning" onClick={editRelationClick}>Modificar</Button>
                </td>
            </tr>
            <EditArtifactRelation open={openEditArtifactRelation} artifactId={props.artifactRelation.id} artifactRelations={props.artifactRelation} />
        </>
    );
};

export default ArtifactRelationRow;