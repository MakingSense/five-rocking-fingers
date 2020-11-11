import ProjectCategory from "./ProjectCategory";
import UserByProject from "./UserByProject";

export default interface Project {
    id: number;
    name: string;
    owner: string;
    client: string;
    budget: number;
    createdDate: Date;
    modifiedDate: Date;
    projectCategories: ProjectCategory[];
    usersByProject: UserByProject[];
}