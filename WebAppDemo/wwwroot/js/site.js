

//document.addEventListener("DOMContentLoaded", function () {
//    const loginForm = document.getElementById('loginForm');
//    const signupForm = document.getElementById('signupForm');

//   //  Login işlemi
//    if (loginForm) {
//        loginForm.addEventListener('submit', async function (e) {
//            e.preventDefault();
//            const username = document.getElementById('UserName').value;
//            const password = document.getElementById('password').value;

//            const data = {
//                UserName: username,
//                Password: password
//            };

//            try {
//                const response = await fetch('http://localhost:5269/api/Auth/login', {
//                    method: 'POST',
//                    headers: {
//                        'Content-Type': 'application/json',
//                        'Accept': 'application/json'
//                    },
//                    body: JSON.stringify(data)
//                });

//                if (response) {
//                    const result = await response.json();
//                    var resultToken = result;
//                    if (response.ok && result) {
//                        sessionStorage.setItem('jwtToken', resultToken);
//                        console.log('token kaydedildi', resultToken);
//                        const token = sessionStorage.getItem('jwtToken');
//                        console.log('token alındı', token)

//                        const protectedResponse = await fetch('http://localhost:5269/api/Auth/protected', {
//                            method: 'GET',
//                            headers: {
//                                'Content-Type': 'application/json',
//                                'Authorization': `Bearer ${token}`
//                            }
//                        });
//                        var protectedResult = await protectedResponse.json();
//                        if (protectedResponse.ok && protectedResult.userName !== undefined) {






//                            var xx = await fetch('http://localhost:5254/Authorize/SecretPage', {
//                                headers: { 'Authorization': `Bearer ` + sessionStorage.getItem('jwtToken') }
//                            })
//                                .then((response) => {
//                                    if (response.ok) {
//                                        //return response.text();
//                                        window.location.href = response.url;
//                                    } else {
//                                        alert('Tekrar giriş yapın.');
//                                        return Promise.reject('Giriş hatası');
//                                    }
//                                })
//                                .then((data) => {
//                                    document.getElementById('secretPageContent').innerHTML = data;
//                                })
//                                .catch((error) => {
//                                    console.error(error);
//                                });


//                            //window.location.href = '/Authorize/SecretPage';
//                        } else {
//                            alert('Yetkilendirme başarısız! Lütfen tekrar giriş yapın.');
//                        }
//                    } else {
//                        alert('Yanlış kullanıcı adı veya şifre.');
//                    }
//                }
                
//            } catch (error) {
//                console.error('Giriş sırasında hata oluştu:', error);
//            }
//        });
//    }
 
//     //Signup işlemi
//    if (signupForm) {
//        signupForm.addEventListener('submit', async function (e) {
//            e.preventDefault();
//            console.log("Signup event tetiklendi!");
//            const username = document.getElementById('UserName').value;
//            const password = document.getElementById('password').value;
//            const email = document.getElementById('email').value;

//            const data = {
//                UserName: username,
//                Password: password,
//                Email: email
//            };

//            try {
//                const response = await fetch('http://localhost:5269/api/Auth/signup', {
//                    method: 'POST',
//                    headers: {
//                        'Content-Type': 'application/json',
//                        'Accept': 'application/json'
//                    },
//                    body: JSON.stringify(data)
//                });

//                if (response.ok) {
//                    window.location.href = '/auth/login';
//                } else {
//                    alert('Kayıt işlemi başarısız. Kullanıcı adı zaten alınmış olabilir.');
//                }
//            } catch (error) {
//                console.error('Kayıt sırasında hata oluştu:', error);
//            }
//        });
//    }
//});