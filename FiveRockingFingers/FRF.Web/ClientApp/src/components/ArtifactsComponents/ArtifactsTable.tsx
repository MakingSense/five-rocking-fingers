import * as React from 'react';
import { Table, Button } from 'reactstrap';
import Artifact from '../../interfaces/Artifact';
import ArtifactsTableRow from './ArtifactsTableRow';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings'
import ArtifactRelation from '../../interfaces/ArtifactRelation'
import NewArtifactDialog from '../NewArtifactDialog';
import NewArtifactsRelation from '../NewArtifactsRelation';
import ArtifactService from '../../services/ArtifactService';
import ProjectService from '../../services/ProjectService';
import ArtifactsTotalPrice from './ArtifactsTotalPrice';
import ArtifactType from '../../interfaces/ArtifactType';
import EditArtifactDialog from './EditArtifactDialog';

const ArtifactsTable = (props: { projectId: number}) => {
    const [artifacts, setArtifacts] = React.useState<Artifact[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewArtifactDialog, setShowNewArtifactDialog] = React.useState(false);
    const [showNewArtifactsRelation, setShowNewArtifactsRelation] = React.useState(false);
    const [showEditArtifactDialog, setEditArtifactDialog] = React.useState(false);
    const [artifactToEdit, setArtifactToEdit] = React.useState<Artifact | null>(null);
    const [artifactsRelations, setArtifactsRelations] = React.useState<ArtifactRelation[]>([]);
    const [price, setPrice] = React.useState<string>('');
    const [projectBudget, setProjectBudget] = React.useState<number>(-1);
    const [updateList, setUpdateList] = React.useState(true);
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

    const getRelations = async () => {
        try {
            const response = await ArtifactService.getAllRelationsByProjectId(projectId);
            if (response.status == 200) {
                setArtifactsRelations(response.data);
            }
            else {
                manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones entre artefactos", severity: "error" });
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar las relaciones entre artefactos", severity: "error" });
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

    const openNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(true);
    }

    const closeNewArtifactsRelation = () => {
        setShowNewArtifactsRelation(false);
    }

    React.useEffect(() => {
        getProjectBudget();
        getArtifacts();
        getRelations();
    }, [projectId]);

    React.useEffect(() => {
        getProjectBudget();
        if (updateList) {
            getRelations();
            getArtifacts();
            setUpdateList(false);
        }
    }, [updateList]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const getRelationsOfAnArtifact = (artifactId: number) => {
        const relations: ArtifactRelation[] = artifactsRelations.filter(relation => relation.artifact1.id === artifactId || relation.artifact2.id === artifactId);

        return relations;
    }

    const handleUpdateList = () => {
        setUpdateList(true);
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
                            <Button className="mx-3" style={{ minHeight: "32px", width: "21%" }} color="success" onClick={openNewArtifactDialog}>Nuevo artefacto</Button>
                            <Button style={{ minHeight: "32px", width: "20%" }} color="success" onClick={openNewArtifactsRelation}>Nueva relación</Button>
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
            <NewArtifactsRelation
                showNewArtifactsRelation={showNewArtifactsRelation}
                closeNewArtifactsRelation={closeNewArtifactsRelation}
                projectId={projectId}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
                artifacts={artifacts}
                artifactsRelations={artifactsRelations}
                updateList={{ update: true, setUpdate: handleUpdateList }}
            />
            { artifactToEdit ?
                <EditArtifactDialog
                    showEditArtifactDialog={showEditArtifactDialog}
                    closeEditArtifactDialog={closeEditArtifactDialog}
                    setOpenSnackbar={setOpenSnackbar}
                    setSnackbarSettings={setSnackbarSettings}
                    artifactToEdit={artifactToEdit}
                    updateArtifacts={getArtifacts}
                    updateRelations={getRelations}
                    artifactsRelations={getRelationsOfAnArtifact(artifactToEdit.id)}
                /> :
                null
            }
        </React.Fragment>
    );
};

export default ArtifactsTable;