import axios from 'axios';
import { BASE_URL } from '../Constants'

const MODULES_URL = `${BASE_URL}modules`;

class ModuleService {

    static getAll = async () => {
        try {
            return await axios.get(`${MODULES_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async (moduleId: number) => {
        try {
            return await axios.get(`${MODULES_URL}/${moduleId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static save = async (module: any) => {
        try {
            return await axios.post(`${MODULES_URL}`, module);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static update = async (moduleId: number, module: any) => {
        try {
            return await axios.put(`${MODULES_URL}/${moduleId}`, module);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static delete = async (moduleId: number) => {
        try {
            return await axios.delete(`${MODULES_URL}/${moduleId}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ModuleService; 
