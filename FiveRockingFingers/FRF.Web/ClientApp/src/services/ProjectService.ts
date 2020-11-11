import axios from 'axios'
import Project from '../interfaces/Project';

export default class ProjectService {

    static searchUser = async (email: string) => {
        const response = axios.get("https://localhost:44346/api/User/search", {
            params: { email: email }
        })
        return response;
    }

    static save = async (project: Project) => {
        console.log("Service: ");
        console.log(project);
        try {
            const response = await axios.post("https://localhost:44346/api/Projects/Save",
            {
                name: project.name,
                owner: project.owner,
                client: project.client,
                budget: project.budget,
                projectCategories: project.projectCategories,
                usersByProject: project.usersByProject
            });
            return response;
        } catch (error) {
            console.log(error.message);
        }
        
        
    }

    static update = async (id: number, project: any) => {
        const response = await axios.put(`https://localhost:44346/api/Projects/Update?id=${id}`, {
            name: project.name,
            id: id,
            owner: project.owner,
            client: project.client,
            createdDate: project.createdDate,
            budget: project.budget,
            projectCategories: project.projectCategories,
            usersByProject: project.usersByProject
        });
        return response;
    }

    static delete = async (id: string) => {
        const response = await axios.delete("https://localhost:44346/api/Projects/Delete/" + id)
        return response;
    }

    static getAll = async (userId: string) => {
        const response = await axios.get("https://localhost:44346/api/Projects/GetAll/" + userId);
        return response;
    }
}