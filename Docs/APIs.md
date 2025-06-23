# API Routes 

## users
- Get all user data = HttpGet: http://localhost:5049/api/User
- Get user data by Id = HttpGet: http://localhost:5049/api/User/{id}
- Add user = HttpPost: http://localhost:5049/api/User
- Update a user's data = HttpPut: http://localhost:5049/api/User/update
- Delete a user = HttpPut: http://localhost:5049/api/User/delete
- Restore a deleted user = HttpPut: http://localhost:5049/api/User/undo-delete
- Change password = HttpPut: http://localhost:5049/api/User/change-password
- User validation = HttpPost: http://localhost:5049/api/User/login