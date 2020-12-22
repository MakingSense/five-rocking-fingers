﻿import ProjectCategory from "./ProjectCategory";
import UserProfile from "./UserProfile";

export default interface Project {
    id: number;
    name: string;
    owner: string;
    client: string;
    budget: number;
    createdDate: Date;
    modifiedDate: Date;
    projectCategories: ProjectCategory[];
    users: UserProfile[];
}