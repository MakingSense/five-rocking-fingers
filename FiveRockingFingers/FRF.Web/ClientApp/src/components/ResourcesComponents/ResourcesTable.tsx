import * as React from 'react';
import { Table, Button } from 'reactstrap';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import ResourceTableRow from './ResourcesTableRow';
import NewResourceDialog from './NewResourceDialog';
import EditResource from './EditResource';

const ResourcesTable = (props: { projectId: number, projectResources: any, resources: any }) => {
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewResourceDialog, setShowNewResourceDialog] = React.useState(false);
    const [showEditResource, setShowEditResource] = React.useState(false);

    const handleOpenNewResource = () => {
        setShowNewResourceDialog(true);
    }

    const handleCloseNewResource = () => {
        setShowNewResourceDialog(false);
    }

    const handleOpenEditResource = (id: number) => {
        setShowEditResource(true);
    }

    const handleCloseEditResource = () => {
        setShowEditResource(false);
    }

    const manageOpenSnackbar = (settings: SnackbarSettings) => {
        setSnackbarSettings(settings);
        setOpenSnackbar(true);
    }

    const updateProjectResources = () => {
        return;
    }

    return (
        <>
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
                    {props.projectResources.map((projectResource: any) =>
                        <ResourceTableRow projectResource={projectResource} openSnackbar={manageOpenSnackbar} updateList={updateProjectResources} openEdit={handleOpenEditResource} />
                    )}
                </tbody>
            </Table>
            <NewResourceDialog
                open={showNewResourceDialog}
                handleClose={handleCloseNewResource}
                openSnackbar={manageOpenSnackbar}
                updateList={updateProjectResources}
                resources={props.resources}
            />
            <EditResource
                open={showEditResource}
                handleClose={handleCloseEditResource}
                openSnackbar={manageOpenSnackbar}
                updateList={updateProjectResources}
            />
            <SnackbarMessage
                message={snackbarSettings.message}
                severity={snackbarSettings.severity}
                open={openSnackbar}
                setOpen={setOpenSnackbar}
            />
        </>
        )
}

export default ResourcesTable;