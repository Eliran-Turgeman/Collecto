# Accepting signups for a form

Once you've created a form in Collecto, you'll receive a unique FormId in the response. This FormId is essential for accepting signups for that specific form. Here's how you can collect signups using the API.

In case you need a reminder on how to create a form go [here](https://www.16elt.com/Collecto/usage/create_forms/).

Now that you have the FormId, you can use it to accept signups by making a POST request to the following endpoint:

```
POST /api/EmailSignups
{
    "FormId": "5",
    "Email": "example@email.com"
}
```

Note that this endpoint is public, and does not require the authorization header.

If you have email confirmations on, an email confirmation will be sent to `example@email.com` asking to confirm the signup. If the user didn't confirm the signup will be dropped.

## Using Collecto on your site

Here is a simple HTML + JS example on how you can use collecto to collect emails in your site w/ two simple steps.

1. Create a HTML form
```html
<form id="signup-form" action="https://your-collecto-instance.com/api/EmailSignups" method="POST">
        <input type="email" name="email" placeholder="Enter your email" required>
        <input type="submit" value="Tell me more">
</form>
```

2. Add a JS script to call collecto
```html
<script>
    document.getElementById('signup-form').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const formData = new FormData(this);
        const email = formData.get('email');
        const formId = '1'; // Update with your actual form ID
        const collectoDomain = 'https://your-collecto-instance.com';
        const messageDiv = document.getElementById('form-message');
        
        try {
            const response = await axios.post(`${collectoDomain}/api/EmailSignups`, {
                formId,
                email
            });
            
            messageDiv.style.display = 'block';
            messageDiv.className = 'message success';
            messageDiv.textContent = 'Signup successful! Please check your email to confirm your subscription.';
            this.reset();
        } catch (error) {
            console.error('Error during signup:', error);
            messageDiv.style.display = 'block';
            messageDiv.className = 'message error';
            
            if (error.response) {
                switch (error.response.status) {
                    case 400: // Bad Request
                        messageDiv.textContent = 'Invalid email address.';
                        break;
                    case 404: // Not Found
                        messageDiv.textContent = 'Signup form not found.';
                        break;
                    case 409: // Conflict
                        if (error.response.data.includes('Email address already signed up')) {
                            messageDiv.textContent = 'Email address already signed up.';
                        } else if (error.response.data.includes('Form is not active')) {
                            messageDiv.textContent = 'Signup form is not active.';
                        } else {
                            messageDiv.textContent = 'A conflict occurred. Please try again.';
                        }
                        break;
                    default:
                        messageDiv.textContent = 'An unexpected error occurred. Please try again.';
                }
            } else {
                messageDiv.textContent = 'Network error. Please check your internet connection and try again.';
            }
        }
    });
</script>
```