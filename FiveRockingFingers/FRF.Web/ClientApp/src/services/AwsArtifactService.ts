import axios from 'axios'
import { BASE_URL } from '../Constants';

const AWS_ARTIFACTS_PROVIDER_URL = `${BASE_URL}api/AwsArtifactsProvider/`;

export default class AwsArtifactsService {

    static GetNamesAsync = async () => {
        return await axios.get(`${AWS_ARTIFACTS_PROVIDER_URL}GetNames`)
        .then(response => {
            if (response.status === 200) {
                return response;
            }
        }
        ).catch(function (error) {
            if (error.response) {
                return error.response;
            }
        });
    }
}