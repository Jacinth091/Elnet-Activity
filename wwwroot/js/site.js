document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');

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

        // Mobile: Toggle Sidebar 'show' class
        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function (e) {
                sidebar.classList.toggle('show');
                e.stopPropagation();
            });
        }

        // Close sidebar when clicking anywhere outside
        document.addEventListener('click', function (e) {
            if (window.innerWidth < 992) {
                if (sidebar.classList.contains('show') && !sidebar.contains(e.target) && e.target !== sidebarToggle) {
                    sidebar.classList.remove('show');
                }
            } else {
                if (!sidebar.classList.contains('collapsed') && !sidebar.contains(e.target)) {
                    sidebar.classList.add('collapsed');
                    localStorage.setItem('sidebar-expanded', 'false');
                }
            }
        });
    }
});
