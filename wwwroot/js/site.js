// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Navigation submenu toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    // Handle submenu toggle
    const navHeaders = document.querySelectorAll('.sidebar .nav-header');
    
    navHeaders.forEach(header => {
        header.addEventListener('click', function(e) {
            e.preventDefault();
            const submenu = this.nextElementSibling;
            const chevron = this.querySelector('.fa-chevron-down, .fa-chevron-up');
            
            if (submenu && submenu.classList.contains('submenu')) {
                submenu.classList.toggle('show');
                
                // Toggle chevron direction
                if (chevron) {
                    if (submenu.classList.contains('show')) {
                        chevron.classList.remove('fa-chevron-down');
                        chevron.classList.add('fa-chevron-up');
                    } else {
                        chevron.classList.remove('fa-chevron-up');
                        chevron.classList.add('fa-chevron-down');
                    }
                }
            }
        });
    });
});
