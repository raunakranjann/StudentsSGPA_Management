CREATE DATABASE studentdb;

USE studentdb;

CREATE TABLE students (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100),
    roll_number VARCHAR(50),
    registration_number VARCHAR(50),
    sem1_sgpa DOUBLE,
    sem2_sgpa DOUBLE,
    sem3_sgpa DOUBLE,
    avg_sgpa DOUBLE
);

use studentdb;




select *from students 
where avg_sgpa>"5";