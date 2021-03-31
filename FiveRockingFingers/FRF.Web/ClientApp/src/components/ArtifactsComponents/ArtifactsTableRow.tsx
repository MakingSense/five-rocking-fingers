import * as React from 'react';
import Artifact from '../../interfaces/Artifact';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';
import { Link } from 'react-router-dom';
import { PROVIDERS } from '../../Constants';

const ArtifactsTableRow = (props: { artifact: Artifact, openSnackbar: Function, updateList: Function, setArtifactToEdit: Function, openEditArtifactDialog: Function, settingTypes: { [key: string]: string }, setSettingTypes: Function   }) => {
    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const handleModifyArtifactClick = () => {
        props.setArtifactToEdit(props.artifact);
        props.setSettingTypes(props.artifact.relationalFields);
        props.openEditArtifactDialog();
    }

    return (
      <>
        <tr>
            <td>{props.artifact.name}</td>
            <td>{props.artifact.artifactType.provider.name}</td>
            <td>{props.artifact.artifactType.name}</td>
            <td>{props.artifact.price}</td>
            <td>                
                <Button
                    className="mx-3" 
                    style={{ minHeight: "32px", width: "20%" }}
                  color="danger"
                  onClick={deleteButtonClick}
                >
                  Borrar
                </Button>

                <Button
                    style={{ minHeight: "32px", width: "20%" }}
                  color="info"
                  tag={Link}
                  to={`/projects/${props.artifact.projectId}/artifacts/${props.artifact.id}`}
                >
                        Relaciones
                </Button>

                {props.artifact.artifactType.name === PROVIDERS[1] ?
                    <Button
                        className="mx-3"
                        style={{ minHeight: "32px", width: "20%" }}
                        color="warning"
                        onClick={handleModifyArtifactClick}
                    >
                        Modificar
                    </Button> :
                    null
                }
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
