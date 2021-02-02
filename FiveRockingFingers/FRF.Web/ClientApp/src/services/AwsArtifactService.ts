import axios from 'axios'
import { BASE_URL } from '../Constants';
import KeyValueStringPair from '../interfaces/KeyValueStringPair';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}aws-artifacts-provider`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        try {
            return await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}/names`);
        } catch (error) {
            return error.response ? error.response : error.message;
        }
    }

    static GetAttibutesAsync = async (serviceCode: string) => {
        const response = await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}/attributes?serviceCode=${serviceCode}`);
        return response;
    }

    static GetProductsAsync = async (serviceCode: string, artifactSettings: KeyValueStringPair[]) => {
        const response = await axios.post(`${AWS_ARTIFACTS_PROVIDER_URL}/products?serviceCode=${serviceCode}`, artifactSettings);
        return response;
    }
}