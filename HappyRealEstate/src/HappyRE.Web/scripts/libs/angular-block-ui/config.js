blkUI.constant('blockUIConfig', {
    templateUrl: 'angular-block-ui/angular-block-ui.ng.html',
    delay: 250,
    message: "Vui lòng chờ",
    autoBlock: true,
    resetOnException: true,
    requestFilter: angular.noop,
    autoInjectBodyBlock: false,
    cssClass: 'block-ui block-ui-anim-fade',
    blockBrowserNavigation: false
});

