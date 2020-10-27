import Project from './Project';
import ArtifactType from './ArtifactType';

export default interface Artifact {
    id: number;
    name: string;
    provider: string;
    settings: string;
    project: Project;
    artifactType: ArtifactType;
}