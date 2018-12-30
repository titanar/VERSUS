import Shuffle from 'shufflejs';

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", CreateShuffleInstance);
} else {  // `DOMContentLoaded` already fired
    CreateShuffleInstance();
}

function CreateShuffleInstance() {
    const scrollSection = document.getElementById('product-scroll-section');
    const itemSelector = '.product-box';
    const sizer = '.product-scroll-sizer';

    const shuffleInstance = new Shuffle(scrollSection, {
        itemSelector: itemSelector,
        sizer: sizer,
        buffer: 20,
        columnThreshold: 20
    })
}