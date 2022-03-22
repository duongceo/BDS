using System.IO;
using System.Collections.Generic;
using System.Web.Optimization;

namespace HappyRE.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            string lang = Core.Const.LANG_CULTURE;
            string route_js = "~/scripts/app/routes." + lang + ".js";

            #region Script: Default
            List<string> scripts = new List<string>();
            // modernizr
            scripts.Add("~/scripts/modernizr-*");
            // jquery & bootstrap
            scripts.AddRange(new string[] {
                "~/scripts/jquery-{version}.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/respond.js"
            });
            // libs
            scripts.AddRange(new string[] {
                "~/scripts/jquery.validate.min.js",
                "~/scripts/jquery.validate.unobtrusive.min.js",
                "~/scripts/moment.js",
                "~/scripts/underscore.js",
                "~/scripts/jquery.purl.js",
                "~/scripts/libs/angular.chosen/chosen.jquery.js",
                "~/scripts/libs/bootbox/bootbox.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/libs/owl-carousel/owl.carousel.js",
                //"~/scripts/libs/bootstrap-slider/bootstrap-slider.js",
                "~/scripts/libs/photoswipe/dist/photoswipe.js",
                "~/scripts/libs/photoswipe/dist/photoswipe-ui-default.js",
                "~/scripts/libs/clipboardjs/clipboard.js",
                "~/scripts/libs/lazyload/lozad.min.js"
            });
            // angulars
            scripts.AddRange(new string[] {
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/ui-bootstrap-tpls-2.2.0.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular.checklist-model/angular.checklist-model.js",
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/libs/angular-file-upload/ng-file-upload-shim.js",
                "~/scripts/libs/angular-file-upload/ng-file-upload.js",
                "~/scripts/libs/angular.sortable/sortable.js",
                "~/scripts/libs/chartjs/chart.js",
                "~/scripts/libs/angular-socialshare/angular-socialshare.js"
            });

            // utils
            scripts.AddRange(new string[] {
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                "~/scripts/app/routes."+lang+".js"
            });

            // app
            scripts.AddRange(new string[] {
                "~/scripts/app/app.js",
                "~/scripts/app/directives/*.js",
                "~/scripts/app/services/*.js"
            });

            #endregion

            #region Css: Defaut
            List<string> styles = new List<string>();
            styles.AddRange(new string[] {
                "~/content/font.google.css",
                "~/content/font-awesome.min.css",
                "~/Content/mogi-icon.css",
                "~/Content/mogi-font.css",
                "~/Content/mogi-new-icon.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout960.css",
                "~/Content/layout1170.css",
                "~/Content/common/sidebar.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/scripts/libs/owl-carousel/owl.carousel.min.css",
                "~/scripts/libs/owl-carousel/owl.theme.default.min.css",
                //"~/scripts/libs/bootstrap-slider/bootstrap-slider.css",
                "~/scripts/libs/toastr/toaster.css",
                "~/scripts/libs/angular-block-ui/angular-block-ui.css",
                "~/scripts/libs/photoswipe/dist/photoswipe.css",
                "~/scripts/libs/photoswipe/dist/default-skin/default-skin.css"
            });
            #endregion

            #region Common
            bundles.Add(GetScriptBundle("~/bundles/common", scripts));
            bundles.Add(GetStyleBundle("~/content/css", styles));
            bundles.Add(GetScriptBundle("~/content/common.js",
                "~/scripts/jquery-1.10.2.min.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.min.js",
                "~/scripts/underscore.min.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js"));
            #endregion

            #region home 
            bundles.Add(GetStyleBundle("~/content/home.css",
                "~/Content/mogi-icon.css",
                "~/content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout1170.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/Content/home/home.mobile.css",
                "~/Content/home/home.desktop.css"));

            bundles.Add(GetScriptBundle("~/content/home.js",
                "~/scripts/jquery-1.10.2.min.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.min.js",
                "~/scripts/underscore.min.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/services/suggestServices.js",
                "~/scripts/app/controllers/homeController.js"));

            //bundles.Add(GetScriptBundle("~/bundles/home_mobile",
            //    "~/scripts/jquery-1.10.2.min.js",
            //    "~/scripts/jquery.cookie.js",
            //    "~/scripts/bootstrap.min.js",
            //    "~/scripts/underscore.min.js",
            //    "~/scripts/libs/lazyload/lozad.min.js",
            //    "~/scripts/angular.min.js",
            //    "~/scripts/angular-locale_vi-vn.js",
            //    "~/scripts/angular-sanitize.js",
            //    "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
            //    //"~/scripts/app/const.js",
            //    "~/res/dynjs/mogi-res-common.js",
            //    "~/scripts/app/resourceUtils.js",
            //    "~/scripts/app/utils.js",
            //    route_js,
            //    "~/scripts/app/app.js"));

            bundles.Add(GetStyleBundle(
                "~/content/home_buyingguide", styles,
                "~/Content/home/buyingguidev2.css"));

            bundles.Add(GetScriptBundle(
                "~/bundles/home_buyingguide", scripts,
                "~/scripts/app/controllers/buyingguideController.js"
                ));

            #endregion

            #region listing & detail
            bundles.Add(GetStyleBundle("~/content/mapcss",
                "~/Content/font.google.css",
                "~/Content/mogi-icon.css",
                "~/Content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout960.css",
                //"~/Content/common/sidebar.css",
                "~/Content/site.css",
                //"~/Content/banner.css",
                //"~/Content/footer.css",
                //"~/scripts/libs/owl-carousel/owl.carousel.min.css",
                //"~/scripts/libs/owl-carousel/owl.theme.default.min.css",
                //"~/scripts/libs/toastr/toaster.css",
                //"~/scripts/libs/angular-block-ui/angular-block-ui.css",
                //"~/scripts/libs/photoswipe/dist/photoswipe.css",
                //"~/scripts/libs/photoswipe/dist/default-skin/default-skin.css",
                //"~/Content/list.*",
                "~/Content/maps/bds-map.css",
                "~/Content/map.*",
                "~/Content/property/list.css"));

            bundles.Add(GetStyleBundle("~/content/list.css",
                "~/Content/mogi-icon.css",
                "~/Content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout1170.css",
                "~/Content/common/sidebar.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/scripts/libs/owl-carousel/owl.carousel.min.css",
                "~/scripts/libs/owl-carousel/owl.theme.default.min.css",
                "~/Content/property/listv3.css"));

            bundles.Add(GetScriptBundle("~/content/list.js",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/libs/owl-carousel/owl.carousel.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/directives/mgCarousel.js",
                "~/scripts/app/services/commonServices.js",
                "~/scripts/app/services/propertyService.js",
                "~/scripts/app/services/suggestServices.js",
                "~/scripts/app/services/userProfileServices.js",
                "~/scripts/app/controllers/listViewV2Controller.js"));


            bundles.Add(GetScriptBundle("~/bundles/mapv2",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/libs/owl-carousel/owl.carousel.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/services/commonServices.js",
                "~/scripts/app/services/propertyService.js",
                "~/scripts/app/services/suggestServices.js",
                "~/scripts/app/services/userProfileServices.js",
                "~/scripts/app/controllers/listViewV2Controller.js",
               "~/scripts/map/maps.js",
               "~/scripts/map/bds-map.js"));


            bundles.Add(GetStyleBundle("~/content/detail.css",
                "~/Content/mogi-icon.css",
                "~/Content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                //"~/Content/layout960.css",
                "~/Content/layout1170.css",
                "~/Content/common/sidebar.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/scripts/libs/owl-carousel/owl.carousel.min.css",
                "~/scripts/libs/owl-carousel/owl.theme.default.min.css",
                "~/scripts/libs/toastr/toaster.css",
                "~/scripts/libs/angular-block-ui/angular-block-ui.css",
                "~/scripts/libs/photoswipe/dist/photoswipe.css",
                "~/scripts/libs/photoswipe/dist/default-skin/default-skin.css",
                //"~/Content/list.destop-similar-box.css",
                "~/content/property/detail.mobile.css",
                "~/content/property/detail.desktop.css"));


            bundles.Add(GetScriptBundle("~/content/detail.js",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/libs/owl-carousel/owl.carousel.js",
                "~/scripts/libs/photoswipe/dist/photoswipe.js",
                "~/scripts/libs/photoswipe/dist/photoswipe-ui-default.js",
                "~/scripts/libs/clipboardjs/clipboard.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/chartjs/chart.js",
                "~/scripts/libs/angular-socialshare/angular-socialshare.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/directives/angular-photoswipe.js",
                "~/scripts/app/directives/chartMogi.js",
                "~/scripts/app/directives/mgCarousel.js",
                "~/scripts/app/directives/mgSocial.js",
                "~/scripts/app/directives/lazyScroll.js",
                "~/scripts/app/services/agentServices.js",
                "~/scripts/app/services/propertyService.js",
                "~/scripts/app/controllers/propertyDetailController.js"));


            bundles.Add(GetScriptBundle(
               "~/bundles/test2", scripts,
               "~/scripts/map/mapUtil.js"));

            bundles.Add(GetStyleBundle(
               "~/content/studentcss", styles,
               "~/Content/property/student.css"));

            bundles.Add(GetScriptBundle(
                "~/bundles/student", scripts,
                "~/res/dynjs/mogi-res-colleges.js",
                "~/scripts/app/controllers/studentController.js"));

            bundles.Add(GetScriptBundle(
                "~/bundles/industrialpark", scripts,
                "~/res/dynjs/mogi-res-industrial-park.js",
                "~/scripts/app/controllers/industrialParkController.js"));


            #endregion

            #region Agent: listing & detail
            bundles.Add(GetStyleBundle("~/content/agentlist.css",
                "~/Content/mogi-icon.css",
                "~/Content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout1170.css",
                "~/Content/common/sidebar.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/scripts/libs/owl-carousel/owl.carousel.min.css",
                "~/scripts/libs/owl-carousel/owl.theme.default.min.css",
                "~/Content/agent/list.css"));

            bundles.Add(GetScriptBundle("~/content/agentlist.js", scripts,
                "~/scripts/app/controllers/agentListingController.js"
                ));

            bundles.Add(GetScriptBundle("~/content/agentdetail.js", scripts,
                "~/scripts/app/controllers/agentDetailController.js"
                ));

            bundles.Add(GetStyleBundle("~/content/agentdetail.css", styles,
                "~/Content/agent/detail.css"));
            #endregion

            #region CMS
            bundles.Add(GetStyleBundle(
                "~/content/mogicms", styles,
                "~/Content/cms.desktop.css"));

            bundles.Add(new StyleBundle("~/content/notarizeOffice").Include(
               "~/Content/cms.desktop.css",
               "~/Content/cms/notarizeOffice.css",
               "~/Content/core/layout.css"));

            bundles.Add(new ScriptBundle("~/bundles/notarizeOffice").Include(
                "~/scripts/app/controllers/notarizeOfficeController.js"));
            #endregion

            bundles.Add(GetStyleBundle("~/content/account.css",
                "~/Content/mogi-icon.css",
                "~/content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout1170.css",
                "~/Content/site.css",
                "~/Content/banner.css",
                "~/Content/footer.css",
                "~/scripts/libs/toastr/toaster.css",
                "~/scripts/libs/angular.chosen/bootstrap-chosen.css",
                "~/Content/login.desktop.css"));

            bundles.Add(new StyleBundle("~/Content/profilecss").Include(
                "~/scripts/libs/toastr/toaster.css",
                  "~/Content/profile.desktop.css"));

            bundles.Add(new StyleBundle("~/content/branchoffice").Include(
                "~/scripts/libs/angular.chosen/bootstrap-chosen.css",
                "~/Content/cms.desktop.css",
                "~/Content/cms/branchoffice.css",
                "~/Content/core/layout.css"));

            bundles.Add(new ScriptBundle("~/bundles/branchoffice").Include(
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/app/controllers/branchOfficeController.js"));

            bundles.Add(new StyleBundle("~/content/feedback.css").Include(
                "~/Content/cms.desktop.css",
                "~/Content/home/feedback.css",
                "~/Content/core/layout.css"));

            bundles.Add(new ScriptBundle("~/content/feedback.js").Include(
                "~/scripts/app/controllers/feedbackController.js"));

            bundles.Add(new StyleBundle("~/Content/profileFavorite").Include(
                "~/Content/profile/favorite.css",
                 "~/Content/core/layout.css"));


            bundles.Add(GetScriptBundle("~/bundles/profile",
                "~/scripts/jquery-1.10.2.min.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.min.js",
                "~/scripts/underscore.min.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular-file-upload/ng-file-upload-shim.js",
                "~/scripts/libs/angular-file-upload/ng-file-upload.js",
                "~/scripts/libs/angular.sortable/sortable.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/services/commonServices.js",
                "~/scripts/app/services/userProfileServices.js",
                "~/scripts/app/services/imageServices.js",
                "~/scripts/app/services/propertyService.js",
                "~/scripts/app/controllers/userProfileController.js"));

            bundles.Add(new ScriptBundle("~/bundles/account").Include(
                "~/scripts/app/controllers/accountController.js"));

            bundles.Add(GetScriptBundle("~/content/account.js",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular.chosen/chosen.jquery.js",
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/services/userProfileServices.js",
                "~/scripts/app/controllers/accountController.js"));


            #region landingpage          
            List<string> stylesLanding = new List<string>();
            stylesLanding.AddRange(new string[] {
                "~/Content/bootstrap.css",
                "~/Content/landingpage/landingpage.css",
                "~/Content/landingpage/landingpage.mobile.css"
            });
            List<string> stylesLandingEn = new List<string>();
            stylesLandingEn.AddRange(new string[] {
                "~/Content/bootstrap.css",
                "~/Content/landingpage/landingpageen.css",
                "~/Content/landingpage/landingpageen.mobile.css"
            });
            List<string> stylesLanding3 = new List<string>();
            stylesLanding3.AddRange(new string[] {
                "~/Content/bootstrap.css",
                "~/Content/landingpage/landing3.css",
                "~/Content/landingpage/landing3.mobile.css"
            });
            List<string> scriptsLanding = new List<string>();
            // modernizr
            scriptsLanding.Add("~/scripts/modernizr-*");
            // jquery & bootstrap
            scriptsLanding.AddRange(new string[] {
                "~/scripts/jquery-{version}.js",
                "~/scripts/bootstrap.js",
                "~/scripts/respond.js"
            });
            // libs
            scriptsLanding.AddRange(new string[] {
                "~/scripts/libs/bootbox/bootbox.js",

            });
            // angulars
            scriptsLanding.AddRange(new string[] {
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/ui-bootstrap-tpls-2.2.0.js"
            });
            // app
            scriptsLanding.AddRange(new string[] {
                "~/scripts/app/appLanding.js",
                //"~/scripts/app/directives/*.js",
                //"~/scripts/app/services/*.js"
            });

            List<string> stylesLanding4 = new List<string>();
            stylesLanding4.AddRange(new string[] {
                 "~/content/font.google.css",
                "~/content/font-awesome.min.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/layout960.css",
                "~/Content/landingpage/landing4.css"});

            // Common
            bundles.Add(GetScriptBundle("~/bundles/landingjs", scriptsLanding));
            bundles.Add(GetStyleBundle("~/content/landingcss", stylesLanding));
            bundles.Add(GetStyleBundle("~/content/landingcssen", stylesLandingEn));
            bundles.Add(GetStyleBundle("~/content/landing3css", stylesLanding3));
            bundles.Add(GetStyleBundle("~/content/landing4css", stylesLanding4));
            #endregion

            #region marketprice
            bundles.Add(GetStyleBundle(
                 "~/content/marketPriceSearch", styles,
                 "~/Content/market/search.css",
                 "~/Content/list.destop-similar-box.css"));

            bundles.Add(GetScriptBundle(
               "~/bundles/marketPrice", scripts,
               "~/scripts/app/controllers/marketPriceController.js"
               ));
            bundles.Add(GetScriptBundle(
               "~/bundles/market", scripts,
               "~/scripts/app/controllers/marketController.js"
               ));

            bundles.Add(GetStyleBundle(
               "~/content/marketPriceDetail", styles,
               "~/Content/market/detail.css",
               "~/Content/list.destop-similar-box.css"));

            bundles.Add(GetStyleBundle(
              "~/content/marketDetail", styles,
              "~/scripts/libs/angular.chosen/bootstrap-chosen.css",
              "~/Content/market/marketdetail.css"));

            bundles.Add(GetStyleBundle(
             "~/content/marketDetail", styles,
             "~/scripts/libs/angular.chosen/bootstrap-chosen.css",
             "~/Content/market/marketdetail.css"));

            bundles.Add(GetStyleBundle(
             "~/content/marketStreetList", styles,
             "~/scripts/libs/angular.chosen/bootstrap-chosen.css",
             "~/Content/layout1170.css",
             "~/Content/market/street.css"));
            #endregion


            bundles.Add(GetStyleBundle(
                 "~/content/marketlist", styles,
                 "~/Content/market/marketlist.css"
                 //"~/Content/market/marketlist.mobile.css",
                 /*"~/Content/list.destop-similar-box.css"*/));

            #region Projects
            bundles.Add(GetStyleBundle("~/content/projectlist.css", styles,
                "~/Content/project/listv2.css",
                "~/Content/list.destop-similar-box.css",
                "~/Content/banner.css"));

            //bundles.Add(GetScriptBundle("~/content/projectlist.js", scripts,"~/scripts/app/controllers/projectsController.js" ));
            bundles.Add(GetScriptBundle("~/content/projectlist.js",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-messages.js",
                "~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/libs/angular.chosen/chosen.jquery.js",
                "~/scripts/libs/angular.chosen/angular-chosen.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/services/suggestServices.js",
                "~/scripts/app/controllers/projectsController.js"));

            bundles.Add(GetStyleBundle("~/content/projectdetail.css", styles,
                "~/Content/project/detailv2.css",
                "~/Content/project/project.css"));


            bundles.Add(GetScriptBundle("~/content/projectdetail.js",
                "~/scripts/jquery-1.10.2.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.js",
                "~/scripts/underscore.js",
                "~/scripts/libs/toastr/toastr.js",
                "~/scripts/libs/owl-carousel/owl.carousel.js",
                "~/scripts/libs/photoswipe/dist/photoswipe.js",
                "~/scripts/libs/photoswipe/dist/photoswipe-ui-default.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                //"~/scripts/angular-messages.js",
                //"~/scripts/angular-animate.js",
                "~/scripts/angular-sanitize.js",
                //"~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                //"~/scripts/libs/angular-block-ui/angular-block-ui.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                "~/scripts/app/directives/angular-photoswipe.js",
                "~/scripts/app/directives/mgCarousel.js",
                "~/scripts/map/mapUtil.js",
                "~/scripts/app/controllers/projectDetailController.js"
                ));
            #endregion

            #region Ward Review
            bundles.Add(GetStyleBundle("~/content/wardreviewcity.css",
                "~/Content/mogi-icon.css",
                "~/content/mogi-font.css",
                "~/Content/bootstrap.css",
                "~/Content/bootstrap.ext.css",
                "~/Content/image.css",
                "~/Content/layout1170.css",
                "~/Content/site.css",
                "~/Content/footer.css",
                "~/Content/wardReview/city.css"));

            bundles.Add(GetStyleBundle("~/content/wardreviewdistrict.css",
               "~/Content/mogi-icon.css",
               "~/content/mogi-font.css",
               "~/Content/bootstrap.css",
               "~/Content/bootstrap.ext.css",
               "~/Content/image.css",
               "~/Content/layout1170.css",
               "~/Content/site.css",
               "~/Content/footer.css",
               "~/Content/wardReview/district.css"));

            bundles.Add(GetStyleBundle("~/content/wardreviewwards.css",
              "~/Content/mogi-icon.css",
              "~/content/mogi-font.css",
              "~/Content/bootstrap.css",
              "~/Content/bootstrap.ext.css",
              "~/Content/image.css",
              "~/Content/layout1170.css",
              "~/Content/site.css",
              "~/Content/footer.css",
              "~/Content/wardReview/wards.css"));

            bundles.Add(GetStyleBundle("~/content/wardreviewward.css",
              "~/Content/mogi-icon.css",
              "~/content/mogi-font.css",
              "~/Content/bootstrap.css",
              "~/Content/bootstrap.ext.css",
              "~/Content/image.css",
              "~/Content/layout1170.css",
              "~/Content/site.css",
              "~/Content/footer.css",
              "~/scripts/libs/owl-carousel/owl.carousel.min.css",
              "~/scripts/libs/owl-carousel/owl.theme.default.min.css",
              "~/scripts/libs/photoswipe/dist/photoswipe.css",
              "~/scripts/libs/photoswipe/dist/default-skin/default-skin.css",
              "~/Content/wardReview/ward.css"));
            //bundles.Add(GetStyleBundle(
            //   "~/content/wardReviecss", styles,
            //   "~/Content/wardReview/ward.css"));
            bundles.Add(GetScriptBundle("~/content/wardReview.js",
                "~/scripts/jquery-1.10.2.min.js",
                "~/scripts/jquery.cookie.js",
                "~/scripts/bootstrap.min.js",
                "~/scripts/underscore.min.js",
                "~/scripts/libs/lazyload/lozad.min.js",
                "~/scripts/libs/owl-carousel/owl.carousel.js",
                "~/scripts/libs/photoswipe/dist/photoswipe.js",
                "~/scripts/libs/photoswipe/dist/photoswipe-ui-default.js",
                "~/scripts/angular.min.js",
                "~/scripts/angular-locale_vi-vn.js",
                "~/scripts/angular-sanitize.js",
                "~/scripts/libs/ui-bootstrap/ui-bootstrap-custom-tpls-2.5.0.min.js",
                "~/scripts/app/const.js",
                "~/res/dynjs/mogi-res-common.js",
                "~/scripts/app/resourceUtils.js",
                "~/scripts/app/utils.js",
                route_js,
                "~/scripts/app/app.js",
                 "~/scripts/app/directives/angular-photoswipe.js",
                "~/scripts/app/directives/mgCarousel.js",
                "~/scripts/app/controllers/wardReviewController.js"));

            //bundles.Add(GetScriptBundle(
            //  "~/bundles/wardReview", scripts,
            //  "~/scripts/app/controllers/wardReviewController.js"));

            #endregion

            BundleTable.EnableOptimizations = MBN.Utils.WebUtils.AppSettings("EnableOptimizations", false);
        }

        private static Bundle GetScriptBundle(string virtualPath, List<string> lst)
        {
            var res = new ScriptBundle(virtualPath);
            res.Orderer = new NoneBundleOrderer();
            return res.Include(lst.ToArray());
        }

        private static Bundle GetScriptBundle(string virtualPath, List<string> lst, params string[] paths)
        {
            var res = new ScriptBundle(virtualPath);
            res.Orderer = new NoneBundleOrderer();
            return res.Include(lst.ToArray()).Include(paths);
        }

        private static Bundle GetScriptBundle(string virtualPath, params string[] paths)
        {
            var res = new ScriptBundle(virtualPath);
            res.Orderer = new NoneBundleOrderer();
            return res.Include(paths);
        }

        private static Bundle GetStyleBundle(string virtualPath, params string[] lst)
        {
            return new StyleBundle(virtualPath).Include(lst);
        }

        private static Bundle GetStyleBundle(string virtualPath, List<string> lst)
        {
            return new StyleBundle(virtualPath).Include(lst.ToArray());
        }

        private static Bundle GetStyleBundle(string virtualPath, List<string> lst, params string[] paths)
        {
            var res = new StyleBundle(virtualPath);
            res.Orderer = new NoneBundleOrderer();
            return res.Include(lst.ToArray()).Include(paths);
        }
    }

    public class NoneBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
