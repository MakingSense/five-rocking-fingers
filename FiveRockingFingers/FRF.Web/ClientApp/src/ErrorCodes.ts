// Not exists errors
export const ARTIFACT_NOT_EXISTS: number = 1;
export const ARTIFACT_TYPE_NOT_EXISTS: number = 2;
export const CATEGORY_NOT_EXISTS: number = 3;
export const PROJECT_NOT_EXISTS: number = 4;
export const PROVIDER_NOT_EXISTS: number = 5;
export const ARTIFACT_FROM_ANOTHER_PROJECT: number = 6;

// Relation errors
export const RELATION_AL_READY_EXISTED: number = 10;
export const RELATION_NOT_VALID_DIFFERENT_BASE_ARTIFACT: number = 11;
export const RELATION_NOT_VALID_REPEATED = 12;
export const RELATION_NOT_VALID_DIFFERENT_TYPE = 13;
export const RELATION_NOT_EXISTS = 14;
export const RELATION_CYCLE_DETECTED = 15;

// User errors
export const INVALID_CREDENTIALS: number = 20;
export const AUTHENTICATION_SERVER_CURRENTLY_UNAVAILABLE: number = 21;
export const USER_NOT_EXISTS: number = 22;

// External errors
export const AMAZON_API_ERROR: number = 30;

//Artifacts errors
export const INVALID_ARTIFACT_SETTINGS: number = 40;