import * as React from 'react';
import { RouteComponentProps } from 'react-router';

type TParams = { projectId: string }

const ProjectDashboard = ({ match }: RouteComponentProps<TParams>) => {

    const projectId = parseInt(match.params.projectId, 10);

    return (
        <>
        {
            `Proyecto numero ${projectId}`
        }</>
    );
}

export default ProjectDashboard;