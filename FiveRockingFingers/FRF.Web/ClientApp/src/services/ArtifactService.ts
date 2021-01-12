import axios from 'axios';
import { BASE_URL } from '../Constants'

const ARTIFACTS_URL = `${BASE_URL}Artifacts/`;

class ArtifactService {

    static getAll = async () => {
        try {
            return await axios.get(`${ARTIFACTS_URL}GetAll`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllByProjectId = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}GetAllByProjectId/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}Get/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (artifact: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}Save`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (id: number, artifact: any) => {
        try {
            return await axios.put(`${ARTIFACTS_URL}Update/${id}`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${ARTIFACTS_URL}Delete/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ArtifactService;