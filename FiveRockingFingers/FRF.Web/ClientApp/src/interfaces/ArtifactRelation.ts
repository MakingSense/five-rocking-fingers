import Artifact from './Artifact';

export default interface ArtifactRelation {
    id: string | null;
    artifact1: Artifact;
    artifact2: Artifact;
    artifact1Property: string;
    artifact2Property: string;
    relationTypeId: number;
}