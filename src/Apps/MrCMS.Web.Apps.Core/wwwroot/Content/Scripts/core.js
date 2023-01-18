//Jquery
import "./jquery-global.js";
import $ from "jquery";

import 'jquery-validation';
import 'jquery-validation-unobtrusive';


//Bootstrap JS
import * as bootstrap from 'bootstrap';

import GLightbox from 'glightbox';
import {setupRecaptcha} from "./recaptcha";

$(() => {
    setupRecaptcha();
    GLightbox({
        selector: ".glightbox",
    });
});