// Fix 1: Added missing comma after "submit"
document.getElementById("mailform").addEventListener("submit", function (e) {
    e.preventDefault();
    submitForm(this);
});

async function submitForm(form) {
    try {
        // Fix 2: Add loading state
        document.querySelector('.loading').style.display = 'block';

        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: form.method,
            body: formData,
            // Fix 3: Add anti-forgery token
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (!response.ok) throw new Error('Network response was not ok');

        const result = await response.json();

        // Fix 4: Update UI with result
        document.querySelector('.loading').style.display = 'none';
        const resultDiv = document.querySelector('#result .error-message, #result .sent-message') ||
            document.createElement('div');
        resultDiv.className = 'sent-message';
        resultDiv.textContent = result.message;
        resultDiv.style.display = 'flex';
        document.getElementById('result').appendChild(resultDiv);

    } catch (error) {
        document.querySelector('.loading').style.display = 'none';
        const errorDiv = document.querySelector('#result .error-message') ||
            document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = '';
        errorDiv.style.display = 'flex';
        document.getElementById('result').appendChild(errorDiv);
    }
}