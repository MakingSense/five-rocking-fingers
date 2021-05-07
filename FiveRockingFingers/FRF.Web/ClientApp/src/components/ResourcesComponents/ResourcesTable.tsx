import * as React from 'react';
import { Table, Button } from 'reactstrap';
import Resource from '../../interfaces/Resource';
import ResourcesTableRow from './ResourcesTableRow';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import ResourceService from '../../services/ResourceService';
import EditResourceDialog from './EditResourceDialog';
import NewResourceDialog from './NewResourceDialog';
import { handleErrorMessage } from '../../commons/Helpers';

const ResourcesTable = () => {
    const [resources, setResources] = React.useState<Resource[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewResourceDialog, setShowNewResourceDialog] = React.useState(false);
    const [showEditResourceDialog, setEditResourceDialog] = React.useState(false);
    const [resourceToEdit, setResourceToEdit] = React.useState<Resource | null>(null);
    const [updateList, setUpdateList] = React.useState(true);
    const loading = resources.length === 0;

    const getResources = async () => {
        try {
            const response = await ResourceService.getAll();
            if (response.status === 200) {
                setResources(response.data);
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al cargar los recursos",
                    manageOpenSnackbar,
                    undefined
                );
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los recursos", severity: "error" });
        }
    }

    const openEditResourceDialog = () => {
        setEditResourceDialog(true);
    }

    const closeEditResourceDialog = () => {
        setResourceToEdit(null);
        setEditResourceDialog(false);
    }

    const closeNewResourceDialog = () => {
        setShowNewResourceDialog(false);
    }

    const openNewResourceDialog = () => {
        setShowNewResourceDialog(true);
    }

    React.useEffect(() => {
        getResources();
    }, []);

    React.useEffect(() => {
        if (updateList) {
            getResources();
            setUpdateList(false);
        }
    }, [updateList]);

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    return (
        <React.Fragment>
            <Table striped bordered hover responsive>
                <thead>
                    <tr>
                        <th>Nombre del rol</th>
                        <th>Descripción</th>
                        <th>Salario mensual</th>
                        <th>Capacidad de trabajo</th>
                        <th >
                            <Button className="mx-3" style={{ minHeight: "32px", width: "19vh" }} color="success" onClick={openNewResourceDialog}>Nuevo recurso</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? null : <>{
                        Array.isArray(resources)
                            ? resources.map((resource) => <ResourcesTableRow
                                key={resource.id}
                                resource={resource}
                                openSnackbar={manageOpenSnackbar}
                                updateList={getResources}
                                setResourceToEdit={setResourceToEdit}
                                openEditResourceDialog={openEditResourceDialog}
                            />
                            ) : null}</>
                    }
                </tbody>
            </Table>
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
            <NewResourceDialog
                showNewResourceDialog={showNewResourceDialog}
                closeNewResourceDialog={closeNewResourceDialog}
                updateList={getResources}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
            />
            {resourceToEdit ?
                <EditResourceDialog
                    showEditResourceDialog={showEditResourceDialog}
                    closeEditResourceDialog={closeEditResourceDialog}
                    setOpenSnackbar={setOpenSnackbar}
                    setSnackbarSettings={setSnackbarSettings}
                    resourceToEdit={resourceToEdit}
                    updateResources={getResources}
                    manageOpenSnackbar={manageOpenSnackbar}
                /> :
                null
            }
        </React.Fragment>
    );
};

export default ResourcesTable;