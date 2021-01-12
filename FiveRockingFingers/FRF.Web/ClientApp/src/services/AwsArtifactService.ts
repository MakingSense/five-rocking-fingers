import axios from 'axios'
import { BASE_URL } from '../Constants';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}AwsArtifactsProvider/`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        try {
            return await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetNames`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    }
}