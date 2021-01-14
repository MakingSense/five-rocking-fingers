export default interface ArtifactRelation {
    id: string;
    artifact1Id: string;
    artifact2Id: string;
    artifact1Property: string;
    artifact2Property: string;
    relationTypeId: number;
}