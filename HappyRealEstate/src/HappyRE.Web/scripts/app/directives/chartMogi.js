mogiApp.directive('tcChartjs', function () {
    return {
        restrict: 'A',
        scope: {
            data: '=chartData',
            options: '=chartOptions',
            plugins: '=chartPlugins',
            type: '@chartType',
            legend: '=?chartLegend',
            chart: '=?chart',
            click: '&chartClick'
        },
        link: function ($scope, $elem, $attrs) {
            var chartType = null;
            var ctx = $elem[0].getContext('2d');
            var chartObj;
            var showLegend = false;
            var autoLegend = false;
            var exposeChart = false;
            var legendElem = null;

            for (var attr in $attrs) {
                if (attr === 'chartLegend') {
                    showLegend = true;
                } else if (attr === 'chart') {
                    exposeChart = true;
                } else if (attr === 'autoLegend') {
                    autoLegend = true;
                }
            }

            $scope.$on('$destroy', function () {
                if (chartObj && typeof chartObj.destroy === 'function') {
                    chartObj.destroy();
                }
            });

            if ($scope.click) {
                $elem[0].onclick = function (evt) {
                    var out = {
                        chartEvent: evt,
                        element: chartObj.getElementAtEvent(evt),
                        elements: chartObj.getElementsAtEvent(evt),
                        dataset: chartObj.getDatasetAtEvent(evt)
                    };

                    $scope.click({ event: out });
                };
            }

            $scope.$watch('[data, options, plugins]', function (value) {
                if (value && $scope.data) {
                    if (chartObj && typeof chartObj.destroy === 'function') {
                        chartObj.destroy();
                    }

                    var type = chartType || $scope.type;
                    if (!type) {
                        throw 'Error creating chart: Chart type required.';
                    }
                    type = cleanChartName(type);
                    chartObj = new Chart(ctx, {
                        type: type,
                        data: angular.copy($scope.data),
                        options: $scope.options,
                        plugins: $scope.plugins
                    });
                   
                    if (showLegend) {
                        $scope.legend = chartObj.generateLegend();
                    }

                    if (autoLegend) {
                        if (legendElem) {
                            legendElem.remove();
                        }
                        angular.element($elem[0]).after(chartObj.generateLegend());
                        legendElem = angular.element($elem[0]).next();
                    }

                    if (exposeChart) {
                        $scope.chart = chartObj;
                    }
                    chartObj.resize();
                }
            }, true);
        }
    };
});

function cleanChartName(type) {
    var typeLowerCase = type.toLowerCase();
    switch (typeLowerCase) {
        case 'polararea':
            return 'polarArea';
        case 'horizontalbar':
            return 'horizontalBar';
        default:
            return type;
    }
}