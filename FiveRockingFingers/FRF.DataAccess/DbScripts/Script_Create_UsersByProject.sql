CREATE TABLE FiveRockingFingers.dbo.UsersByProject (
    id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    ProjectId INT NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Project_User FOREIGN KEY (ProjectId) REFERENCES FiveRockingFingers.dbo.Projects (Id)
);