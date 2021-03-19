import ArtifactType from './ArtifactType';

export default interface Artifact {
    id: number;
    name: string;
    provider: string;
    settings: {[key: string] : string};
    projectId: number;
    artifactType: ArtifactType;
    price: number;
    relationalFields: { [key: string]: string };
}