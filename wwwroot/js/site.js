// ═══════════════════════════════════════════════════════════════
// Site JavaScript — Vanilla JS only (no jQuery for app logic)
// ═══════════════════════════════════════════════════════════════

document.addEventListener('DOMContentLoaded', function () {

    // ── Sidebar Toggle Mechanics ───────────────────────────────
    const sidebar = document.getElementById('sidebar');
    const closeBtn = document.getElementById('sidebarClose');

    if (sidebar) {
        // Click anywhere on the collapsed sidebar to expand it.
        // We listen on the sidebar itself and only act when collapsed.
        sidebar.addEventListener('click', function (e) {
            if (sidebar.classList.contains('collapsed')) {
                // If they clicked on a nav-link, let the browser navigate normally
                if (e.target.closest('.nav-link-portal') || e.target.closest('.btn-logout')) {
                    return;
                }

                // Otherwise, they clicked empty space: expand the sidebar
                e.preventDefault();
                e.stopPropagation();
                sidebar.classList.remove('collapsed');
            }
        });

        // Explicit collapse via the chevron button (only when expanded)
        if (closeBtn) {
            closeBtn.addEventListener('click', function (e) {
                // Stop the click from bubbling to the sidebar (which would re-expand)
                e.stopPropagation();
                e.preventDefault();
                sidebar.classList.add('collapsed');
            });
        }
    }

    // ── Back to Top (smooth scroll) ────────────────────────────
    var backToTopLinks = document.querySelectorAll('.footer-link[href="#"]');
    backToTopLinks.forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    });
});
