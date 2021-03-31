import Provider from './Provider';

export default interface ArtifactType {
    id: number;
    name: string;
    description: string;
    provider: Provider
}