import * as React from 'react';
import Resource from '../../interfaces/Resource';
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';

const ResourcesTableRow = (props: { resource: Resource, openSnackbar: Function, updateList: Function, setResourceToEdit: Function, openEditResourceDialog: Function }) => {

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);
    const { resource, openSnackbar, updateList, setResourceToEdit, openEditResourceDialog } = props;

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    const handleModifyResourceClick = () => {
        setResourceToEdit(resource);
        openEditResourceDialog();
    }

    return (
        <>
            <tr>
                <td>{resource.roleName}</td>
                <td>{resource.description}</td>
                <td>{resource.salaryPerMonth}</td>
                <td>{resource.workloadCapacity}</td>
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
                        onClick={handleModifyResourceClick}
                    >
                        Modificar
                    </Button>
                </td>
            </tr>
            <ConfirmationDialog
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                resourceToDelete={resource}
                openSnackbar={props.openSnackbar}
                updateList={props.updateList}
            />
        </>
    );
};

export default ResourcesTableRow;