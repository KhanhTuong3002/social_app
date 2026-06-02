// On page load or when changing themes, best to add inline in `head` to avoid FOUC
if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    document.documentElement.classList.add('dark')
    } else {
    document.documentElement.classList.remove('dark')
    }

// Whenever the user explicitly chooses light mode
localStorage.theme = 'light'

// Whenever the user explicitly chooses dark mode
localStorage.theme = 'dark'

// Whenever the user explicitly chooses to respect the OS preference
localStorage.removeItem('theme')



// add post upload image 
const addPostUrlEl = document.getElementById('addPostUrl');
if (addPostUrlEl) {
    addPostUrlEl.addEventListener('change', function(){
        if (this.files[0] ) {
            var picture = new FileReader();
            picture.readAsDataURL(this.files[0]);
            picture.addEventListener('load', function(event) {
                const addPostImageEl = document.getElementById('addPostImage');
                if (addPostImageEl) {
                    addPostImageEl.setAttribute('src', event.target.result);
                    addPostImageEl.style.display = 'block';
                }
            });
        }
    });
}


// Create Status upload image 
const createStatusUrlEl = document.getElementById('createStatusUrl');
if (createStatusUrlEl) {
    createStatusUrlEl.addEventListener('change', function(){
        if (this.files[0] ) {
            var picture = new FileReader();
            picture.readAsDataURL(this.files[0]);
            picture.addEventListener('load', function(event) {
                const createStatusImageEl = document.getElementById('createStatusImage');
                if (createStatusImageEl) {
                    createStatusImageEl.setAttribute('src', event.target.result);
                    createStatusImageEl.style.display = 'block';
                }
            });
        }
    });
}


// create product upload image
const createProductUrlEl = document.getElementById('createProductUrl');
if (createProductUrlEl) {
    createProductUrlEl.addEventListener('change', function(){
        if (this.files[0] ) {
            var picture = new FileReader();
            picture.readAsDataURL(this.files[0]);
            picture.addEventListener('load', function(event) {
                const createProductImageEl = document.getElementById('createProductImage');
                if (createProductImageEl) {
                    createProductImageEl.setAttribute('src', event.target.result);
                    createProductImageEl.style.display = 'block';
                }
            });
        }
    });
}







    