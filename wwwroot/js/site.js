// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {
    const navbarCollapse = document.getElementById("navbarCollapse");

    if (!navbarCollapse || !window.bootstrap) {
        return;
    }

    navbarCollapse.querySelectorAll('a[href*="#"]').forEach((link) => {
        link.addEventListener("click", () => {
            if (navbarCollapse.classList.contains("show")) {
                window.bootstrap.Collapse.getOrCreateInstance(navbarCollapse).hide();
            }
        });
    });
});
