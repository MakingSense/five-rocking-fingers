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

        // Users
        public const int UserNotExists = 21;
    }
}
