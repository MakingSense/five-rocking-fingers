namespace FRF.Core.Response
{
    public static class ErrorCodes
    {
        // Not exists errors
        public const int ArtifactNotExists = 1;
        public const int ArtifactTypeNotExists = 2;
        public const int CategoryNotExists = 3;
        public const int ProjectNotExists = 4;

        // Relation errors
        public const int RelationAlreadyExisted = 10;
        public const int RelationNotValid = 11;
        public const int RelationNotExists = 12;

        // User errors
        public const int InvalidCredentials = 20;
        public const int AuthenticationServerCurrentlyUnavailable = 21;
        public const int UserNotExists = 22;

        // External errors
        public const int AmazonApiError = 30;
    }
}
