document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('sidebar');
    const mobileToggle = document.getElementById('mobileSidebarToggle');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebar) {
        // Remove init class to enable smooth transitions
        setTimeout(() => {
            sidebar.classList.remove('sidebar-init');
        }, 10);

        // Desktop: Toggle Sidebar (Expand on click when collapsed, collapse on header click when expanded)
        sidebar.addEventListener('click', function (e) {
            if (window.innerWidth >= 992) {
                const isCurrentlyCollapsed = this.classList.contains('collapsed');
                const header = e.target.closest('.sidebar-header');

                if (isCurrentlyCollapsed) {
                    this.classList.remove('collapsed');
                    localStorage.setItem('sidebar-expanded', 'true');
                    e.stopPropagation();
                } else if (header) {
                    this.classList.add('collapsed');
                    localStorage.setItem('sidebar-expanded', 'false');
                    e.stopPropagation();
                }
            }
        });

        // Mobile: Toggle Sidebar
        if (mobileToggle) {
            mobileToggle.addEventListener('click', function (e) {
                sidebar.classList.toggle('show');
                overlay.classList.toggle('show');
                e.stopPropagation();
            });
        }

        // Close sidebar when clicking overlay
        if (overlay) {
            overlay.addEventListener('click', function () {
                sidebar.classList.remove('show');
                overlay.classList.remove('show');
            });
        }

        // Close sidebar when clicking links on mobile
        const navLinks = sidebar.querySelectorAll('.nav-link-portal');
        navLinks.forEach(link => {
            link.addEventListener('click', () => {
                if (window.innerWidth < 992) {
                    sidebar.classList.remove('show');
                    overlay.classList.remove('show');
                }
            });
        });
    }
});
