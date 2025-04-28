// Fetch Options.json and populate table
document.addEventListener('DOMContentLoaded', function () {
    fetch('/js/Create/Options.json')
        .then(response => response.json())
        .then(data => {
            if (!data) return;
            const tbody = document.querySelector('#tblSettings tbody');
            data.forEach(item => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td class="Property">${item.Property}</td>
                    <td class="Default">${item.Default}</td>
                    <td class="Description">${item.Description}</td>
                `;
                tbody.appendChild(tr);
            });
        });

    // Toggle sidenav on menu button click
    const toggle = document.querySelector('.menu-toggle');
    if (toggle) {
        toggle.addEventListener('click', function () {
            document.body.classList.toggle('menu-close');
        });
    }

    // Toggle side menu based on screen size
    function toggleSideMenuByScreenSize() {
        const screenWidth = 900;
        if (document.body.clientWidth < screenWidth) {
            document.body.classList.add('menu-close');
        } else {
            document.body.classList.remove('menu-close');
        }
    }

    window.addEventListener('resize', toggleSideMenuByScreenSize);
    toggleSideMenuByScreenSize();

    // Smooth scroll on menu link click
    const menuLinks = document.querySelectorAll('ul.menu a');
    menuLinks.forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            const href = this.getAttribute('href');
            if (href && href !== '#') {
                const target = document.querySelector(href);
                if (target) {
                    const top = target.offsetTop - 30;
                    document.querySelector('.page-content').scrollTo({
                        top: top,
                        behavior: 'smooth'
                    });
                }
            }
        });
    });
});

// YearPicker
(function () {
    const version = "1.1.0";
    const namespace = "yearpicker";

    const defaults = {
        year: null,
        startYear: null,
        endYear: null,
        itemTag: "li",
        selectedClass: "selected",
        disabledClass: "disabled",
        hideClass: "hide",
        template: `
            <div class="yearpicker-container">
                <div class="yearpicker-header">
                    <div class="yearpicker-prev" data-view="yearpicker-prev">&lsaquo;</div>
                    <div class="yearpicker-current" data-view="yearpicker-current">SelectedYear</div>
                    <div class="yearpicker-next" data-view="yearpicker-next">&rsaquo;</div>
                </div>
                <div class="yearpicker-body">
                    <ul class="yearpicker-year" data-view="years"></ul>
                </div>
            </div>
        `,
        onShow: null,
        onHide: null,
        onChange: null,
    };

    class Yearpicker {
        constructor(element, options = {}) {
            this.options = Object.assign({}, defaults, options);
            this.element = element;
            this.isInput = element.tagName === "INPUT" || element.tagName === "TEXTAREA";
            this.template = this.createTemplate();
            this.show = false;
            this.build = false;
            this.year = this.options.year || this.getValue() || new Date().getFullYear();
            this.viewYear = this.year;
            this.startYear = this.options.startYear || null;
            this.endYear = this.options.endYear || null;

            this.init();
        }

        createTemplate() {
            const temp = document.createElement('div');
            temp.innerHTML = this.options.template.trim();
            return temp.firstChild;
        }

        init() {
            if (this.build) return;
            this.build = true;

            this.yearsPrev = this.template.querySelector(".yearpicker-prev");
            this.yearsCurrent = this.template.querySelector(".yearpicker-current");
            this.yearsNext = this.template.querySelector(".yearpicker-next");
            this.yearsBody = this.template.querySelector(".yearpicker-year");

            this.template.classList.add(this.options.hideClass);
            this.element.parentNode.insertBefore(this.template, this.element.nextSibling);

            this.bindEvents();
            this.renderYear();
        }

        bindEvents() {
            if (this.isInput) {
                this.element.addEventListener('focus', () => this.showView());
            } else {
                this.element.addEventListener('click', () => this.showView());
            }
        }

        showView() {
            if (!this.build) this.init();
            if (this.show) return;

            this.show = true;
            this.template.classList.remove(this.options.hideClass);

            this.template.addEventListener('click', this.click.bind(this));
            document.addEventListener('click', this.globalClick = this.globalClick.bind(this));

            if (typeof this.options.onShow === "function") {
                this.options.onShow(this.year);
            }
        }

        hideView() {
            if (!this.show) return;
            this.show = false;

            this.template.classList.add(this.options.hideClass);
            this.template.removeEventListener('click', this.click.bind(this));
            document.removeEventListener('click', this.globalClick);

            if (typeof this.options.onHide === "function") {
                this.options.onHide(this.year);
            }
        }

        click(e) {
            const target = e.target;
            if (target.classList.contains('disabled')) return;

            const view = target.dataset.view;
            if (view === "yearpicker-prev") {
                this.viewYear -= 12;
                this.renderYear();
            } else if (view === "yearpicker-next") {
                this.viewYear += 12;
                this.renderYear();
            } else if (view === "yearpicker-items") {
                this.year = parseInt(target.innerText);
                this.setValue();
                this.renderYear();
                this.hideView();
            }
        }

        globalClick(e) {
            if (!this.template.contains(e.target) && this.element !== e.target) {
                this.hideView();
            }
        }

        renderYear() {
            const now = new Date();
            const thisYear = now.getFullYear();
            const start = -5;
            const end = 6;
            const items = [];

            for (let i = start; i <= end; i++) {
                const year = this.viewYear + i;
                let disabled = false;

                if (this.startYear && year < this.startYear) {
                    disabled = true;
                }
                if (this.endYear && year > this.endYear) {
                    disabled = true;
                }

                items.push(this.createItem({
                    text: year,
                    selected: year === this.year,
                    disabled: disabled,
                    highlighted: year === thisYear
                }));
            }

            this.yearsBody.innerHTML = items.join(" ");
            this.yearsPrev.classList.toggle(this.options.disabledClass, this.startYear && (this.viewYear - 12) < this.startYear);
            this.yearsNext.classList.toggle(this.options.disabledClass, this.endYear && (this.viewYear + 12) > this.endYear);
            this.yearsCurrent.innerText = this.year;
        }

        createItem({ text, selected, disabled, highlighted }) {
            const classes = ["yearpicker-items"];
            if (selected) classes.push(this.options.selectedClass);
            if (disabled) classes.push(this.options.disabledClass);
            if (highlighted) classes.push('highlighted');
            return `<${this.options.itemTag} class="${classes.join(' ')}" data-view="yearpicker-items">${text}</${this.options.itemTag}>`;
        }

        getValue() {
            return parseInt(this.isInput ? this.element.value : this.element.innerText) || null;
        }

        setValue() {
            const value = this.year;
            if (this.isInput) {
                this.element.value = value;
            } else {
                this.element.innerText = value;
            }

            if (typeof this.options.onChange === "function") {
                this.options.onChange(this.year);
            }
            const event = new Event('change');
            this.element.dispatchEvent(event);
        }
    }

    window.Yearpicker = Yearpicker;

    // Initialize all with attribute [data-toggle="yearpicker"]
    document.addEventListener('DOMContentLoaded', function () {
        const elements = document.querySelectorAll('[data-toggle="yearpicker"]');
        elements.forEach(el => {
            new Yearpicker(el);
        });
    });

})();
