import axios from 'axios'
import { BASE_URL } from '../Constants';
import Project from '../interfaces/Project';

const PROJECTS_URL = `${BASE_URL}projects`;

export default class ProjectService {

    static save = async (project: Project) => {
        try {
            return await axios.post(`${PROJECTS_URL}`,
                {
                    name: project.name,
                    owner: project.owner,
                    client: project.client,
                    budget: project.budget,
                    projectCategories: project.projectCategories,
                    users: project.users.map((parameter) => ({ userId: parameter.userId }))
                });
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (id: number, project: Project) => {
        try {
            return await axios.put(`${PROJECTS_URL}/${id}`,
                {
                    name: project.name,
                    id: id,
                    owner: project.owner,
                    client: project.client,
                    createdDate: project.createdDate,
                    budget: project.budget,
                    projectCategories: project.projectCategories,
                    users: project.users.map((parameter) => ({ userId: parameter.userId }))
                });
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: string) => {
        try {
            return await axios.delete(`${PROJECTS_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAll = async () => {
        try {
            return await axios.get(`${PROJECTS_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${PROJECTS_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getBudget = async (id : number) => {
        try {
            return await axios.get(`${PROJECTS_URL}/${id}/budget`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}