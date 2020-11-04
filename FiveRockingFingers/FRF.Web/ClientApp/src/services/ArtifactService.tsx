import axios from 'axios';
import { url } from '../Constants'

class ArtifactService {

    static getAll = async () => {
        const response = await axios.get(url + 'api/Artifacts/GetAll');
        return response;
    }

    static getAllByProjectId = async (id: number) => {
        const response = await axios.get(url + "api/Artifacts/GetAllByProjectId/" + id);
        return response;
    }

    static get = async (id: number) => {
        const response = await axios.get(url + 'api/Artifacts/Get/' + id);
        return response;
    }

    static save = async (artifact: any) => {
        const response = await axios.post(url + 'api/Artifacts/Save', artifact);
        return response;
    }

    static update = async (id: number, artifact: any) => {
        const response = await axios.put(url + 'api/Artifacts/Update/' + id, artifact)
        return response;
    }

    static delete = async (id: number) => {
        const response = await axios.delete(url + 'api/Artifacts/Delete/' + id);
        return response;
    }
}

export default ArtifactService;