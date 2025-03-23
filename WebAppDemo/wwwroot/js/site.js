document.getElementById('loginForm').addEventListener('submit', function (e) {
    e.preventDefault();  // Formun normal submit işlemini engeller

    const username = document.getElementById('UserName').value;
    const password = document.getElementById('password').value;

    const data = {
        username: username,
        password: password
    };

    fetch('http://localhost:5269/api/Auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json()) 
        .then(response => { 
            if (response.token) {
                localStorage.setItem('jwtToken', response.token);
                window.location.href = '/home'; 
            } else {
                alert('Login failed');
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
});



//document.getElementById('signupForm').addEventListener('submit', function (e) {
//    debugger;

//    e.preventDefault();  // Formun normal submit işlemini engeller

//    const username = document.getElementById('UserName').value;
//    debugger;
//    const email = document.getElementById('email').value;
//    const password = document.getElementById('password').value;
//    const confirmPassword = document.getElementById('confirmPassword').value;

//    if (password !== confirmPassword) {
//        alert("Passwords do not match!");
//        return;
//    }

//    const data = {
//        username: username,
//        email: email,
//        password: password
//    };

//    fetch('http://localhost:5269/api/Auth/signup', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify(data)
//    })
//        .then(response => response.json())
//        .then(response => {
//            if (response.token) {
//                localStorage.setItem('jwtToken', response.token);
//                window.location.href = '/home';  // Başarılı kayıt sonrası yönlendirme
//            } else {
//                alert('Sign up failed');
//            }
//        })
//        .catch(error => {
//            console.error('Error:', error);
//        });
//});
