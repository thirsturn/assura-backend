-- Assura Database Schema for Authentication and RBAC
-- Run this script in MySQL to create the required tables

USE Assura;

-- Create divisions table
CREATE TABLE IF NOT EXISTS divisions (
    divisionID VARCHAR(36) PRIMARY KEY,
    divisionName VARCHAR(100) NOT NULL UNIQUE
);

-- Create roles table with seed data
CREATE TABLE IF NOT EXISTS roles (
    roleID VARCHAR(36) PRIMARY KEY,
    roleName VARCHAR(50) NOT NULL UNIQUE
);

-- Seed default roles
INSERT IGNORE INTO roles (roleID, roleName) VALUES 
('1', 'Admin'),
('2', 'StoreKeeper'),
('3', 'User');

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    userID VARCHAR(36) PRIMARY KEY,
    firstname VARCHAR(100) NOT NULL,
    lastname VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    telephone VARCHAR(20),
    username VARCHAR(100) NOT NULL UNIQUE,
    passwordHash VARCHAR(255) NOT NULL,
    refreshToken VARCHAR(500),
    refreshTokenExpiryTime DATETIME,
    FcmToken VARCHAR(500),
    SocketId VARCHAR(100),
    isBlocked BOOLEAN DEFAULT FALSE,
    lastlogin DATETIME,
    createdOn DATETIME,
    lastUpdated DATETIME,
    divisionId VARCHAR(36),
    FOREIGN KEY (divisionId) REFERENCES divisions(divisionID) ON DELETE SET NULL
);

-- Create user_roles junction table
CREATE TABLE IF NOT EXISTS user_roles (
    UserId VARCHAR(36) NOT NULL,
    RoleId VARCHAR(36) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES users(userID) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES roles(roleID) ON DELETE CASCADE
);

-- Create indexes for better query performance
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_user_roles_userid ON user_roles(UserId);
CREATE INDEX idx_user_roles_roleid ON user_roles(RoleId);
