import axios from 'axios';
import { BASE_URL } from '../Constants'

const RESOURCES_URL = `${BASE_URL}resources`;

class ResourceService {

    static getAll = async () => {
        try {
            return await axios.get(`${RESOURCES_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (resoruceId: number) => {
        try {
            return await axios.get(`${RESOURCES_URL}/${resoruceId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (resource: any) => {
        try {
            return await axios.post(`${RESOURCES_URL}`, resource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (resoruceId: number, resource: any) => {
        try {
            return await axios.put(`${RESOURCES_URL}/${resoruceId}`, resource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (resoruceId: number) => {
        try {
            return await axios.delete(`${RESOURCES_URL}/${resoruceId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ResourceService;