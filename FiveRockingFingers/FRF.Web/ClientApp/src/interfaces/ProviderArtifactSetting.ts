import AwsArtifactSettingName from './AwsArtifactSettingName';

export default interface ProviderArtifactSetting {
    name: AwsArtifactSettingName;
    values: string[];
}