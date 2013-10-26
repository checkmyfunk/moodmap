$(function () {

    window.viewModel = (function (ko, datacontext) {

        function Mood(data) {
            var self = this;

            self.id = ko.observable(data.id);
            self.name = ko.observable(data.name == null ? '' : data.name);
            self.image = ko.observable(data.image == null ? '' : data.image);
            self.score = ko.observable(data.score == null ? '' : data.score);
        }

        function Country(data) {
            var self = this;

            self.id = ko.observable(data.id);
            self.name = ko.observable(data.name == null ? '' : data.name);
            self.abbreviation = ko.observable(data.abbreviation == null ? '' : data.abbreviation);
            self.score = ko.observable(data.score == null ? '' : data.score);
        }

        function ViewModel() {
            var self = this;

            self.mood = ko.observable();
            self.country = ko.observable();
            self.moods = ko.observableArray();
            self.countries = ko.observableArray();

            self.save = function () {
                //if (self.selectedPlaceholder() != null && self.selectedLanguage() != null) {
                //    self.applyMask(document.body);
                //    $.ajax(WEBROOT + 'api/resource/', {
                //        data: ko.toJSON({ Language: self.selectedLanguage(), Placeholder: self.selectedPlaceholder(), Text: self.textDirty() }),
                //        type: 'POST',
                //        contentType: "application/json",
                //        success: function (result) {
                //            self.removeMask(document.body);
                //            if (!result)
                //                alert('Server error. Can\'t save placeholder value.');
                //            else {
                //                self.resources()[self.selectedPlaceholder()][self.selectedLanguage()] = self.textDirty();
                //                self.text(self.textDirty());
                //            }
                //        }
                //    });
                //}
            };

            //self.applyMask = function (selector, transparent) {
            //    APPLY_MASK = true;
            //    setTimeout(function () { self.applyMaskFinish(selector, transparent) }, 100);
            //};
            //self.applyMaskFinish = function (selector, transparent) {
            //    if (APPLY_MASK) {
            //        var container = $(selector);

            //        if (container.children('.pleasewait').length > 0)
            //            return;

            //        container.append(
            //            '<div class="progress"> \
            //                <i class="icon-spinner icon-spin icon-large" style="vertical-align: top; font-size: 20px;"></i>&nbsp;&nbsp; \
            //                <span style="font-weight: bold; font-size: 19px;">Please wait...</span> \
            //            </div>');

            //        var transparencyStyle = (transparent != undefined && transparent != null && transparent) ? 'style="background-color: transparent"' : '';
            //        container.append('<div class="overlay" ' + transparencyStyle + '>&nbsp;</div>');

            //        var spinner = container.children('.progress');
            //        var containerMeasurer = selector == document.body || selector == 'body' ? $(window) : container;
            //        spinner
            //            .css('top', container.get(0).scrollTop + ((containerMeasurer.height() - spinner.height()) / 2.3))
            //            .css('left', container.get(0).scrollLeft + ((containerMeasurer.width() - spinner.width()) / 1.9));
            //        container.children('.overlay')
            //            .height(container.get(0).scrollHeight)
            //            .width(container.get(0).scrollWidth);
            //    }

            //    APPLY_MASK = false;
            //};
            //self.removeMask = function (selector) {
            //    APPLY_MASK = false;
            //    $(selector).children('.progress').remove();
            //    $(selector).children('.overlay').remove();
            //};
        }

        return new ViewModel();

    })(ko, null);
});