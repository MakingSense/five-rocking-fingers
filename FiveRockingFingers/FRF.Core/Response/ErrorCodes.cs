namespace FRF.Core.Response
{
    public static class ErrorCodes
    {
        // Not exists errors
        public const int ArtifactNotExists = 1;
        public const int ArtifactTypeNotExists = 2;
        public const int CategoryNotExists = 3;
        public const int ProjectNotExists = 4;
        public const int ProviderNotExists = 5;
        public const int ArtifactFromAnotherProject = 6;
        public const int InvalidStartDateForProject = 7;
        public const int ResourceNotExist = 8;
        public const int ProjectResourceNotExists = 9;

        // Relation errors
        public const int RelationAlreadyExisted = 10;
        public const int RelationNotValidDifferentBaseArtifact = 11;
        public const int RelationNotValidRepeated = 12;
        public const int RelationNotValidDifferentType = 13;
        public const int RelationNotExists = 14;
        public const int RelationCycleDetected = 15;

        // User errors
        public const int InvalidCredentials = 20;
        public const int AuthenticationServerCurrentlyUnavailable = 21;
        public const int UserNotExists = 22;

        // External errors
        public const int AmazonApiError = 30;

        //Artifacts errors
        public const int InvalidArtifactSettings = 40;

        //Resources errors
        public const int ResourceNameRepeated = 50;

        //ProjectResourcecs errors
        public const int InvalidBeginDateForProjectResource = 51;
        public const int InvalidEndDateForProjectResource = 52;
    }
}
