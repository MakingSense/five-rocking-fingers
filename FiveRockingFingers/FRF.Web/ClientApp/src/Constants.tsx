// Constants definitions for access on ClientApp

export const BASE_URL = process.env.REACT_APP_BASE_URL;
export const SETTINGTYPES = ['decimal', 'naturalNumber'];

export const CUSTOM_PROVIDER = 'Custom';
export const AWS_PROVIDER = 'AWS';
export const PROVIDERS = [AWS_PROVIDER, CUSTOM_PROVIDER];

//Amazon EC2 constants
export const AMAZON_EC2 = 'AmazonEC2';
export const COMPUTE_INSTACE = 'Compute Instance';
export const STORAGE = 'Storage';
export const STORAGE_SNAPSHOT = 'Storage Snapshot';
export const SYSTEM_OPERATION = 'System Operation';
export const PROVISIONED_THROUGHPUT = 'Provisioned Throughput';
export const DATA_TRANSFER_1 = 'Data Transfer 1';
export const DATA_TRANSFER_2 = 'Data Transfer 2';
export const DATA_TRANSFER_3 = 'Data Transfer 3';

//Amazon S3 constants
export const AMAZON_S3 = 'AmazonS3';
