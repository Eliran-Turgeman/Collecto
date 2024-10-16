# Collecto
API for managing email collection forms.

**NOTE**: You can also view the same contents within the app using swagger. [Start](http://127.0.0.1:8000/usage/overview/#setup-running-the-application) the application and go [here](localhost:5001//swagger/index.html)

## Version: v1

### Terms of service
https://example.com/terms

**Contact information:**  
Example Contact  
https://example.com/contact  

**License:** [Example License](https://example.com/license)

### /register

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |

### /login

#### POST
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| useCookies | query |  | No | boolean |
| useSessionCookies | query |  | No | boolean |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /refresh

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /confirmEmail

#### GET
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| userId | query |  | No | string |
| code | query |  | No | string |
| changedEmail | query |  | No | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /resendConfirmationEmail

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /forgotPassword

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |

### /resetPassword

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |

### /manage/2fa

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

### /manage/info

#### GET
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

### /api/EmailSignups/form/{formId}

#### GET
##### Summary:

Get email signups for a specific form.

##### Description:

Sample request:

    GET /api/EmailSignups/form/5

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| formId | path | Form id to get email signups for. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Returns all email signups for the form. |
| 400 | If the user is not authenticated. |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | If the form is not found. |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

### /api/EmailSignups

#### POST
##### Summary:

Signup for an email form.

##### Description:

Sample request:

    POST /api/EmailSignups
    {
        "FormId": "5",
        "Email": "example@email.com"
    }

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Email signup to form was successful |
| 400 | If the email is invalid |
| 404 | If the form is not found |
| 409 | If the form is not active, or the email address is already signed up for this form. |
| 429 | If API calls quota exceeded - 10 calls per 1min |

### /api/EmailSignups/confirmations

#### GET
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| confirmationToken | query |  | No | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/SignupForms

#### GET
##### Summary:

Get all signup forms for the current user.

##### Description:

Sample request:

    Get /api/SignupForms

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Returns all signup forms the current user has created. |
| 401 | If the user is not authenticated |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

#### POST
##### Summary:

Creates a new signup form.

##### Description:

Sample Requests:

Create a new form:

    POST /api/SignupForms
    {
        "FormName": "My Form"
    }
    
Create a new inactive form:
    
    POST /api/SignupForms
    {
        "FormName": "My Form",
        "Status": "Inactive"
    }

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 201 | Returns the newly created form. |
| 400 | If the user is not authenticated. |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

### /api/SignupForms/{id}

#### GET
##### Summary:

Get a specific signup form.

##### Description:

Sample request:

    Get /api/SignupForms/5

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Signup form id | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Returns the signup form matching the id. |
| 400 | If the user is not authenticated. |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | If the signup form is not found. |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

#### DELETE
##### Summary:

Deletes a signup form.

##### Description:

Sample request:

    DELETE /api/SignupForms/5

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Id of form to delete. | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 204 | If the form is deleted successfully. |
| 400 | If the user is not authenticated. |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | If the form is not found. |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |

#### PUT
##### Summary:

Updates a signup form.

##### Description:

Sample requests:

Updating all info:

    PUT /api/SignupForms/5
    {
        "FormName": "Updated Form Name",
        "Status": "Inactive"
    }
    
Updating only status:

    PUT /api/SignupForms/5
    {
        "Status": "Active"
    }

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Id of a form to update | Yes | integer |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | |
