/**
 * jQuery Form Builder Plugin
 * Copyright (c) 2009 Mike Botsko, Botsko.net LLC (http://www.botsko.net)
 * http://www.botsko.net/blog/2009/04/jquery-form-builder-plugin/
 * Originally designed for AspenMSM, a CMS product from Trellis Development
 * Licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * Copyright notice and license must remain intact for legal use
 */
(function ($) {
    $.fn.formbuilder = function (options) {
        // Extend the configuration options with user-provided
        var defaults = {
            save_url: false,
            load_url: false,
            control_box_target: false,
            serialize_prefix: 'frmb',
            messages: {
                save: "Save",
                add_new_field: "Add New Field...",
                text: "Text Field",
                title: "Title",
                paragraph: "Paragraph",
                checkboxes: "Checkboxes",
                radio: "Radio",
                select: "Select List",
                text_field: "Text Field",
                label: "Label",
                paragraph_field: "Paragraph Field",
                select_options: "Select Options",
                add: "Add",
                checkbox_group: "Checkbox Group",
                remove_message: "Are you sure you want to remove this element?",
                remove: "Remove",
                radio_group: "Radio Group",
                selections_message: "Allow Multiple Selections",
                hide: "Hide",
                required: "Required",
                show: "Show",
                css_class: "Class",
                on_saved: function () {
                },
                get_object_id: function () {
                    return "";
                }
            }
        };
        var opts = $.extend(defaults, options);
        var frmb_id = 'frmb-' + $('ul[id^=frmb-]').length++;
        return this.each(function () {
            var ul_obj = $(this).append('<ul id="' + frmb_id + '" class="frmb"></ul>').find('ul');
            var field = '', field_type = '', last_id = 1, help, form_db_id;
            // Add a unique class to the current element
            $(ul_obj).addClass(frmb_id);
            // load existing form data
            if (opts.load_url) {
                $.getJSON(opts.load_url, function (json) {
                    form_db_id = json.form_id;
                    fromJson(json.form_structure);
                });
            }
            // Create form control select box and add into the editor
            var controlBox = function (target) {
                var select = '';
                var box_content = '';
                var save_button = '';
                var box_id = frmb_id + '-control-box';
                var save_id = frmb_id + '-save-button';
                // Add the available options
                select += '<option value="0">' + opts.messages.add_new_field + '</option>';
                select += '<option value="input_text">' + opts.messages.text + '</option>';
                select += '<option value="textarea">' + opts.messages.paragraph + '</option>';
                select += '<option value="checkbox">' + opts.messages.checkboxes + '</option>';
                select += '<option value="radio">' + opts.messages.radio + '</option>';
                select += '<option value="select">' + opts.messages.select + '</option>';
                // Build the control box and search button content
                box_content = '<select id="' + box_id + '" class="frmb-control">' + select + '</select>';
                save_button = '<input type="submit" id="' + save_id + '" class="frmb-submit btn btn-primary" value="' + opts.messages.save + '"/>';
                // Insert the control box into page
                if (!target) {
                    $(ul_obj).before(box_content);
                } else {
                    $(target).append(box_content);
                }
                // Insert the search button
                $(ul_obj).after(save_button);
                // Set the form save action
                $('#' + save_id).click(function () {
                    save();
                    return false;
                });
                // Add a callback to the select element
                $('#' + box_id).change(function () {
                    appendNewField($(this).val());
                    $(this).val(0).blur();
                    // This solves the scrollTo dependency
                    $('html, body').animate({
                        scrollTop: $('#frm-' + (last_id - 1) + '-item').offset().top - 100
                    }, 500);
                    return false;
                });
            }(opts.control_box_target);
            // Json parser to build the form builder
            var fromJson = function (json) {
                var values = '';
                var options = false;
                // Parse json
                $(json).each(function () {
                    // checkbox type
                    if (this.cssClass === 'checkbox') {
                        options = [this.title];
                        values = [];
                        $.each(this.values, function () {
                            values.push([this.value, this.baseline]);
                        });
                    }
                        // radio type
                    else if (this.cssClass === 'radio') {
                        options = [this.title];
                        values = [];
                        $.each(this.values, function () {
                            values.push([this.value, this.baseline]);
                        });
                    }
                        // select type
                    else if (this.cssClass === 'select') {
                        options = [this.title, this.multiple];
                        values = [];
                        $.each(this.values, function () {
                            values.push([this.value, this.baseline]);
                        });
                    }
                    else {
                        values = [this.values];
                    }
                    appendNewField(this.cssClass, values, options, this.required, this.itemclass);
                });
            };
            // Wrapper for adding a new field
            var appendNewField = function (type, values, options, required, css_class) {
                css_class = css_class || '';
                field = '';
                field_type = type;
                values = values || '';
                switch (type) {
                    case 'input_text':
                        appendTextInput(values, required, css_class);
                        break;
                    case 'textarea':
                        appendTextarea(values, required, css_class);
                        break;
                    case 'checkbox':
                        appendCheckboxGroup(values, options, required, css_class);
                        break;
                    case 'radio':
                        appendRadioGroup(values, options, required, css_class);
                        break;
                    case 'select':
                        appendSelectList(values, options, required, css_class);
                        break;
                }
            };
            // single line input type="text"
            var appendTextInput = function (values, required, css_class) {
                field += '<label>' + opts.messages.label + '</label>';
                field += '<input class="fld-title" id="title-' + last_id + '" type="text" value="' + values + '" />';
                field += '<label>' + opts.messages.css_class + '</label>';
                field += '<input class="fld-title" id="css_class-' + last_id + '" type="text" value="' + css_class + '" />';
                help = '';
                appendFieldLi(opts.messages.text, field, required, help);
            };
            // multi-line textarea
            var appendTextarea = function (values, required, css_class) {
                field += '<label>' + opts.messages.label + '</label>';
                field += '<input class="fld-title" id="title-' + last_id + '" type="text" value="' + values + '" />';
                field += '<label>' + opts.messages.css_class + '</label>';
                field += '<input class="fld-title" id="css_class-' + last_id + '" type="text" value="' + css_class + '" />';
                help = '';
                appendFieldLi(opts.messages.paragraph_field, field, required, help);
            };
            // adds a checkbox element
            var appendCheckboxGroup = function (values, options, required, css_class) {
                var title = '';
                if (typeof (options) === 'object') {
                    title = options[0];
                }
                field += '<div class="chk_group">';
                field += '<div class="frm-fld"><label>' + opts.messages.title + '</label>';
                field += '<input class="fld-title" id="title-' + last_id + '" type="text" value="' + title + '" />';
                field += '<label>' + opts.messages.css_class + '</label>';
                field += '<input class="fld-title" id="css_class-' + last_id + '" type="text" value="' + css_class + '" />';
                field += '<div class="false-label">' + opts.messages.select_options + '</div>';
                field += '<div class="fields">';
                if (typeof (values) === 'object') {
                    for (i = 0; i < values.length; i++) {
                        field += checkboxFieldHtml(values[i]);
                    }
                }
                else {
                    field += checkboxFieldHtml('');
                }
                field += '<div class="add-area"><a href="#" class="add add_ck">' + opts.messages.add + '</a></div>';
                field += '</div>';
                field += '</div>';
                help = '';
                appendFieldLi(opts.messages.checkbox_group, field, required, help);
            };
            // Checkbox field html, since there may be multiple
            var checkboxFieldHtml = function (values) {
                var checked = false;
                var value = '';
                if (typeof (values) === 'object') {
                    value = values[0];
                    checked = (values[1] === 'false' || values[1] === 'undefined' || values[1] === false) ? false : true;
                }
                field = '';
                field += '<div>';
                field += '<input type="checkbox"' + (checked ? ' checked="checked"' : '') + ' />';
                field += '<input type="text" class="item" value="' + value + '" />';
                field += '<a href="#" class="remove" title="' + opts.messages.remove_message + '">' + opts.messages.remove + '</a>';
                field += '</div>';
                return field;
            };
            // adds a radio element
            var appendRadioGroup = function (values, options, required, css_class) {
                var title = '';
                if (typeof (options) === 'object') {
                    title = options[0];
                }
                field += '<div class="rd_group">';
                field += '<div class="frm-fld"><label>' + opts.messages.title + '</label>';
                field += '<input class="fld-title" id="title-' + last_id + '" type="text" value="' + title + '" />';
                field += '<label>' + opts.messages.css_class + '</label>';
                field += '<input class="fld-title" id="css_class-' + last_id + '" type="text" value="' + css_class + '" />';
                field += '<div class="false-label">' + opts.messages.select_options + '</div>';
                field += '<div class="fields">';
                if (typeof (values) === 'object') {
                    for (i = 0; i < values.length; i++) {
                        field += radioFieldHtml(values[i], 'frm-' + last_id + '-fld');
                    }
                }
                else {
                    field += radioFieldHtml('', 'frm-' + last_id + '-fld');
                }
                field += '<div class="add-area"><a href="#" class="add add_rd">' + opts.messages.add + '</a></div>';
                field += '</div>';
                field += '</div>';
                help = '';
                appendFieldLi(opts.messages.radio_group, field, required, help);
            };
            // Radio field html, since there may be multiple
            var radioFieldHtml = function (values, name) {
                var checked = false;
                var value = '';
                if (typeof (values) === 'object') {
                    value = values[0];
                    checked = (values[1] === 'false' || values[1] === 'undefined' || values[1] === false) ? false : true;
                }
                field = '';
                field += '<div>';
                field += '<input type="radio"' + (checked ? ' checked="checked"' : '') + ' name="radio_' + name + '" />';
                field += '<input type="text" class="item" value="' + value + '" />';
                field += '<a href="#" class="remove" title="' + opts.messages.remove_message + '">' + opts.messages.remove + '</a>';
                field += '</div>';
                return field;
            };
            // adds a select/option element
            var appendSelectList = function (values, options, required, css_class) {
                var multiple = false;
                var title = '';
                if (typeof (options) === 'object') {
                    title = options[0];
                    multiple = options[1] === 'true' ? true : false;
                }
                field += '<div class="opt_group">';
                field += '<div class="frm-fld"><label>' + opts.messages.title + '</label>';
                field += '<input class="fld-title" id="title-' + last_id + '" type="text" value="' + title + '" />';
                field += '<label>' + opts.messages.css_class + '</label>';
                field += '<input class="fld-title" id="css_class-' + last_id + '" type="text" value="' + css_class + '" />';
                field += '';
                field += '<div class="false-label">' + opts.messages.select_options + '</div>';
                field += '<div class="fields">';
                field += '<input type="checkbox" name="multiple"' + (multiple ? 'checked="checked"' : '') + '>';
                field += '<label class="auto">' + opts.messages.selections_message + '</label>';
                if (typeof (values) === 'object') {
                    for (i = 0; i < values.length; i++) {
                        field += selectFieldHtml(values[i], multiple);
                    }
                }
                else {
                    field += selectFieldHtml('', multiple);
                }
                field += '<div class="add-area"><a href="#" class="add add_opt">' + opts.messages.add + '</a></div>';
                field += '</div>';
                field += '</div>';
                help = '';
                appendFieldLi(opts.messages.select, field, required, help);
            };
            // Select field html, since there may be multiple
            var selectFieldHtml = function (values, multiple) {
                if (multiple) {
                    return checkboxFieldHtml(values);
                }
                else {
                    return radioFieldHtml(values);
                }
            };
            // Appends the new field markup to the editor
            var appendFieldLi = function (title, field_html, required, help) {
                if (required) {
                    required = required === 'checked' ? true : false;
                }
                var li = '';
                li += '<li id="frm-' + last_id + '-item" class="' + field_type + '">';
                li += '<div class="legend">';
                li += '<a id="frm-' + last_id + '" class="toggle-form" href="#">' + opts.messages.hide + '</a> ';
                li += '<a id="del_' + last_id + '" class="del-button delete-confirm" href="#" title="' + opts.messages.remove_message + '"><span>' + opts.messages.remove + '</span></a>';
                li += '<strong id="txt-title-' + last_id + '">' + title + '</strong></div>';
                li += '<div id="frm-' + last_id + '-fld" class="frm-holder">';
                li += '<div class="frm-elements">';
                li += '<div class="frm-fld"><label for="required-' + last_id + '">' + opts.messages.required + '</label>';
                li += '<input class="required" type="checkbox" value="1" name="required-' + last_id + '" id="required-' + last_id + '"' + (required ? ' checked="checked"' : '') + ' /></div>';
                li += field;
                li += '</div>';
                li += '</div>';
                li += '</li>';
                $(ul_obj).append(li);
                $('#frm-' + last_id + '-item').hide();
                $('#frm-' + last_id + '-item').animate({
                    opacity: 'show',
                    height: 'show'
                }, 'slow');
                last_id++;
            };
            // handle field delete links
            $('.remove').live('click', function () {
                $(this).parent('div').animate({
                    opacity: 'hide',
                    height: 'hide',
                    marginBottom: '0px'
                }, 'fast', function () {
                    $(this).remove();
                });
                return false;
            });
            // handle field display/hide
            $('.toggle-form').live('click', function () {
                var target = $(this).attr("id");
                if ($(this).html() === opts.messages.hide) {
                    $(this).removeClass('open').addClass('closed').html(opts.messages.show);
                    $('#' + target + '-fld').animate({
                        opacity: 'hide',
                        height: 'hide'
                    }, 'slow');
                    return false;
                }
                if ($(this).html() === opts.messages.show) {
                    $(this).removeClass('closed').addClass('open').html(opts.messages.hide);
                    $('#' + target + '-fld').animate({
                        opacity: 'show',
                        height: 'show'
                    }, 'slow');
                    return false;
                }
                return false;
            });
            // handle delete confirmation
            $('.delete-confirm').live('click', function () {
                var delete_id = $(this).attr("id").replace(/del_/, '');
                if (confirm($(this).attr('title'))) {
                    $('#frm-' + delete_id + '-item').animate({
                        opacity: 'hide',
                        height: 'hide',
                        marginBottom: '0px'
                    }, 'slow', function () {
                        $(this).remove();
                    });
                }
                return false;
            });
            // Attach a callback to add new checkboxes
            $('.add_ck').live('click', function () {
                $(this).parent().before(checkboxFieldHtml());
                return false;
            });
            // Attach a callback to add new options
            $('.add_opt').live('click', function () {
                $(this).parent().before(selectFieldHtml('', false));
                return false;
            });
            // Attach a callback to add new radio fields
            $('.add_rd').live('click', function () {
                $(this).parent().before(radioFieldHtml(false, $(this).parents('.frm-holder').attr('id')));
                return false;
            });
            // saves the serialized data to the server 
            var save = function () {
                if (opts.save_url) {
                    $.ajax({
                        type: "POST",
                        url: opts.save_url,
                        data: {
                            data: $(ul_obj).serializeFormList({
                                prepend: opts.serialize_prefix,
                                get_object_id: opts.get_object_id
                            })
                        },
                        success: opts.on_saved
                    });
                }
            };
        });
    };
})(jQuery);
/**
 * jQuery Form Builder List Serialization Plugin
 * Copyright (c) 2009 Mike Botsko, Botsko.net LLC (http://www.botsko.net)
 * Originally designed for AspenMSM, a CMS product from Trellis Development
 * Licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * Copyright notice and license must remain intact for legal use
 * Modified from the serialize list plugin
 * http://www.botsko.net/blog/2009/01/jquery_serialize_list_plugin/
 */
(function ($) {
    $.fn.serializeFormList = function (options) {
        // Extend the configuration options with user-provided
        var defaults = {
            prepend: 'ul',
            is_child: false,
            attributes: ['class'],
            get_object_id: function () {
                return "";
            }
        };
        var opts = $.extend(defaults, options);
        if (!opts.is_child) {
            opts.prepend = '&' + opts.prepend;
        }
        var obj = [];
        // Begin the core plugin
        this.each(function () {
            var li_count = 0;
            $(this).children().each(function () {
                var item = {};
                for (att = 0; att < opts.attributes.length; att++) {
                    var key = (opts.attributes[att] === 'class' ? 'cssClass' : opts.attributes[att]);
                    item[key] = $(this).attr(opts.attributes[att]);
                    // append the form field values
                    if (opts.attributes[att] === 'class') {
                        var id = $(this).attr('id');
                        item.required = encodeURIComponent($('#' + id + ' input.required').attr('checked'));
                        switch ($(this).attr(opts.attributes[att])) {
                            case 'input_text':
                                item.values = $('#' + id + ' input[type=text][id^="title"]').val();
                                item.itemclass = $('#' + id + ' input[type=text][id^="css_class"]').val();
                                break;
                            case 'textarea':
                                item.values = $('#' + id + ' input[type=text][id^="title"]').val();
                                item.itemclass = $('#' + id + ' input[type=text][id^="css_class"]').val();
                                break;
                            case 'checkbox':
                                item.values = [];
                                item.title = $('#' + id + ' input[type=text][id^="title"]').val();
                                item.itemclass = $('#' + id + ' input[type=text][id^="css_class"]').val();
                                $('#' + id + ' input.item[type=text]').each(function () {
                                    var check = {};
                                    check.value = $(this).val();
                                    check.baseline = $(this).prev().is(':checked');
                                    item.values.push(check);
                                });
                                break;
                            case 'radio':
                                item.values = [];
                                item.title = $('#' + id + ' input[type=text][id^="title"]').val();
                                item.itemclass = $('#' + id + ' input[type=text][id^="css_class"]').val();
                                $('#' + id + ' input.item[type=text]').each(function () {
                                    var check = {};
                                    check.value = $(this).val();
                                    check.baseline = $(this).prev().is(':checked');
                                    item.values.push(check);
                                });
                                break;
                            case 'select':
                                item.values = [];
                                item.title = $('#' + id + ' input[type=text][id^="title"]').val();
                                item.itemclass = $('#' + id + ' input[type=text][id^="css_class"]').val();
                                item.multiple = $('#' + id + ' input[name=multiple]').attr('checked');
                                $('#' + id + ' input.item[type=text]').each(function () {
                                    var check = {};
                                    check.value = $(this).val();
                                    check.baseline = $(this).prev().is(':checked');
                                    item.values.push(check);
                                });
                                break;
                        }
                    }
                }
                li_count++;
                obj.push(item);

            });
        });
        var returnObj = {};
        returnObj.form_structure = obj;
        returnObj.document_id = opts.get_object_id();
        return (JSON.stringify(returnObj));
    };
})(jQuery);

if (typeof JSON !== "object") { JSON = {} } (function () { function f(a) { return a < 10 ? "0" + a : a } function quote(a) { escapable.lastIndex = 0; return escapable.test(a) ? '"' + a.replace(escapable, function (a) { var b = meta[a]; return typeof b === "string" ? b : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + a + '"' } function str(a, b) { var c, d, e, f, g = gap, h, i = b[a]; if (i && typeof i === "object" && typeof i.toJSON === "function") { i = i.toJSON(a) } if (typeof rep === "function") { i = rep.call(b, a, i) } switch (typeof i) { case "string": return quote(i); case "number": return isFinite(i) ? String(i) : "null"; case "boolean": case "null": return String(i); case "object": if (!i) { return "null" } gap += indent; h = []; if (Object.prototype.toString.apply(i) === "[object Array]") { f = i.length; for (c = 0; c < f; c += 1) { h[c] = str(c, i) || "null" } e = h.length === 0 ? "[]" : gap ? "[\n" + gap + h.join(",\n" + gap) + "\n" + g + "]" : "[" + h.join(",") + "]"; gap = g; return e } if (rep && typeof rep === "object") { f = rep.length; for (c = 0; c < f; c += 1) { if (typeof rep[c] === "string") { d = rep[c]; e = str(d, i); if (e) { h.push(quote(d) + (gap ? ": " : ":") + e) } } } } else { for (d in i) { if (Object.prototype.hasOwnProperty.call(i, d)) { e = str(d, i); if (e) { h.push(quote(d) + (gap ? ": " : ":") + e) } } } } e = h.length === 0 ? "{}" : gap ? "{\n" + gap + h.join(",\n" + gap) + "\n" + g + "}" : "{" + h.join(",") + "}"; gap = g; return e } } "use strict"; if (typeof Date.prototype.toJSON !== "function") { Date.prototype.toJSON = function (a) { return isFinite(this.valueOf()) ? this.getUTCFullYear() + "-" + f(this.getUTCMonth() + 1) + "-" + f(this.getUTCDate()) + "T" + f(this.getUTCHours()) + ":" + f(this.getUTCMinutes()) + ":" + f(this.getUTCSeconds()) + "Z" : null }; String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function (a) { return this.valueOf() } } var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = { "\b": "\\b", "	": "\\t", "\n": "\\n", "\f": "\\f", "\r": "\\r", '"': '\\"', "\\": "\\\\" }, rep; if (typeof JSON.stringify !== "function") { JSON.stringify = function (a, b, c) { var d; gap = ""; indent = ""; if (typeof c === "number") { for (d = 0; d < c; d += 1) { indent += " " } } else if (typeof c === "string") { indent = c } rep = b; if (b && typeof b !== "function" && (typeof b !== "object" || typeof b.length !== "number")) { throw new Error("JSON.stringify") } return str("", { "": a }) } } if (typeof JSON.parse !== "function") { JSON.parse = function (text, reviver) { function walk(a, b) { var c, d, e = a[b]; if (e && typeof e === "object") { for (c in e) { if (Object.prototype.hasOwnProperty.call(e, c)) { d = walk(e, c); if (d !== undefined) { e[c] = d } else { delete e[c] } } } } return reviver.call(a, b, e) } var j; text = String(text); cx.lastIndex = 0; if (cx.test(text)) { text = text.replace(cx, function (a) { return "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) }) } if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]").replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) { j = eval("(" + text + ")"); return typeof reviver === "function" ? walk({ "": j }, "") : j } throw new SyntaxError("JSON.parse") } } })()