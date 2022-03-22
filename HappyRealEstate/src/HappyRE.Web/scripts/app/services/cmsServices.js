function cmsServices($http) {
    function GetNews(newsId) {
        return $http({ method: 'GET', url: mogiRoutes.Cms.GetNews + "?id=" + newsId });
    }
    function GetListNews(data) {
        return $http({ method: 'POST', url: mogiRoutes.Cms.GetListNews, data: data });
    }
    function GetCategoriesByGroup(id) {
        return $http({ method: 'POST', url: mogiRoutes.Cms.GetCategoriesByGroup + "?id=" + id });
    }
    function GetByUrlCode(groupId, urlCode) {
        return $http({ method: 'POST', url: mogiRoutes.Cms.GetByUrlCode + "?groupId=" + groupId + '&urlCode=' + urlCode });
    }

    function GetNotarizeOffices() {
        return $http({ method: 'GET', url: mogiRoutes.Cms.GetNotarizeOffices });
    }

    return {
        GetNews: GetNews,
        GetListNews: GetListNews,
        GetCategoriesByGroup: GetCategoriesByGroup,
        GetByUrlCode: GetByUrlCode,
        GetNotarizeOffices: GetNotarizeOffices
    };
}
cmsServices.$inject = ["$http"];
mogiApp.factory('cmsServices', cmsServices);
