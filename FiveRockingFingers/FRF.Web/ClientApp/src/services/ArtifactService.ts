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
            return await axios.get(`${BASE_URL}projects/${id}/artifacts`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (artifactId: number) => {
        try {
            return await axios.get(`${ARTIFACTS_URL}/${artifactId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (artifact: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (artifactId: number, artifact: any) => {
        try {
            return await axios.put(`${ARTIFACTS_URL}/${artifactId}`, artifact);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (artifactId: number) => {
        try {
            return await axios.delete(`${ARTIFACTS_URL}/${artifactId}`);
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

    static deleteRelation = async (artifactId: string) => {
        try {
            return await axios.delete(`${BASE_URL}relations/${artifactId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static deleteRelations = async (artifactRelationIds: any) => {
        try {
            return await axios.delete(`${BASE_URL}relations`, { data: artifactRelationIds });
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
    
    static setRelations = async (artifactId: number, artifactRelationsList: any) => {
        try {
            return await axios.post(`${ARTIFACTS_URL}/${artifactId}/relations`, artifactRelationsList);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllRelationsByProjectId = async (projectId: number) => {
        try {
            return await axios.get(`${BASE_URL}projects/${projectId}/relations`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getRelationsAsync = async (artifactId: number) => {
        try {
            return await axios.get(`${BASE_URL}artifacts/${artifactId}/relations`);
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
