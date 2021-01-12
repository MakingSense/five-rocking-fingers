import axios from 'axios';
import { BASE_URL } from '../Constants';

const USER_URL = `${BASE_URL}User`;

export default class ProjectService {

    static searchUser = async (email: string) => {
        try {
            return await axios.get(`${USER_URL}/search`,
                {
                    params: { email: email }
                });
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static get = async () => {
        try {
            return await axios.get(`${USER_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static logout = async () => {
        try {
            return await axios.get(`${USER_URL}/logout`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}