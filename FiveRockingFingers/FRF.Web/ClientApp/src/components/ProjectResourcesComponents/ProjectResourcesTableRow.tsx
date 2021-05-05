import * as React from 'react'
import { Button } from 'reactstrap';
import ProjectResource from '../../interfaces/ProjectResource';
import DeleteProjectResource from './DeleteProjectResource';
import { ToFormatedDate } from '../../commons/Helpers';
import EditProjectResource from './EditProjectResource';
import Resource from '../../interfaces/Resource';

const ProjectResourcesTableRow = (props: { projectResource: ProjectResource, manageOpenSnackbar: Function, updateList: Function, resources: Resource[] }) => {
    const [showEditProjectResource, setShowEditProjectResource] = React.useState(false);
    const { projectResource, manageOpenSnackbar, updateList, resources } = props;

    const [openConfirmDialog, setOpenConfirmDialog] = React.useState(false);

    const openEditDialog = () => {
        setShowEditProjectResource(true);
    }

    const handleCloseEditProjectResource = () => {
        setShowEditProjectResource(false);
    }

    const deleteButtonClick = () => {
        setOpenConfirmDialog(true);
    }

    return (
        <>
            <tr>
                <td>{projectResource.resource.roleName}</td>
                <td>{projectResource.beginDate ? ToFormatedDate(projectResource.beginDate) : '-/-/-'}</td>
                <td>{projectResource.endDate ? ToFormatedDate(projectResource.endDate) : '-/-/-'}</td>
                <td>{projectResource.dedicatedHours}</td>
                <td>{projectResource.resource.workloadCapacity * (projectResource.dedicatedHours / 8)}</td>
                <td>{projectResource.resource.salaryPerMonth * (projectResource.dedicatedHours / 8)}</td>
                <td style={{ display: 'flex' }}>
                    <Button className="mx-3" style={{ minHeight: "32px", width: "45%" }} color="warning" onClick={openEditDialog}>Modificar</Button>
                    <Button className="mx-3" style={{ minHeight: "32px", width: "45%" }} color="danger" onClick={deleteButtonClick}>Desvincular</Button>
                </td>
            </tr>
            <DeleteProjectResource
                open={openConfirmDialog}
                setOpen={setOpenConfirmDialog}
                projectResourceToDelete={projectResource}
                openSnackbar={manageOpenSnackbar}
                updateList={updateList}
            />
            <EditProjectResource
                open={showEditProjectResource}
                handleClose={handleCloseEditProjectResource}
                manageOpenSnackbar={manageOpenSnackbar}
                updateList={updateList}
                projectResource={projectResource}
                resources={resources}
            />
        </>)
}

export default ProjectResourcesTableRow;