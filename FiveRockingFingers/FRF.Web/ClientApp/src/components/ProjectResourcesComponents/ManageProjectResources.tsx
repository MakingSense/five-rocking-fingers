import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ProjectResourcesTable from './ProjectResourcesTable';

type TParams = { projectId: string }

const ManageProjectResources = ({ match }: RouteComponentProps<TParams>) => {

    const projectId = parseInt(match.params.projectId, 10);

    return (
        <ProjectResourcesTable projectId={projectId} />
    )
}

export default ManageProjectResources;