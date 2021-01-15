import ArtifactRelationDTO from '../../interfaces/ArtifactRelationDTO';
import ArtifactsRelations from '../../interfaces/ArtifactRelation';
import ArtifactService from '../../services/ArtifactService';
import Artifact from '../../interfaces/Artifact';


export async function ArtifactsRelationsMapper (artifactsRelationsDTOList: ArtifactRelationDTO[]) {
    
    let artifactsRelationsList: ArtifactsRelations[] = [];
    artifactsRelationsDTOList.forEach(async element => {
        let artifact1 : Artifact = await ArtifactService.get(parseInt(element.artifact1, 10));
        let artifact2 : Artifact = await ArtifactService.get(parseInt(element.artifact2, 10));
        let artifactsRelation: ArtifactsRelations = {
            artifact1: artifact1,
            artifact2: artifact2,
            setting1: artifact1.settings,
            setting2: artifact2.settings,
            relationTypeId: element.relationTypeId
        };
        artifactsRelationsList.push(artifactsRelation);
        
    });
    artifactsRelationsDTOList.map(relation => {
            let artifactsRelation = {
                artifact1Id: relation.artifact1.id,
                artifact2Id: relation.artifact2.id,
                artifact1Property: relation.setting1.key,
                artifact2Property: relation.setting2.key,
                relationTypeId: relation.relationTypeId
            };
            artifactsRelationsList.push(artifactsRelation);
        });

}