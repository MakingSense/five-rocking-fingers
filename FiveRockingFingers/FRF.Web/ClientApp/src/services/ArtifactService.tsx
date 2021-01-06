import axios from 'axios';
import { BASE_URL } from '../Constants'

const ARTIFACTS_URL = `${BASE_URL}api/Artifacts/`

class ArtifactService {

    static getAll = async () => {
        return await axios.get(`${ARTIFACTS_URL}GetAll`).then(response => {
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

    static getAllByProjectId = async (id: number) => {
        return await axios.get(`${ARTIFACTS_URL}GetAllByProjectId/${id}`)
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

    static get = async (id: number) => {
        return await axios.get(`${ARTIFACTS_URL}Get/${id}`)
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

    static save = async (artifact: any) => {
        return await axios.post(`${ARTIFACTS_URL}Save`, artifact)
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

    static update = async (id: number, artifact: any) => {
        return await axios.put(`${ARTIFACTS_URL}Update/${id}`, artifact)
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

    static delete = async (id: number) => {
        return await axios.delete(`${ARTIFACTS_URL}Delete/${id}`)
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
}

export default ArtifactService;