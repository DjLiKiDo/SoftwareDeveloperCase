# SoftwareDeveloperCase
Technical Skills Test
----------


The solution should be developed as an API using C#. 

The full source code can be uploaded to a Github repository (es) or sent by email.


## Problem

### Consider the following business rules:

- A user has: name, email, department and password.
- A user can adopt one or more roles.
- There is a hierarchy of roles.
- By default there are two roles: Employee and Manager.
- Any role will also be an Employee.
- Users in a role have certain permissions: read, add, update and delete.
- By default a Manager can have all the permissions.
- By default an Employee can only have read permission.
- Notify the Manager when a user has been registered in their department.

### The API must allow:
- Register users. Verify that there is no other user with the same email.
- Register roles. Be careful with the inheritance of roles. Don&#39;t define recursive roles.
- Assign permissions to roles.
- Assign roles to users.
- Get the permissions of a user based on her roles.


We will evaluate code quality, readability, maintainability, test coverage (implement unit tests that cover most cases), quality of tests.

The application of the SOLID principles (or any other design pattern) will be taken into account.

Additionally, the creation of a web application to display all users information and permissions will be valued.

In order to save some time, there is no need to store the data anywhere (users, permissions…).

