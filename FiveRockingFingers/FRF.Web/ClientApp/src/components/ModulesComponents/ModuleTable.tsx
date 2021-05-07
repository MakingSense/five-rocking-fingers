import * as React from 'react';
import { Table, Button } from 'reactstrap';
import Module from '../../interfaces/Module';
import ModulesTableRow from './ModulesTableRow';
import SnackbarMessage from '../../commons/SnackbarMessage';
import SnackbarSettings from '../../interfaces/SnackbarSettings';
import ModuleService from '../../services/ModuleService';
import EditModuleDialog from './EditModuleDialog';
import NewModuleDialog from './NewModuleDialog';
import { handleErrorMessage } from '../../commons/Helpers';

const ModulesTable = () => {
    const [modules, setModules] = React.useState<Module[]>([]);
    const [openSnackbar, setOpenSnackbar] = React.useState(false);
    const [snackbarSettings, setSnackbarSettings] = React.useState<SnackbarSettings>({ message: "", severity: undefined });
    const [showNewModuleDialog, setShowNewModuleDialog] = React.useState(false);
    const [showEditModuleDialog, setEditModuleDialog] = React.useState(false);
    const [moduleToEdit, setModuleToEdit] = React.useState<Module | null>(null);
    const [updateList, setUpdateList] = React.useState(true);
    const loading = modules.length === 0;

    const getModules = async () => {
        try {
            const response = await ModuleService.getAll();
            if (response.status === 200) {
                setModules(response.data);
            }
            else {
                handleErrorMessage(
                    response.data,
                    "Hubo un error al cargar los módulos",
                    manageOpenSnackbar,
                    undefined
                );
            }
        }
        catch {
            manageOpenSnackbar({ message: "Hubo un error al cargar los módulos", severity: "error" });
        }
    }

    const openEditModuleDialog = () => {
        setEditModuleDialog(true);
    }

    const closeEditModuleDialog = () => {
        setModuleToEdit(null);
        setEditModuleDialog(false);
    }

    const closeNewModuleDialog = () => {
        setShowNewModuleDialog(false);
    }

    const openNewModuleDialog = () => {
        setShowNewModuleDialog(true);
    }

    React.useEffect(() => {
        getModules();
    }, []);

    React.useEffect(() => {
        if (updateList) {
            getModules();
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
                        <th>Nombre del módulo</th>
                        <th>Descripción</th>
                        <th>Costo sugerido</th>
                        <th >
                            <Button className="mx-3" style={{ minHeight: "32px", width: "19vh" }} color="success" onClick={openNewModuleDialog}>Nuevo recurso</Button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? null : <>{
                        Array.isArray(modules)
                            ? modules.map((Module) => <ModulesTableRow
                                key={Module.id}
                                module={Module}
                                openSnackbar={manageOpenSnackbar}
                                updateList={getModules}
                                setModuleToEdit={setModuleToEdit}
                                openEditModuleDialog={openEditModuleDialog}
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
            <NewModuleDialog
                showNewModuleDialog={showNewModuleDialog}
                closeNewModuleDialog={closeNewModuleDialog}
                updateList={getModules}
                setOpenSnackbar={setOpenSnackbar}
                setSnackbarSettings={setSnackbarSettings}
            />
            {moduleToEdit ?
                <EditModuleDialog
                    showEditModuleDialog={showEditModuleDialog}
                    closeEditModuleDialog={closeEditModuleDialog}
                    setOpenSnackbar={setOpenSnackbar}
                    setSnackbarSettings={setSnackbarSettings}
                    moduleToEdit={moduleToEdit}
                    updateModules={getModules}
                    manageOpenSnackbar={manageOpenSnackbar}
                /> :
                null
            }
        </React.Fragment>
    );
};

export default ModulesTable;