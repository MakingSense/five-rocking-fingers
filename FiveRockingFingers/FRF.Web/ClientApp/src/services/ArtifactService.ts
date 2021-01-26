import axios from 'axios';
import { BASE_URL } from '../Constants'

const ARTIFACTS_URL = `${BASE_URL}artifacts`;

class ArtifactService {

    static getAll = async () => {
        try {
            return await axios.get(`${ARTIFACTS_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllByProjectId = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}/projects/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (artifact: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}/newArtifact`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (id: number, artifact: any) => {
        try {
            return await axios.put(`${ARTIFACTS_URL}/${id}`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${ARTIFACTS_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getRelations = async (artifactId: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}/${artifactId}/relations`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static deleteRelation = async (id: string) => {
        try {
            return await axios.delete(`${ARTIFACTS_URL}/relations/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
    
    static setRelations = async (artifactRelationsList: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}/relations`, artifactRelationsList);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllRelationsByProjectId = async (projectId: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}/relations/projects/${projectId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
    
    static updateArtifactsRelations = async (artifactId: number, artifactRelationsList: any) => {
        try {
            return await axios.put(`${ARTIFACTS_URL}/${artifactId}/relations`, artifactRelationsList);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ArtifactService;
