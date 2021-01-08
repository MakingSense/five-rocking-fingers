import axios from 'axios';
import { BASE_URL } from '../Constants'

const ARTIFACTS_URL = `${BASE_URL}api/Artifacts/`

class ArtifactService {

    static getAll = async () => {
        try {
            return await axios.get(`${ARTIFACTS_URL}GetAll`);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }

    static getAllByProjectId = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}GetAllByProjectId/${id}`);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }

    static get = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}Get/${id}`);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }

    static save = async (artifact: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}Save`, artifact);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }

    static update = async (id: number, artifact: any) => {
        try {
            return await axios.put(`${ARTIFACTS_URL}Update/${id}`, artifact);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${ARTIFACTS_URL}Delete/${id}`);
        } catch (error) {
            if (error.response) {
                return error.response;
            }
        }
    }
}

export default ArtifactService;