import axios from 'axios'
import { BASE_URL } from '../Constants';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}api/AwsArtifactsProvider/`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        const response = await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetNames`);
        return response;
    }

    static GetAttibutesAsync = async (serviceCode: string) => {
        const response = await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetAttributes?serviceCode=${serviceCode}`);
        return response;
    }

    static GetProductsAsync = async (serviceCode: string, artifactSettings: KeyValueStringPair[]) => {
        const response = await axios.post(`${AWS_ARTIFACTS_PROVIDER_URL}GetProducts?serviceCode=${serviceCode}`, artifactSettings);
        return response;
    }
}