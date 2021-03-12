import * as React from 'react';
import { Table, Button } from 'reactstrap';
import Artifact from '../../interfaces/Artifact';
import ArtifactsTableRow from './ArtifactsTableRow';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'
import NewArtifactDialog from '../NewArtifactDialog';
import ArtifactService from '../../services/ArtifactService';
import ProjectService from '../../services/ProjectService';
import ArtifactsTotalPrice from './ArtifactsTotalPrice';
import EditArtifactDialog from './EditArtifactDialog';

const ArtifactsTable = (props: { projectId: number}) => {
    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewArtifactDialog, setShowNewArtifactDialog] = React.useState(false);
    const [showEditArtifactDialog, setEditArtifactDialog] = React.useState(false);
    const [artifactToEdit, setArtifactToEdit] = React.useState<Artifact | null>(null);
    const [price, setPrice] = React.useState<string>('');
    const [projectBudget, setProjectBudget] = React.useState<number>(-1);
    const loading = projectBudget=== -1 || artifacts.length === 0;
    const {projectId} = props;

    const getTotalPrice = async () => {
        try {
          const response = await ProjectService.getBudget(projectId);
          if(response.status === 200){
            setPrice(response.data.toFixed(2));
          }
        } catch {
          setPrice('');
        }
      };

      const getProjectBudget = async () => {
        try {
          const response = await ProjectService.get(projectId);
          if(response.status === 200){
              const{budget} = response.data;
              setProjectBudget(budget);
          }
        } catch {
            setProjectBudget(-1);
        }
      };

    const getArtifacts = async () => {
        try {
            const response = await ArtifactService.getAllByProjectId(projectId);
            if (response.status == 200) {
                setArtifacts(response.data);
                getTotalPrice();
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los artifacts", severity: "error" });
        }
    }

    const openEditArtifactDialog = () => {
        setEditArtifactDialog(true);
    }

    const closeEditArtifactDialog = () => {
        setArtifactToEdit(null);
        setEditArtifactDialog(false);
    }

    const closeNewArtifactDialog = () => {
        setShowNewArtifactDialog(false);
    }

    const openNewArtifactDialog = () => {
        setShowNewArtifactDialog(true);
    }

    React.useEffect(() => {
        getProjectBudget();
        getArtifacts();
    }, [projectId]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    return (
        <React.Fragment>
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Provedor</th>
                        <th>Tipo</th>
                        <th>Precio</th>
                        <th >
                            <Button className="mx-3" style={{ minHeight: "32px", width: "19vh" }} color="success" onClick={openNewArtifactDialog}>Nuevo artefacto</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? null : <>{
                        Array.isArray(artifacts)
                            ? artifacts.map((artifact) => <ArtifactsTableRow
                                key={artifact.id}
                                artifact={artifact}
                                openSnackbar={manageOpenSnackbar}
                                updateList={getArtifacts}
                                setArtifactToEdit={setArtifactToEdit}
                                openEditArtifactDialog={openEditArtifactDialog}
                            />
                            ) : null}
                        <ArtifactsTotalPrice totalPrice={price} projectBudget={projectBudget} /></>
                    }
                </tbody>
            </Table>
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
            <NewArtifactDialog
                showNewArtifactDialog={showNewArtifactDialog}
                closeNewArtifactDialog={closeNewArtifactDialog}
                projectId={projectId}
                updateList={getArtifacts}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
            />
            { artifactToEdit ?
                <EditArtifactDialog
                    showEditArtifactDialog={showEditArtifactDialog}
                    closeEditArtifactDialog={closeEditArtifactDialog}
                    setOpenSnackbar={setOpenSnackbar}
                    setSnackbarSettings={setSnackbarSettings}
                    artifactToEdit={artifactToEdit}
                    updateArtifacts={getArtifacts}
                /> :
                null
            }
        </React.Fragment>
    );
};

export default ArtifactsTable;