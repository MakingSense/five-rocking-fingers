import axios from 'axios';
import { BASE_URL } from '../Constants';
import Resource from '../interfaces/Resource';

const RESOURCE_URL = `${BASE_URL}resources`;

class ResourceService {

    static getAll = async () => {
        try {
            return await axios.get(`${RESOURCE_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (id: number) => {
        try {
            return await axios.get(`${RESOURCE_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (resource: Resource) => {
        try {
            return await axios.post(`${RESOURCE_URL}`, resource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (id: number, resource: Resource) => {
        try {
            return await axios.put(`${RESOURCE_URL}/${id}`, resource);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (id: number) => {
        try {
            return await axios.delete(`${RESOURCE_URL}/${id}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ResourceService;