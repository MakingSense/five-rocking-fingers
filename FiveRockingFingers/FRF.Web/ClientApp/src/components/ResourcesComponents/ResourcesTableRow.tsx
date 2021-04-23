import * as React from 'react'
import { Button } from 'reactstrap';
import ConfirmationDialog from './ConfirmationDialog';

const ResourcesTableRow = (props: { projectResource: any, openSnackbar: Function, updateList: Function, openEdit: Function }) => {
    const { projectResource } = props;

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);

    const openEditDialog = () => {
        props.openEdit(projectResource.Id);
    }

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    return (
        <>
            <tr>
                <td>{projectResource.Resource.RoleName }</td>
                <td></td>
                <td></td>
                <td>{projectResource.DedicatedHours}</td>
                <td>{projectResource.Resource.WorkloadCapacity * (projectResource.DedicatedHours/8)}</td>
                <td>{projectResource.Resource.SalaryPerMonth * (projectResource.DedicatedHours / 8)}</td>
                <td style={{ display: 'flex' }}>
                    <Button className="mx-3" style={{ minHeight: "32px", width: "45%" }} color="warning" onClick={openEditDialog}>Modificar</Button>
                    <Button className="mx-3" style={{ minHeight: "32px", width: "45%" }} color="danger" onClick={deleteButtonClick}>Eliminar</Button>
                </td>
            </tr>
            <ConfirmationDialog
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                projectResourceToDelete={projectResource}
                openSnackbar={props.openSnackbar}
                updateList={props.updateList}
            />
        </>)
}

export default ResourcesTableRow;