import {setupRecaptcha} from "./recaptcha";
import {setupPushButton} from "./webpush";

setupRecaptcha();
$(()=>{
    setupPushButton();
})