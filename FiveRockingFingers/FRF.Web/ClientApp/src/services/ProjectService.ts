import axios from 'axios'
import { BASE_URL } from '../Constants';
import Project from '../interfaces/Project';

const PROJECTS_URL = `${BASE_URL}api/Projects/`;

export default class ProjectService {

    static searchUser = async (email: string) => {
        const response = axios.get("https://localhost:44346/api/User/search", {
            params: { email: email }
        })
        return response;
    }

    static save = async (project: Project) => {
        const response = await axios.post(`${PROJECTS_URL}Save`,
            {
                name: project.name,
                owner: project.owner,
                client: project.client,
                budget: project.budget,
                projectCategories: project.projectCategories,
                usersProfile: project.usersProfile.map((parameter) => ({ userId:parameter.userId}))
            });
        return response;
    }

    static update = async (id: number, project: Project) => {
        const response = await axios.put(`${PROJECTS_URL}Update?id=${id}`, {
            name: project.name,
            id: id,
            owner: project.owner,
            client: project.client,
            createdDate: project.createdDate,
            budget: project.budget,
            projectCategories: project.projectCategories,
            usersProfile: project.usersProfile.map((parameter) => ({userId:parameter.userId}))
        });
        return response;
    }

    static delete = async (id: string) => {
        const response = await axios.delete(`${PROJECTS_URL}Delete/${id}`)
        return response;
    }

    static getAll = async (userId: string) => {
        const response = await axios.get(`${PROJECTS_URL}GetAll/${userId}`);
        return response;
    }
}