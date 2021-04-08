import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Snackbar } from '@material-ui/core';
import * as React from 'react';
import ArtifactService from '../../services/ArtifactService';
import ArtifactRelation from '../../interfaces/ArtifactRelation';
import RelationCard from './RelationCard';
import { handleErrorMessage } from '../../commons/Helpers';

const DeleteArtifactsRelation = (props: { open: boolean, setOpen: Function, artifactRelationToDelete: ArtifactRelation, openSnackbar: Function, updateList: Function }) => {
    const [relationList, setRelationList] = React.useState<ArtifactRelation[]>([]);
    const handleClose = () => {
        props.setOpen(false);
    };

    const handleConfirm = async () => {
        try {
            const response = await ArtifactService.deleteRelation(props.artifactRelationToDelete.id!)
            if (response.status == 204) {
                props.openSnackbar({ message: "La relacion ha sido borrado con éxito", severity: "success" });
                props.updateList(true);
            }
            else {
              handleErrorMessage(
                response.data,
                "Hubo un error al borrar la relacion",
                props.openSnackbar,
                undefined
              );
            }
        }
        catch {
            props.openSnackbar({ message: "Hubo un error al borrar la relacion", severity: "error" });
        }
        handleClose();
    };

    const handleCancel = () => {
        setRelationList([]);
        handleClose();
    };

    const deleteRelation = (index: number) => {
        let relationListCopy = [...relationList];
        relationListCopy.splice(index, 1);
        setRelationList(relationListCopy);
    }

    return (
        <>
            <Dialog
                open={props.open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"¿Está seguro que desea eliminar la relacion entre los artefactos?"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                            <RelationCard
                                Relation={props.artifactRelationToDelete}
                                deleteRelation={deleteRelation}
                                index={0}
                                isDeletable={false}
                            />
                        <hr />
                    </DialogContentText>
                    <DialogContentText id="alert-dialog-description">
                        ¿ Desea eliminar ?
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleConfirm} color="primary">
                        Confirmar
                    </Button>
                    <Button onClick={handleCancel} color="secondary">
                        Cancelar
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}

export default DeleteArtifactsRelation;