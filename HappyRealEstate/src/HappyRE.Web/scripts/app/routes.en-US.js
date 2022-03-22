﻿var webAPIUrl = mogiDatas.WebAPIUrl;
var mogiRoutes = {
	loginUrl: '/signin',
    City: {
        GetStreetById: '/city/getstreetbyid',
        GetZoneByCityId: '/city/getzonebycityid'
    },
    Agent: {
        SendMessageToAgent: '/Agent/SendMessageToAgent',
        SearchAgent: '/find-agent/',
        SearchAgentKeyword: '/find-agent?q='
    },
    Map: {
        MapView: '/LocationMap/MapView',
        GetMap: '/LocationMap/GetMap',
        GetMaps: '/LocationMap/GetMaps'
    },
    Manage: {
        UpdateLegal: '/Manage/UpdateLegal',
        LegalView: '/Manage/LegalView',
        DeletedLegal: '/Manage/DeletedLegal',
        PropertyTypeView: '/Manage/PropertyTypeView',
        DeletedPropertyType: '/Manage/DeletedPropertyType',
        UpdatePropertyType: '/Manage/UpdatePropertyType',
        UpdateDataPropertyType: '/Manage/UpdateDataPropertyType',
        UpdateDirection: '/Manage/UpdateDirection',
        DeletedDirection: '/Manage/DeletedDirection',
        DirectionView: '/Manage/DirectionView',
        LevelDetailView: '/Manage/LevelDetailView',
        LevelView: '/Manage/LevelView',
        UpdateLevelDetail: '/Manage/UpdateLevelDetail',
        DeleteLevelDetail: '/Manage/DeleteLevelDetail',
        UpdateLevel: '/Manage/UpdateLevel',
        DeleteLevel: '/Manage/DeleteLevel',
        CurrencyView: '/Manage/CurrencyView',
        AddCurrency: '/Manage/AddCurrency',
        UpdateCurrency: '/Manage/UpdateCurrency',
        DeleteCurrency: '/Manage/DeleteCurrency',
        GetStreetName: '/Manage/GetStreetName',
        StreetNameView: '/Manage/StreetNameView',
        UpdateStreetName: '/Manage/UpdateStreetName',
        DeleteStreetName: '/Manage/DeleteStreetName',
        GetAutocompleteStreetName: '/Manage/GetAutocompleteStreetName',
        StreetView: '/Manage/StreetView',
        AddStreet: '/Manage/AddStreet',
        AddStreetName: '/Manage/AddStreetName',
        GetStreets: '/Manage/GetStreets',
        UpdateStreet: '/Manage/UpdateStreet',
        DeletedStreet: '/Manage/DeletedStreet',
        SysCodeView: '/Manage/SysCodeView',
        AddSysCode: '/Manage/AddSysCode',
        GetSysCodes: '/Manage/GetSysCodes',
        UpdateSysCode: '/Manage/UpdateSysCode',
        DeleteSysCode: '/Manage/DeleteSysCode',
        GetStreetByDistrict: '/Manage/GetStreetByDistrict',
    },
    Upload: {
        UploadImage: '/Upload/UploadImage'
    },
    Project: {
        SearchProject: "/du-an/",
        SearchProjectKeyword: "/du-an?q="
    },
    Property: {
        ListView: '/',
        MapViewData: '/Property/MapViewData',
		ListViewByIds: '/Property/ListViewByIds',
		ListingByIds: '/Property/ListingByIds',
        ReportMessages: '/Property/ReportMessages',
        ReportAbuse: '/Property/ReportAbuse',
		TopService: '/Property/TopService',
		GetNext: '/property/getnext',
        GetBankInterestRates: '/Property/GetBankInterestRates',
        GetPropertyByStreet: '/MarketPrice/GetPropertyByStreet',
        GetPropertyByUniqueAddress: '/MarketPrice/GetPropertyByUniqueAddress'
    },
    Common: {
		Suggest: webAPIUrl + '/api/common/suggest-map',
		Suggest_Agent: webAPIUrl + '/api/common/suggest-agent',
		Favorite_Add: webAPIUrl + '/api/common/favorite-add',
		Favorite_Remove: webAPIUrl + '/api/common/favorite-remove',
		Favorite_GetList: webAPIUrl + '/api/common/favorite-get-list',
		Suggest_Project: webAPIUrl + '/api/common/suggest-project',
		Suggest_Street: webAPIUrl + '/api/common/suggest-street',
		Profile_GetInfo: webAPIUrl + '/api/common/profile-get-info',
		Profile_GetInbox: webAPIUrl + '/api/common/profile-get-inbox',
		GetMessage: '/common/getmessage',
		FeedBack: webAPIUrl + '/common/feedback'
    },
    Profile: {
		login: '/account/login',
		updateUser: '/account/updateprofile',
        verifiedOTP: '/account/verified',
        accountKitVerifySms: '/account/accountkitverifysms',
        ackLoginSuccess: '/account/accountkitloginsuccess',
		ackLoginFailed: '/account/accountkitloginfailed',
		registerMogiPro: '/account/registermogipro',
		resetPassword: '/account/resetpassword',
        sendVerifyEmail: '/profile/sendverifyemail',
		updateEmail: '/profile/UpdateEmail',
		getInfo: '/profile/getinfo',
		getMessage: '/profile/getmessage',
		getInbox: '/profile/getinbox',
        getProfile: '/profile/GetProfile',
        getUserData: '/profile/GetUserData',
        updateProfile: '/profile/UpdateProfile',        
        updatePassword: '/profile/UpdatePassword',
        getAlertSearchs: '/profile/GetAlertSearchs',
        deleteAlertSearch: '/profile/DeleteAlertSearch',
        updateAlertSearch: '/profile/UpdateAlertSearch',
        getReceiveEmailType: '/profile/getReceiveEmailType',
        getRecentSearchs: '/profile/GetRecentSearchs',
        getAlertSearchLatest: '/profile/GetAlertSearchLatest',
        saveRecentSearch: '/profile/SaveRecentSearch',
        clearRecentSearch: '/profile/ClearRecentSearch',
        UnrollAlert: '/profile/ConfirmUnsubcribeSearchAlert',
        MogiProRegister: '/account/mogipro',
        ReadMessage: '/profile/ReadMessage',
        GetMessages: '/profile/MemberInboxMessage',
        validateMobile: '/account/validatemobile',
		checkReferal: '/account/checkReferal',
		sendSMS: '/account/sendsms'
    },
    Cms: {
        GetNews: '/Cms/GetNews',
        GetListNews: '/Cms/GetListNews',
        NewsDetail: '/Cms/Detail',
        GetCategoriesByGroup: '/Cms/GetCategoriesByGroup',
        GetByUrlCode: '/Cms/GetByUrlCode',
        GetNotarizeOffices: '/Cms/GetNotarizeOffices'

    },
    MBNAgent: {
        GetLocationAgent: '/Cms/GetLocationAgent?cityId=',
        GetMBNAgents: '/Cms/GetMBNAgents'
    }
};