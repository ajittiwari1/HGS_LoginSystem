<h1>MS SQL Queries</h1>
<p>
  CREATE DATABASE HGS_LoginSystem;
GO

USE HGS_LoginSystem;
GO

CREATE TABLE Users (
    UserID VARCHAR(50) PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    Status VARCHAR(10) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL
);

select * from users



</p>
