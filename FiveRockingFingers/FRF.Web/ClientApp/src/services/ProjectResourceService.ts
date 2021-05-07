import axios from 'axios';
import { BASE_URL } from '../Constants';
import ProjectResource from '../interfaces/ProjectResource';

const PROJECTRESOURCE_URL = `${BASE_URL}project-resources`;

class ProjectResourceService {

    static getAll = async () => {
        try {
            return await axios.get(`${PROJECTRESOURCE_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${PROJECTRESOURCE_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllByProjectId = async (id: number) => {
        try {
            return await axios.get(`${BASE_URL}projects/${id}/project-resources`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (projectResource: ProjectResource) => {
        try {
            return await axios.post(`${PROJECTRESOURCE_URL}`, projectResource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (id: number, projectResource: ProjectResource) => {
        try {
            return await axios.put(`${PROJECTRESOURCE_URL}/${id}`, projectResource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${PROJECTRESOURCE_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ProjectResourceService;