(function() {
    $(function() {
        $('input:radio[name=DatabaseProvider]').click(toggleProvider);
        $('input:radio[name=SqlConnectionInfo]').click(toggleSqlConnectionInfo);
        $('input:radio[name=SqlAuthenticationType]').click(toggleSqlAuthenticationType);
        toggleProvider();
        toggleSqlConnectionInfo();
        toggleSqlAuthenticationType();
    });


    function toggleProvider() {
        var required = $("input[name=DatabaseProvider]:checked").data('connection-string-required');
        $('#sqlConnectionInfo').toggle(required == 'True');
    }

    function toggleSqlConnectionInfo() {
        var selectedProvider = $("input[name=SqlConnectionInfo]:checked").attr('id');
        if (selectedProvider == 'sqlconnectioninfo_values') {
            $('#sqlDatabaseInfo').show();
            $('#sqlDatabaseConnectionString').hide();
        } else if (selectedProvider == 'sqlconnectioninfo_raw') {
            $('#sqlDatabaseInfo').hide();
            $('#sqlDatabaseConnectionString').show();
        }
    }

    function toggleSqlAuthenticationType() {
        var selectedProvider = $("input[name=SqlAuthenticationType]:checked").attr('id');
        if (selectedProvider == 'sqlauthenticationtype_sql') {
            $('#pnlSqlServerUsername').show();
            $('#pnlSqlServerPassword').show();
        } else if (selectedProvider == 'sqlauthenticationtype_windows') {
            $('#pnlSqlServerUsername').hide();
            $('#pnlSqlServerPassword').hide();
        }
    }


    (function() {

        function show() {
            window.setTimeout(function() {
                document.getElementById("throbber").style.display = "block";
            }, 1000);
        }

        if (document.addEventListener) {
            document.addEventListener("submit", show, false);
        } else {
            document.forms[0].attachEvent("onsubmit", show);
        }
    })();
})();