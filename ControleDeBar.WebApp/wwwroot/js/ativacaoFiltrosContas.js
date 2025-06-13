document.addEventListener("DOMContentLoaded", function () {
    const params = new URLSearchParams(window.location.search);
    const status = params.get("status");

    const botoesFiltro = document.querySelectorAll('.btn-filtro');

    for (const btn of botoesFiltro) {
        const href = btn.getAttribute('href') || "";

        if (
            (status === null && !href.includes("status=")) ||
            (status === "abertas" && href.includes("status=abertas")) ||
            (status === "fechadas" && href.includes("status=fechadas")) ||
            (status === "faturas" && href.includes("status=faturas"))
        ) {
            btn.classList.remove('btn-outline-primary');
            btn.classList.add('btn-primary');
        } else {
            btn.classList.remove('btn-primary');
            btn.classList.add('btn-outline-primary');
        }
    }
});