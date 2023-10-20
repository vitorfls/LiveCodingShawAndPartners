# Live Coding Challenge - Shaw And Partners - ORM and User Classes

## Developed by Vitor Lopes

I, Vitor Lopes, have developed and published the solution to this Live Coding Challenge. 

## Overview

In this challenge, you'll find the code solution that demonstrates my skills and understanding of object-oriented programming and database operations. It includes the implementation of two classes: `ORM` and `User`, as well as tasks related to data manipulation and validation.

## Prerequisites

Before attempting this challenge, you should have:

- A basic knowledge of the C# programming language.
- Familiarity with object-oriented programming (OOP) concepts.
- An understanding of dictionaries and LINQ.

## Tasks

### Task 1: ORM and User Classes

1. Create two classes: `ORM` and `User`.
2. In the `ORM` class, implement a `GetInstance()` method that checks if a `User` instance exists. If it does, return it; otherwise, create and return it.

### Task 2: Implement `GetAll`

1. Create a method `GetAll` in the `ORM` class to retrieve all records from a table.
2. Use a `foreach` loop to print each record as a dictionary.

### Task 3: Column Validation

1. Implement a `Column` class with properties for data type, primary key, auto-increment, and not null.
2. Create a method in the `Table` class to validate if columns match the `Column` class specifications during inserts and updates.

### Task 4: Data Insertion and Updating

1. Implement an `Insert` method in the `Table` class to insert a record with default values.
2. Implement an `Update` method in the `Table` class to update a record.
3. Prior to any insert or update operation, validate each record to maintain data integrity and compliance with column specifications.


### Task 5: Filtering

1. Implement the ability to filter and search for records based on specific criteria.
2. Allow filtering by a specific key, such as "id".

## Instructions

1. Fork this repository to your GitHub account.
2. Clone the forked repository to your local machine.
3. Complete the tasks as described in the provided code.
4. Commit your changes and push them to your GitHub repository.
5. Create a `README.md` file in your repository to document your approach and any additional comments.
6. Ensure your code is well-formatted and commented.

## Additional Information

- You may use any development environment and tools that you are comfortable with.
- Feel free to add more comments or explanations to the code to make it easier to understand.
