import {setupRecaptcha} from "./recaptcha";

setupRecaptcha();
$(() => {
    GLightbox({
        selector: ".glightbox",
    });
})