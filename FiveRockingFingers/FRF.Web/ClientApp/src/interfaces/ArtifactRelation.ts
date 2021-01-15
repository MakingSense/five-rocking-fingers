import Artifact from './Artifact';
import AwsArtifact from './AwsArtifact';
import KeyValueStringPair from './KeyValueStringPair';

export default interface ArtifactRelation {
    id: string | null;
    artifact1: Artifact;
    artifact2: Artifact;
    setting1: KeyValueStringPair;
    setting2: KeyValueStringPair;
    relationTypeId: number;
}