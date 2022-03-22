function agent($http) {
    return {
        SendMessageToAgent: SendMessageToAgent
    };

	function SendMessageToAgent(data) {
		return $http({ method: 'POST', url: mogiRoutes.Agent.SendMessageToAgent, data: data });
	}
}
agent.$inject = ["$http"];
mogiApp.factory('agentServices', agent);