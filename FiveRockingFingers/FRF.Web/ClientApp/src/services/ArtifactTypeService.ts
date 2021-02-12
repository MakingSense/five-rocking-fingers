import axios from 'axios';
import { BASE_URL } from '../Constants';

const ARTIFACTTYPES_URL = `${BASE_URL}artifact-types`;

class ArtifactTypeService {

    static getAll = async () => {
        try {
            return await axios.get(`${ARTIFACTTYPES_URL}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };

    static getAllByProvider = async (providerName: string) => {
        try {
            return await axios.get(`${ARTIFACTTYPES_URL}/${providerName}`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    };
}

export default ArtifactTypeService;