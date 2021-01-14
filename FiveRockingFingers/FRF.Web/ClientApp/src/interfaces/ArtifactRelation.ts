import Artifact from './Artifact';
import AwsArtifact from './AwsArtifact';

export default interface ArtifactRelation {
    artifact1: Artifact;
    artifact2: Artifact;
    setting1: AwsArtifact;
    setting2: AwsArtifact;
    relationTypeId: number;
}