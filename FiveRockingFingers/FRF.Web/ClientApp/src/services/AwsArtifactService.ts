import axios from 'axios'
import { BASE_URL } from '../Constants';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}api/AwsArtifactsProvider/`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        const response = await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetNames`);
        return response;
    }

    static GetAttibutesAsync = async (serviceCode: string) => {
        const response = await axios.post(`${AWS_ARTIFACTS_PROVIDER_URL}GetAttributes?serviceCode=${serviceCode}`);
        return response;
    }
}