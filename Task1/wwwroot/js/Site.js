document.addEventListener("DOMContentLoaded", function (event) {

    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // show navbar
                nav.classList.toggle('show')
                // change icon (using class toggle now)
                toggle.classList.toggle('bx-x') // Assumes initial icon is bx-menu
                // add padding to body
                bodypd.classList.toggle('body-pd')
                // add padding to header
                headerpd.classList.toggle('body-pd')
            })
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    /*===== LINK ACTIVE =====*/
    // Select all link types
    const allLinks = document.querySelectorAll('.nav_link, .nav_submenu-link');

    function colorLink() {
        if (allLinks) {
            // Remove active from all links first
            allLinks.forEach(l => l.classList.remove('active'));

            // Add active to the clicked link
            this.classList.add('active');

            // --- NEW: If it's a submenu link, also keep its parent active ---
            const parentItem = this.closest('.nav_item');
            if (parentItem) {
                const parentLink = parentItem.querySelector('.nav_link_parent');
                if (parentLink) {
                    parentLink.classList.add('active'); // Keep parent visually active too
                }
            }
            // --- END NEW ---
        }
    }
    allLinks.forEach(l => l.addEventListener('click', colorLink));


    /*===== SUBMENU TOGGLE =====*/
    const linkParents = document.querySelectorAll('.nav_link_parent');

    function toggleSubmenu(event) {
        // Prevent the link's default navigation if href="#"
        if (this.getAttribute('href') === '#') {
            event.preventDefault();
        }

        // Get the parent .nav_item which contains the link and the submenu
        const parentItem = this.closest('.nav_item');
        if (parentItem) {
            // Find the submenu within this item
            const submenu = parentItem.querySelector('.nav_submenu');
            // Find the dropdown icon within this link
            const dropdownIcon = this.querySelector('.nav_dropdown-icon');

            if (submenu) {
                // --- Close other open submenus (optional) ---
                const allSubmenus = document.querySelectorAll('.nav_submenu');
                const allArrows = document.querySelectorAll('.nav_link_parent .nav_dropdown-icon');
                allSubmenus.forEach(sub => {
                    if (sub !== submenu && sub.classList.contains('submenu-show')) {
                        sub.classList.remove('submenu-show');
                        // Also reset the arrow of the other submenu's parent
                        const otherParent = sub.closest('.nav_item');
                        if (otherParent) {
                            const otherArrow = otherParent.querySelector('.nav_link_parent .nav_dropdown-icon');
                            if (otherArrow) otherArrow.classList.remove('arrow-rotate');
                        }
                    }
                });
                // --- End close other submenus ---


                // Toggle the current submenu
                submenu.classList.toggle('submenu-show');
                // Toggle the arrow rotation
                if (dropdownIcon) {
                    dropdownIcon.classList.toggle('arrow-rotate');
                }
            }
        }
    }

    linkParents.forEach(l => l.addEventListener('click', toggleSubmenu));

    // Your code to run since DOM is loaded and ready
});