// validation
$(function ($) {
    var validator = $['validator'],
        adapters = validator['unobtrusive']['adapters'];


    validator['addMethod']('release',
        function (value, element, params) {
            var year = params[0], releaseYear = parseInt(value, 10);

            // We can opt for [Remote] validation if we don't have trust in client side date
            var currentYear = new Date().getFullYear();

            return releaseYear >= year && releaseYear <= currentYear;
        });

    adapters['add']('release',
        ['year'],
        function (options) {
            options['rules']['release'] = [parseInt(options['params']['year'], 10)];
            options['messages']['release'] = options['message'];
        });

    validator['addMethod']('birthday',
        function (value, element, params) {
            var timestamp = Date.parse(value);
           var dateObject = new Date(timestamp);

            var now = Date.now();

            return dateObject <= now;
        });

    adapters['add']('birthday',
        [],
        function (options) {
            options['rules']['birthday'] = options['params'];
            options['messages']['birthday'] = options['message'];
        });

}(jQuery));