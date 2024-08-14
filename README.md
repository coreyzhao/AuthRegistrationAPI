# Authentication and Registration API

This is a C# .NET Core API for user authentication and registration. It allows users to register, log in, and manage their authentication tokens.

## Features

- **User Registration:** Allows new users to register with a unique email and password.
- **User Authentication:** Users can log in with their credentials and receive a JWT token.
- **Token Validation:** Secure endpoint access using JWT tokens.
- **Password Hashing:** User passwords are securely hashed using industry-standard algorithms.

## Getting Started

### Prerequisites

- .NET SDK 8.0
- A database (e.g. MS SQL Server, MySQL)

### Installation

1. **Clone the repository:**

    ```bash
    git clone https://github.com/coreyzhao/AuthRegistrationAPI.git
    cd AuthRegistrationAPI
    ```

2. **Install dependencies:**

    ```bash
    dotnet restore
    ```

3. **Configure the database:**

   Update the `appsettings.json` file with your database connection string.

4. **Run the application:**

    ```bash
    dotnet run
    ```

5. **Access the API:**

    The API will be running at `https://localhost:5000`.

## Configuration

- **appsettings.json:**
  - Configure the connection string for your database.
  - Set up JWT settings (PasswordKey and TokenKey).


## Auth API Endpoints

- **POST /Auth/Register**
  - Registers a new user.
  - **Request:** `{
  "email": "string",
  "password": "string",
  "passwordConfirm": "string",
  "firstName": "string",
  "lastName": "string",
  "gender": "string"
  }`
  - **Response:** `200 Success` or `400 Bad Request`

- **POST /Auth/Login**
  - Authenticates an existing user.
  - **Request:** `{ "email": "string", "password": "string" }`
  - **Response:** `{ "token": "JWT token" }` or `401 Unauthorized`
 
- **GET /Auth/RefreshToken**
  - Refreshes an existing user's token.
  - **Request:** `{}`
  - **Response:** `{ "token": "JWT token" }` or `401 Unauthorized`
 
## Post/User API Endpoints

- **GET /Post/Posts**
  - Returns all posts.
 
- **GET /Post/PostSingle/{postId}**
  - Returns a specific post based on the postId.
 
- **GET /Post/PostByUser/{userId}**
  - Returns posts based on the userId.
 
- **GET /Post/MyPosts**
  - Returns posts based on the current authenticated user.
 
- **GET /Post/PostsBySearch/{searchParam}**
  - Returns all posts with matching characters.
 
- **POST /Post/Post**
  - Creates a new post.
 
- **DELETE /Post/Post/{postId}**
  - Deletes a specific post by postId.
 
- **GET /User/GetUsers**
  - Returns all users.
 
- **GET /User/GetSingleUser/{userId}**
  - Returns a single user by userId.

- **DELETE /User/DeleteUser/{userId}**
  - Deletes a single user by userId





