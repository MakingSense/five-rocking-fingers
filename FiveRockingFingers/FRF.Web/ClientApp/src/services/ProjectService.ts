import axios from 'axios'
import { BASE_URL } from '../Constants';
import Project from '../interfaces/Project';

const PROJECTS_URL = `${BASE_URL}api/Projects/`;

export default class ProjectService {

    static searchUser = async (email: string) => {
        return axios.get("https://localhost:44346/api/User/search", {
            params: { email: email }
        })
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static save = async (project: Project) => {
        return axios.post(`${PROJECTS_URL}Save`,
            {
                name: project.name,
                owner: project.owner,
                client: project.client,
                budget: project.budget,
                projectCategories: project.projectCategories,
                users: project.users.map((parameter) => ({ userId:parameter.userId}))
            })
            .then(response => {
                if (response.status === 200) {
                    return response;
                }
            }
            ).catch(function (error) {
                if (error.response) {
                    return error.response;
                }
            });
    }

    static update = async (id: number, project: Project) => {
        return axios.put(`${PROJECTS_URL}Update?id=${id}`, {
            name: project.name,
            id: id,
            owner: project.owner,
            client: project.client,
            createdDate: project.createdDate,
            budget: project.budget,
            projectCategories: project.projectCategories,
            users: project.users.map((parameter) => ({ userId:parameter.userId}))
        })
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static delete = async (id: string) => {
        return axios.delete(`${PROJECTS_URL}Delete/${id}`)
        .then(response => {
            if (response.status === 204) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }

    static getAll = async () => {
        return axios.get(`${PROJECTS_URL}GetAll/`)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }
}