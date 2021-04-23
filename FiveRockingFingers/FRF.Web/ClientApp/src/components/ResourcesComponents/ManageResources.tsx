import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ResourcesTable from './ResourcesTable';

type TParams = { projectId: string }

const resources = [
    {
        Id: 1,
        RoleName: 'Junior',
        SalaryPerMonth: 1000,
        WorkloadCapacity: 50
    },
    {
        Id: 2,
        RoleName: 'SemiSenior',
        SalaryPerMonth: 1500,
        WorkloadCapacity: 80
    }
]

const projectResources = [
    {
        Id: 1,
        ProjectId: 1,
        ResourceId: 2,
        DedicatedHours: 8,
        Resource: resources[1]
    },
    {
        Id: 2,
        ProjectId: 1,
        ResourceId: 1,
        DedicatedHours: 8,
        Resource: resources[0]
    },
    {
        Id: 3,
        ProjectId: 1,
        ResourceId: 1,
        DedicatedHours: 4,
        Resource: resources[0]
    }
]


const ManageResources = ({ match }: RouteComponentProps<TParams>) => {

    const projectId = parseInt(match.params.projectId, 10);

    return (<>
        <ResourcesTable projectId={projectId} projectResources={projectResources} resources={resources}/>
        </>)
}

export default ManageResources;