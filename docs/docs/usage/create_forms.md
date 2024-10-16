# Create a form w/ Collecto

This guide will walk you through the steps to create a new signup form using Collecto's API. To perform this action, you'll need to:

- Login (or register if needed) to obtain a Bearer token.
- Use the token to authorize your API request to create the form.

## Register

To register, first start the application - refer to [here](http://127.0.0.1:8000/usage/overview/#setup-running-the-application) for guidance.

After the application is live, you can access the following url To register : [localhost:5001/Identity/Account/Register](localhost:5001/Identity/Account/Register)

Note that if you just registered, you might need to confirm your registration based on the configuration of environment variables. Learn more [here](localhost:5001/Identity/Account/Login).

## Creating a form

To create a form we first need to obtain a bearer token via the /login endpoint.

To login make the following api call

```
POST /login

BODY

{
    "email": "email_you_registered_with"
    "password": "password_you_registered_with
}
```

Example response

```json
{
    "tokenType": "Bearer",
    "accessToken": "CfDJ8DG6wMVUulxDuszI796dx4uVk-4nASZd9Hy_Jjd7qv4S4C-8O34PvkXlBuRFuA8XM-OmZsb3V3LTaoa2AYVFYOvA2MqbkzY0OyEa4a8HtpAXI8KROFLs8jEwsIOrw47x7xcd4bMdlYlC7hRJd3FXIppf87R7nNDxCcIO-nimkcUAY6weYKwIwa6XWumuyAI89E1EyKbOmyjJOrfi7uf6J1_0JzM7kJkFAHrQR2Um4mdb6zNtD7mwpvjCaCTbWwoH7LKbYO72sNV3DhJs2o7o7G9yJiog0vy9-QQXrWULS2O2N9rMLLB_dfNIDJaqLLMDUWFttQUK0uFvJNrdL7Tu4q-N8Xd0TVGckCbYLxMZQ33aACXdRvQbgyM8LD8nAaydQvVUnAP623-x0UxZFwvCanXkl6fG2vELEhjDClRBDLr2XWMJ-AkgyXoyGyYSqMrbMb2LWXvWv89f_YD8aZ2wBeRS0gTJ2OKYv8tCQhsjO8rGMw5PuIxrsFqej6KEcyFa2LPXKQJjCl8hBJOn_ey0sYC2e08VUCZbE0RQxMuhfH27yBIp76OanXdSrH1Tz7prsJR3O1EalgEwgktFYD2jwSa_3Y5n9GCp_OZ6RFODJp49P5YxhfC-RGPtJW7pGGY9O5Z3xY3n98GjzHKZlZ6gwAFzVg_pniM7B-WWNKmXu9RzCeOlF81RIkqiCWCSp_9vvyll3-AiTtKkDn44KWqbuoE",
    "expiresIn": 3600,
    "refreshToken": "CfDJ8DG6wMVUulxDuszI796dx4srTN8_f5x54qdNPY-zzEAkCulSC2OgdnHFUv0RmL_U-BCLlm_r90b4Moqz7Irh_U4C5t0S-ZklNPjQMiLTGdL1aPpbuuBNVwFpTiDOhrGJGxR-imupgcLc3NCKjpyKSp9e5OaD_lz-XbJGcZiob9Qe2FrDA9ze61m8lwkZ4S0XT16DevxdTz_sJleMbLz7UoIscIN97fStvglSSbDESIAu8ITt14-N-acESBUwPiHHy9Ri-h7ZBHf7vkjwJiH9wlxTo-KhRlES6lVq-LJf0_mig2-MRjS6mmnaSOopFhUJKvqI9PZb0Q5n9dQ8j8uqUgsYWXGbbpTl3z9Wr9E3RnVkqUqaEHagyWRsGycayrBM7_ZaQMGbKo1oNuqFKxxCL8a6IzHq9TNSDyuErv2_GO_Ak6b7gVDvaFahqQv_bPA69x_-dRwYoJ-H5fY___UQ4fL_aYQ21SVo3ksV3Dp6rT3zeQ4Nd5sqohl-fidXvcRjknCDdo0RyY4yuL5dkwYWDOCOQxMoyfJQ0uAA6iFQgL-PWD-pfTOW5pjhzv3KdYkGYUqoyoVrmL-hzBv_uuvwv5iNZ8qN7GJkY_6V4N2q8d8F8ms_dNVN3_5nLp5OBY5gNF-Zxy0k9ah8URlKq04dlkut80PfsqGjXCfrgLVYDBk_2GZ9r3Ou7Um0SBboTt5ES1clJ1-xuaMdgdHPvi6McdI"
}
```

Now to create a form, we will use the token in the "accessToken" field in the authorization header with the bearer prefix - "Bearer <Token\>".  

Make the following request

```
POST /api/SignupForms
{
    "formName": "My Form"
}
```

