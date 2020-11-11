import axios from 'axios';
import { BASE_URL } from '../Constants'

const ARTIFACTS_URL = `${BASE_URL}api/Artifacts/`

class ArtifactService {

    static getAll = async () => {
        const response = await axios.get(`${ARTIFACTS_URL}GetAll`);
        return response;
    }

    static getAllByProjectId = async (id: number) => {
        const response = await axios.get(`${ARTIFACTS_URL}GetAllByProjectId/${id}`);
        return response;
    }

    static get = async (id: number) => {
        const response = await axios.get(`${ARTIFACTS_URL}Get/${id}`);
        return response;
    }

    static save = async (artifact: any) => {
        const response = await axios.post(`${ARTIFACTS_URL}Save`, artifact);
        return response;
    }

    static update = async (id: number, artifact: any) => {
        const response = await axios.put(`${ARTIFACTS_URL}Update/${id}`, artifact)
        return response;
    }

    static delete = async (id: number) => {
        const response = await axios.delete(`${ARTIFACTS_URL}Delete/${id}`);
        return response;
    }
}

export default ArtifactService;