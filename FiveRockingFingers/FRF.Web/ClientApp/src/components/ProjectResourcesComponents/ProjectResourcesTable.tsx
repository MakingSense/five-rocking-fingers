import * as React from 'react';
import { Table, Button } from 'reactstrap';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import ProjectResource from '../../interfaces/ProjectResource';
import ProjectResourceService from '../../services/ProjectResourceService';
import ProjectResourcesTableRow from './ProjectResourcesTableRow';
import NewProjectResourceDialog from './NewProjectResourceDialog';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import { handleErrorMessage } from '../../commons/Helpers';
import { LinearProgress } from '@material-ui/core';
import Resource from '../../interfaces/Resource';
import ResourceService from '../../services/ResourceService';

const ResourcesTable = (props: { projectId: number }) => {
    const [projectResources, setProjectResources] = React.useState<ProjectResource[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewProjectResourceDialog, setShowNewProjectResourceDialog] = React.useState(false);
    const [updateList, setUpdateList] = React.useState(true);
    const [loading, setLoading] = React.useState<boolean>(false);
    const { projectId } = props;
    const [resources, setResources] = React.useState<Resource[]>([]);

    const getResources = async () => {
        try {
            const response = await ResourceService.getAll();
            if (response.status == 200) {
                setResources(response.data);
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al cargar los recursos del proyecto",
                    manageOpenSnackbar,
                    undefined
                );
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los recursos del proyecto", severity: "error" });
        }
    }

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const getProjectResources = async () => {
        try {
            const response = await ProjectResourceService.getAllByProjectId(projectId);
            if (response.status == 200) {
                setLoading(false);
                setProjectResources(response.data);
            }
            else {
                setLoading(false);
                handleErrorMessage(
                    response.data,
                    "Hubo un error al cargar los recursos del proyecto",
                    manageOpenSnackbar,
                    undefined
                );
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los recursos del proyecto", severity: "error" });
        }
    }

    React.useEffect(() => {
        setLoading(true);
        getProjectResources();
        setUpdateList(false);
        getResources();
    }, [projectId]);

    React.useEffect(() => {
        if (updateList) {
            setLoading(true);
            getProjectResources();
            setUpdateList(false);
        }
    }, [updateList]);

    const handleOpenNewResource = () => {
        setShowNewProjectResourceDialog(true);
    }

    const handleCloseNewResource = () => {
        setShowNewProjectResourceDialog(false);
    }

    const updateProjectResources = () => {
        setUpdateList(true);
    }

    return (
        <Grid container >
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>Nombre del rol</th>
                        <th>Fecha de inicio</th>
                        <th>Fecha de fin</th>
                        <th>Horas por mes</th>
                        <th>Capacidad de trabajo</th>
                        <th>Salario</th>
                        <th style={{ textAlign: 'center' }}>
                            <Button className="mx-3" style={{ minHeight: "32px", width: "90%" }} color="success" onClick={handleOpenNewResource}>Añadir Recurso</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {loading ?
                        <tr><td colSpan={7}>
                            <LinearProgress variant="query" />
                        </td></tr>
                        : !projectResources?.length && !loading ?
                            <tr><td colSpan={7} style={{ textAlign: 'center' }}>
                                <p className="lead">El proyecto no cuenta con recursos asignados. Puede asignar uno haciendo click en
                                <span>
                                        <Button className="mx-3" style={{ width: "10%" }} color="success" onClick={handleOpenNewResource}>Añadir Recurso</Button>
                                </span>
                                </p>
                            </td></tr> :
                            projectResources.map((projectResource: any, index: number) =>
                                <ProjectResourcesTableRow key={index} projectResource={projectResource} manageOpenSnackbar={manageOpenSnackbar} updateList={updateProjectResources} resources={resources} />
                            )}
                </tbody>
            </Table>
            <NewProjectResourceDialog
                projectId={projectId}
                open={showNewProjectResourceDialog}
                handleClose={handleCloseNewResource}
                manageOpenSnackbar={manageOpenSnackbar}
                updateList={updateProjectResources}
                resources={resources}
            />
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
        </Grid >
    )
}

export default ResourcesTable;