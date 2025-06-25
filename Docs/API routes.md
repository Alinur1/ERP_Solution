# API Routes 

## 1. users
- Get all user data = HttpGet: http://localhost:5049/api/User
- Get user data by Id = HttpGet: http://localhost:5049/api/User/{id}
- Add user = HttpPost: http://localhost:5049/api/User
- Update a user's data = HttpPut: http://localhost:5049/api/User/update
- Delete a user = HttpPut: http://localhost:5049/api/User/delete
- Restore a deleted user = HttpPut: http://localhost:5049/api/User/undo-delete
- Change password = HttpPut: http://localhost:5049/api/User/change-password
- User validation = HttpPost: http://localhost:5049/api/User/login

## 2. roles
- Get all role data = HttpGet: http://localhost:5049/api/Role
- Get role data by Id = HttpGet: http://localhost:5049/api/Role/{id}
- Add a role = HttpPost: http://localhost:5049/api/Role
- Update a role = HttpPut: http://localhost:5049/api/Role/update
- Delete a role = HttpPut: http://localhost:5049/api/Role/delete
- Restore a deleted role: HttpPut: http://localhost:5049/api/Role/undo-delete

## 3. user_roles
- Get all users' role = HttpGet: http://localhost:5049/api/UserRole
- Get users' role by user_role Id = HttpGet: http://localhost:5049/api/UserRole/by-roles/{id}
- Get user's designated role by user Id = HttpGet: http://localhost:5049/api/UserRole/by-userid/{id}
- Assign a role to the user = HttpPost: http://localhost:5049/api/UserRole
- Update users' role = HttpPut: http://localhost:5049/api/UserRole/update
- Delete a user role = HttpDelete: http://localhost:5049/api/UserRole/delete

## 4. features

## 5. role_permissions

## 6. categories

## 7. suppliers

## 8. products

## 9. inventory

## 10. customers

## 11. sales_orders

## 12. sales_order_items

## 13. invoices

## 14. purchase_orders

## 15. purchase_order_items

## 16. expenses

## 17. departments

## 18. employees

## 19. attendance

## 20. payroll

## 21. accounts

## 22. transactions

## 23. ledgers

## 24. reports

## 25. notifications

## 26. settings

## 27. company_profile