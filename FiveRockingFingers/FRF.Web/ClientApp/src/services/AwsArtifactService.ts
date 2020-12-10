import axios from 'axios'
import { BASE_URL } from '../Constants';
import AwsArtifact from '../interfaces/AwsArtifact';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}api/AwsArtifactsProvider/`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        const response = await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetAllNames`);
        return response;
    }
}