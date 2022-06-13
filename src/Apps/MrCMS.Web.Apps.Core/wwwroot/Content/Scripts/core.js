import GLightbox from 'glightbox';
import {setupRecaptcha} from "./recaptcha";

setupRecaptcha();
(function() {
    GLightbox({
        selector: ".glightbox",
    });
})();