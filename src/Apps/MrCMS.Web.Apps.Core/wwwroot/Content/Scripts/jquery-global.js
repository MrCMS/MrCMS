import $ from "jquery";

//Support for safari 12.1 and below
if (typeof globalThis === 'undefined') {
    var globalThis = Function('return this')();
}

globalThis.jQuery = $;