mogiApp.controller('cmsController', ['$scope','$sce','$filter', 'cmsServices', 'blockUI', function ($scope,$sce,$filter, cmsServices, blockUI) {
    $scope.CMS_News = {
        NewsId: 0,
        CategoryId: null,
        SEOId: 0,
        Code: '',
        CodeUrl: '',
        Subject: '',
        Summary: '',
        HtmlBody: '',
        CoverImage: '',
        Images: '',
        HasImage: false,
        Publish: false,
        SEOTile: '',
        SEODescription: '',
        SEOKeyword: ''
    };
    $scope.Query = {
        Keyword: '',
        ParentId: 0,
        CategoryId: 0,
        Publish: 1,
        PageIndex: 1,
        PageSize: 10
    };

    $scope.GetListNews = function (id) {
        $scope.Query.CategoryId = id;
        GetListNews();        
    };

    function GetListNews() {
		cmsServices.GetListNews($scope.Query).then(function (rp) {
			rp = rp.data;
             if (rp.Status == true) {
                 var data = rp.Data;
                 if (data.Total == 1) {
                     $scope.goDetail(data.Data[0].NewsId);
                 } else {
                     $scope.TotalItem = data.Total;
                     $scope.CMS_ListNews = data.Data;
                 }
             }
         });
    };

    $scope.PageChanged = function () {
        GetListNews();
    };

    $scope.goDetail = function (id) {
        if (id == null) return;
        window.location = mogiRoutes.Cms.NewsDetail + '/' + id;
    };
    $scope.getNewsDetail = function (id) {
        blockUI.start();
		cmsServices.GetNews(id).then(function (rp) {
			rp = rp.data;
            blockUI.stop();
            if (rp.Status == true) {
                $scope.CMS_News = rp.Data.News;
                if ($scope.CMS_News.NewsId > 0) {
                    $scope.CMS_News.HtmlBody = $sce.trustAsHtml($scope.CMS_News.Body);
                    $scope.CMS_News.SEOTitle = rp.Data.Seo.Title;
                    $scope.CMS_News.SEODescription = rp.Data.Seo.Description;
                    $scope.CMS_News.SEOKeyword = rp.Data.Seo.Keyword;
                }
            }
        });
    };

    $scope.GetCategoriesByGroup = function (groupId) {
        mogiUtils.ScrollToTop();
		cmsServices.GetCategoriesByGroup(groupId).then(function (rp) {
			rp = rp.data;
             if (rp.Status == true) {
                 $scope.CMS_Categories = rp.Data;
             }
         });
    };

    $scope.cateHasChild = function (item) {
        var a= $filter('filter')($scope.CMS_Categories, function (d) { return d.ParentId === item.CategoryId; });
        return a.length > 0;
    }

}]);


mogiApp.filter('imgUrlFromJson',[function () {
    return function (x) {
        if (x && x != null) {
            return angular.fromJson(x).Url;
        }
        return '';
    }
}]);