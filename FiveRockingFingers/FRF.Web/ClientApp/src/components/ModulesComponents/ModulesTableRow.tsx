import * as React from 'react';
import Module from '../../interfaces/Module';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';

const ModulesTableRow = (props: { module: Module, openSnackbar: Function, updateList: Function, setModuleToEdit: Function, openEditModuleDialog: Function }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const { module, openSnackbar, updateList, setModuleToEdit, openEditModuleDialog } = props;

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const handleModifyModuleClick = () => {
        setModuleToEdit(module);
        openEditModuleDialog();
    }

    return (
        <>
            <tr>
                <td>{module.name}</td>
                <td>{module.description}</td>
                <td>{module.suggestedCost}</td>
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
                        className="mx-3"
                        style={{ minHeight: "32px", width: "20%" }}
                        color="warning"
                        onClick={handleModifyModuleClick}
                    >
                        Modificar
                    </Button>
                </td>
            </tr>
            <ConfirmationDialog
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                moduleToDelete={module}
                openSnackbar={props.openSnackbar}
                updateList={props.updateList}
            />
        </>
    );
};

export default ModulesTableRow;