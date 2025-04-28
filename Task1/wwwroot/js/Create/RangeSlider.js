
class Slider {
    constructor(rangeElement, valueElement, options) {
        this.rangeElement = rangeElement
        this.valueElement = valueElement
        this.options = options

        // Attach a listener to "change" event
        this.rangeElement.addEventListener('input', this.updateSlider.bind(this))
    }

    // Initialize the slider
    init() {
        this.rangeElement.setAttribute('min', options.min)
        this.rangeElement.setAttribute('max', options.max)
        this.rangeElement.value = options.cur

        this.updateSlider()
    }

    // Format the money
    asMoney(value) {
        return '₨' + parseFloat(value)
            .toLocaleString('en-IN', { maximumFractionDigits: 2 })
    }

    generateBackground(rangeElement) {
        if (this.rangeElement.value === this.options.min) {
            return
        }

        let percentage = (this.rangeElement.value - this.options.min) / (this.options.max - this.options.min) * 100
        return 'background: linear-gradient(to right, #50299c, #7a00ff ' + percentage + '%, #d3edff ' + percentage + '%, #dee1e2 100%)'
    }

    updateSlider(newValue) {
        this.valueElement.innerHTML = this.asMoney(this.rangeElement.value)
        this.rangeElement.style = this.generateBackground(this.rangeElement.value)
    }
}

let rangeElement = document.querySelector('.range__slider input[type="range"]');
let valueElement = document.querySelector('.range__value span');

let options = {
    min: 200000,
    max: 200000000,
    cur: 3750000
}

if (rangeElement) {
    let slider = new Slider(rangeElement, valueElement, options)

    slider.init()
}