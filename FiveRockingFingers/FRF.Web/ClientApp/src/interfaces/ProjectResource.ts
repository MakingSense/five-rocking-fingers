import Resource from './Resource';

export default interface ProjectResource {
    id: number;
    projectId: number;
    resourceId: number;
    beginDate?: Date;
    endDate?: Date;
    dedicatedHours: number;
    resource: Resource;
}