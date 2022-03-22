function mbnAgentServices($http) {
    var service = {
        GetLocationAgent: GetLocationAgent,
        GetMBNAgents: GetMBNAgents
    };
    return service;

    function GetLocationAgent(cityId) {
        return $http({ method: 'GET', url: mogiRoutes.MBNAgent.GetLocationAgent + cityId });
    };

    function GetMBNAgents(cityId, districtId) {
        return $http({ method: 'GET', url: mogiRoutes.MBNAgent.GetMBNAgents + '?cityId=' + cityId + '&districtId=' + districtId });
    };
}
mbnAgentServices.$inject = ['$http'];
mogiApp.factory('mbnAgentServices', mbnAgentServices);