var PostingUI = (function ($,_){
var self = this;
self.fields=[{"FieldId":1,"Name":"Address","Label":"Số nhà","PlaceHolder":"Số nhà","Tooltip":"Các bạn có thể dùng chức năng Ẩn Số Nhà nếu không muốn công khai địa chỉ nhà mình","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":2,"Name":"Area","Label":"Diện tích sử dụng","PlaceHolder":"Diện tích sử dụng","Tooltip":"Nhập diện tích sử dụng","Min":0,"Max":1000000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":3,"Name":"BathRooms","Label":"Số phòng WC","PlaceHolder":"Số phòng WC","Tooltip":"Nhập số phòng WC","Min":0,"Max":100,"MinLength":null,"MaxLength":null,"Pattern":"/^\\d+$/"},{"FieldId":4,"Name":"BedRooms","Label":"Số phòng ngủ","PlaceHolder":"Số phòng ngủ","Tooltip":"Nhập số phòng ngủ","Min":0,"Max":100,"MinLength":null,"MaxLength":null,"Pattern":"/^\\d+$/"},{"FieldId":5,"Name":"Body","Label":"Mô tả","PlaceHolder":"Mô tả","Tooltip":"Mô tả thêm về căn nhà","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":6,"Name":"ContactMobile","Label":"Số di động","PlaceHolder":"Số di động","Tooltip":"Vui lòng không sử dụng dấu cách ","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":7,"Name":"ContactName","Label":"Người liên hệ","PlaceHolder":"Người liên hệ","Tooltip":"Người liên hệ","Min":null,"Max":null,"MinLength":1,"MaxLength":150,"Pattern":null},{"FieldId":8,"Name":"ContactPhone","Label":"Số cố định","PlaceHolder":"Số cố định","Tooltip":"Vui lòng không sử dụng dấu cách ","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":";jlklj"},{"FieldId":9,"Name":"Contract","Label":"Hợp đồng","PlaceHolder":"Hợp đồng","Tooltip":"Nhập thời gian ký hợp đồng cho thuê","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":10,"Name":"Deposit","Label":"Đặt cọc","PlaceHolder":"Đặt cọc","Tooltip":"Nhập thời gian đặt cọc","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":11,"Name":"DirectionId","Label":"Hướng","PlaceHolder":"Hướng","Tooltip":"Giúp cho người mua tìm kiếm và quyết định hiệu quả hơn - Rất cần thiết cho BĐS","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":12,"Name":"DistrictId","Label":"Quận/Huyện","PlaceHolder":"Quận/Huyện","Tooltip":"","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":13,"Name":"Floor","Label":"Vị trí tầng/lầu","PlaceHolder":"Vị trí tầng/lầu","Tooltip":"Nhập vị trí tầng/lầu căn hộ","Min":0,"Max":100,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":14,"Name":"IndoorUtilities","Label":"Nội thất","PlaceHolder":"Nội thất","Tooltip":"Nhập nội thất","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":15,"Name":"LandArea","Label":"Diện tích đất","PlaceHolder":"Diện tích đất","Tooltip":"Nhập diện tích đất","Min":0,"Max":1000000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":16,"Name":"LegalId","Label":"Pháp lý","PlaceHolder":"Pháp lý","Tooltip":"Nhập pháp lý","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":17,"Name":"Length","Label":"Dài","PlaceHolder":"Dài","Tooltip":"Nhập chiều dài","Min":null,"Max":1000000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":18,"Name":"ManagementFee","Label":"Phí quản lý","PlaceHolder":"Phí quản lý","Tooltip":"Nhập phí quản lý","Min":0,"Max":10000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":19,"Name":"NumOfFloors","Label":"Số tầng/lầu","PlaceHolder":"Số tầng/lầu","Tooltip":"Nhập số tầng lầu","Min":0,"Max":100,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":20,"Name":"OutdoorUtilities","Label":"Tiện ích xung quanh","PlaceHolder":"Tiện ích xung quanh","Tooltip":"Chọn tiện ích xung quanh","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":21,"Name":"Price","Label":"Giá","PlaceHolder":"Giá","Tooltip":"Nhập giá bán","Min":100000,"Max":900000000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":22,"Name":"ProjectId","Label":"Dự án","PlaceHolder":"Dự án","Tooltip":"Chọn dự án","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":23,"Name":"RoadWidth","Label":"Đường trước nhà","PlaceHolder":"Đường trước nhà","Tooltip":"Nhập chiều rộng đường trước nhà","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":24,"Name":"StreetId","Label":"Đường","PlaceHolder":"Đường","Tooltip":"Chọn đường","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":25,"Name":"Title","Label":"Tiêu đề","PlaceHolder":"Tiêu đề","Tooltip":"Nếu không thể hoàn thành \"Các bước đăng tin\" trong vòng 20 phút, Quý Khách vui lòng điền trước  những mục bắt buộc (*) để lưu và tiếp tục. Tin đăng của quý khách sẽ được lưu lại trong \"Tin Đang  Soạn\" để Quý Khách có thể quay lại bổ sung trong tương lai. ","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":26,"Name":"WardId","Label":"Phường/Xã","PlaceHolder":"Phường/Xã","Tooltip":"Phường/Xã","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":27,"Name":"Width","Label":"Rộng","PlaceHolder":"Rộng","Tooltip":"Nhập chiều rộng","Min":0,"Max":1000000000,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":28,"Name":"CityId","Label":"Tỉnh thành","PlaceHolder":"Tỉnh thành","Tooltip":"Chọn tỉnh thành","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":29,"Name":"CurrencyId","Label":"Chọn đơn vị tính","PlaceHolder":"Chọn đơn vị tính","Tooltip":"Chọn đơn vị tính","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":30,"Name":"WidthBehind","Label":"Nở hậu","PlaceHolder":"Nở hậu","Tooltip":"Nở hậu","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":100,"Name":"ForRent","Label":"Giao dịch","PlaceHolder":"Giao dịch","Tooltip":null,"Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":101,"Name":"PropertyTypeId","Label":"Loại bất động sản","PlaceHolder":"Loại bất động sản","Tooltip":null,"Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":102,"Name":"SubPropertyTypeId","Label":"Kiểu bất động sản","PlaceHolder":"Kiểu bất động sản","Tooltip":null,"Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":110,"Name":"ImageObject","Label":"Ảnh","PlaceHolder":"Ảnh","Tooltip":"Ảnh","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":111,"Name":"FloorPlanObject","Label":"Mặt bằng","PlaceHolder":"Mặt bằng","Tooltip":"Mặt bằng","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":112,"Name":"VideoObject","Label":"Video","PlaceHolder":"Video","Tooltip":"Video","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":113,"Name":"HiddenAddress","Label":"Ẩn số nhà","PlaceHolder":"Ẩn số nhà","Tooltip":"Ẩn số nhà","Min":null,"Max":null,"MinLength":null,"MaxLength":null,"Pattern":null},{"FieldId":114,"Name":"Title","Label":"Tiêu đề","PlaceHolder":"Vui lòng nhập tiêu đề tin đăng của bạn. Tối thiểu là 30 ký tự và tối đa là 70 ký tự","Tooltip":"Tiêu đề","Min":null,"Max":null,"MinLength":30,"MaxLength":70,"Pattern":null}];
self.hash = {};
self.msg = {"Required":"Vui lòng nhập “{0}”.","Min":"“{0}” phải lớn hơn {1}.","Max":"“{0}” phải nhỏ hơn {1}.","MaxLength":"“{0}” có chiều dài tối đa là {1} ký tự.","MinLength":"“{0}” chiều dài tối thiểu là {1} ký tự.","Number":"“{0}” phải là kiểu số","/^\\d+$/":"",";jlklj":""};
self.ui = {
 "S_0_0":[{"fid":1,"val":0},{"fid":2,"lbl":"Diện tích sử dụng","val":39},{"fid":3,"val":8230},{"fid":4,"val":8230},{"fid":5,"val":1},{"fid":6,"lbl":"Số điện thoại liên hệ","val":1},{"fid":7,"lbl":"Tên liên hệ","val":17},{"fid":11,"val":1},{"fid":12,"val":1},{"fid":13,"val":38},{"fid":14,"val":0},{"fid":15,"val":39},{"fid":16,"val":1},{"fid":17,"lbl":"Dài","val":33},{"fid":18,"val":8192},{"fid":19,"val":38},{"fid":20,"val":0},{"fid":21,"val":39},{"fid":22,"val":0},{"fid":23,"val":8192},{"fid":24,"val":1},{"fid":26,"val":0},{"fid":27,"lbl":"Rộng","val":35},{"fid":28,"val":1},{"fid":29,"val":1},{"fid":101,"val":1},{"fid":102,"val":1},{"fid":110,"val":1},{"fid":111,"val":0},{"fid":112,"val":0},{"fid":113,"val":0},{"fid":114,"lbl":"Tiêu đề","val":25}],
 "R_0_0":[{"fid":1,"val":0},{"fid":2,"val":39},{"fid":3,"val":8230},{"fid":4,"val":8230},{"fid":5,"val":1},{"fid":6,"lbl":"Số điện thoại liên hệ","val":1},{"fid":7,"lbl":"Tên liên hệ","val":1},{"fid":8,"lbl":"Số điện thoại liên hệ","val":65},{"fid":11,"val":0},{"fid":12,"val":1},{"fid":13,"val":38},{"fid":14,"val":0},{"fid":15,"val":8230},{"fid":16,"val":8192},{"fid":17,"val":8192},{"fid":18,"val":8192},{"fid":19,"val":38},{"fid":20,"val":0},{"fid":21,"val":39},{"fid":22,"val":0},{"fid":23,"val":8192},{"fid":24,"val":1},{"fid":26,"val":0},{"fid":27,"val":8192},{"fid":28,"val":1},{"fid":29,"val":1},{"fid":101,"val":1},{"fid":102,"val":1},{"fid":110,"val":1},{"fid":111,"val":0},{"fid":112,"val":0},{"fid":113,"val":0},{"fid":114,"val":25}],
 "S_1_0":[{"fid":2,"val":38},{"fid":3,"val":39},{"fid":4,"val":39},{"fid":13,"val":8230},{"fid":19,"val":39}],
 "R_1_0":[{"fid":2,"val":38},{"fid":3,"val":39},{"fid":4,"val":39},{"fid":13,"val":8230},{"fid":15,"val":39},{"fid":19,"val":39}],
 "S_2_0":[{"fid":1,"val":8192},{"fid":2,"val":38},{"fid":13,"val":8230},{"fid":14,"val":8192},{"fid":19,"val":8230},{"fid":20,"val":8192}],
 "R_2_0":[{"fid":1,"val":8192},{"fid":2,"val":38},{"fid":13,"val":8230},{"fid":14,"val":8192},{"fid":15,"val":39},{"fid":19,"val":8230},{"fid":20,"val":8192}],
 "S_3_0":[{"fid":3,"val":39},{"fid":4,"val":39},{"fid":13,"val":39},{"fid":15,"val":8230},{"fid":27,"tip":"0","val":8256}],
 "R_3_0":[{"fid":3,"val":39},{"fid":4,"val":39},{"fid":13,"val":39},{"fid":15,"val":8230}],
 "R_5_0":[{"fid":14,"val":8192},{"fid":15,"val":8230}],
 "S_6_0":[{"fid":14,"val":8192},{"fid":15,"val":8230},{"fid":16,"val":0}],
 "R_6_0":[{"fid":14,"val":8192},{"fid":15,"val":8230}],
 "R_7_0":[{"fid":11,"val":8192},{"fid":13,"val":8230},{"fid":15,"val":8230},{"fid":20,"val":8192}],
 "R_8_0":[{"fid":19,"val":8230}],
 "R_1080_0":[{"fid":1,"val":8192},{"fid":2,"val":38},{"fid":11,"val":8192},{"fid":13,"val":8230},{"fid":14,"val":8192},{"fid":15,"val":39},{"fid":19,"val":8230},{"fid":20,"val":8192}],};
self.mapObject = function(a,b,m){
    _.each(a, function(value, key) {
        if (m) key = m[key] || key;
        if (key == 'Value' && a['Hidden'] == true) {
            b[key] = null;
        }
        else if (key == 'Value') {
        } else {
            b[key] = value;
        }
    });
    return b;
};
self.getField = function(fid){
    var v = self.hash[fid];
    if (v == null) {
        v = _.find(self.fields, function (o) { return o.FieldId == fid; });
        self.hash[fid] = v;
    }
    return v;
};
self.getFirstStep = function(obj){
    var r = self.getUI(false,0,0);
    r = _.filter(r,function(o){ return o.fid >= 100; });
    if (obj) {
        for (prop in obj) {
            var o = obj[prop];
            var d = _.find(r,function(i){return i.Name==prop;});
            if (d) self.mapObject(d, o);
        }
    }
};
self.getUI = function(rent,pid,sid,obj,map){
    pid = pid || 0;
    sid = sid || 0;
    var t = (rent ? 'R' : 'S');
    var k = t + '_' + pid + '_' + sid;
    var res = self.hash[k], h = self.hash['hash_' + k] || {};
    if (res == null) {
        res = [];
        var conf = self.ui[t + '_' + pid + '_' + sid];
        if (conf == null) conf = self.ui[t + '_' + pid + '_0'];
        if (conf == null) conf = self.ui[t + '_0_0'];
        if (conf == null) return null;
        _.each(conf, function (o) {
            var f = getField(o.fid);
            h[f.Name] = new PostingField(self, f, o);
            res.push(h[f.Name]);
        });
        if(sid==0)
            conf = self.ui[t + '_0_0'];
        else
            conf = self.ui[t + '_'+ pid +'_0'];
        _.each(conf, function (o) {
            var f = getField(o.fid);
            if (h[f.Name]) return;
            h[f.Name] = new PostingField(self, f, o);
            res.push(h[f.Name]);
        });
        self.hash[k] = res;
        self.hash['hash_' + k] = h;
    }
    if (obj) {
        for (prop in obj) {
            var o = obj[prop];
            var d = h[prop];
            if (d) self.mapObject(d, o, map);
        }
    }
    return res;
};
return {
    getUI: self.getUI,
    getFirstStep: self.getFirstStep
};
})(jQuery,_);

function PostingField(ui, f, d) {
    var msg = ui.msg, val = d.val || 0;;
    this.fid = f.FieldId;
    this.Name = f.Name;
    this.Label = (d['lbl'] || f.Label);
    this.PlaceHolder = (d['plh'] || f.PlaceHolder);
    this.Tip = (d['tip'] || f.Tooltip);
    this.Required = ((val & 1) > 0);
    this.Min = ((val & 2) > 0 ? f.Min : null);
    this.Max = ((val & 4) > 0 ? f.Max : null);
    this.MinLength = ((val & 8) > 0 ? f.MinLength : null);
    this.MaxLength = ((val & 16) > 0 ? f.MaxLength : null);
    this.Number = ((val & 32) > 0);
    this.Pattern = ((val & 64) > 0);
    this.Hidden = ((val & 8192) > 0);
    this.Value=null;
    this.MsgRequired = (this.Required ? msg.Required.format(this.Label) : '');
    //this.MsgMin = (this.Min ? msg.Min.format(this.Label, f.Min) : '');
    this.MsgMin =msg.Min.format(this.Label, f.Min) ;
    //this.MsgMax = (this.Max ? msg.Max.format(this.Label, f.Max) : '');
    this.MsgMax = msg.Max.format(this.Label, f.Max);
    this.MsgMinLength = (this.MinLength ? msg.MinLength.format(this.Label, f.MinLength) : '');
    this.MsgMaxLength = (this.MaxLength ? msg.MaxLength.format(this.Label, f.MaxLength) : '');
    this.MsgNumber = (this.Number ? msg.Number.format(this.Label) : '');
    //this.MsgPattern = (this.Pattern ? msg[f.Pattern].format(this.Label) : '');
}