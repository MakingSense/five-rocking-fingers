export default interface ArtifactRelation {
    id: string | null;
    artifact1: string;
    artifact2: string;
    setting1: string;
    setting2: string;
    relationTypeId: number;
}