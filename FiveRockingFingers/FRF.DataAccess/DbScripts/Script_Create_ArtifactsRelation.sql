use FiveRockingFingers
CREATE TABLE ArtifactsRelation (
    Artifact1Id INT NOT NULL,
    Artifact2Id INT NOT NULL,
    Artifact1Property NVARCHAR(200) NOT NULL,
    Artifact2Property NVARCHAR(200) NOT NULL,
    RelationTypeId INT NOT NULL,
	CONSTRAINT PK_ARTIFACT_RELATION PRIMARY KEY (Artifact1Id, Artifact2Id,Artifact1Property,Artifact2Property),
	CONSTRAINT FK_ARTIFACTS1_RELATION FOREIGN KEY (Artifact1Id) REFERENCES dbo.Artifacts (Id),
	CONSTRAINT FK_ARTIFACTS2_RELATION FOREIGN KEY (Artifact2Id) REFERENCES dbo.Artifacts (Id)
);