
var bizHelper = (function() {
    var renderPriority = function (p) {
        switch (p) {
            case '1':
                return "<h5><span class='label label-danger'>Cao</span></h5>";
                break;
            case '2':
                return "<h5><span class='label label-primary'>Vừa</span></h5>";
                break;
            case '3':
                return "<h5><span class='label label-default'>Thấp</span></h5>";
                break;
            default:
                return "";
        }
    },

        renderStatus = function (p) {
            switch (p) {
                case '1':
                    return "<h5><span class='label label-info'>Mới</span></h5>";
                    break;
                case '2':
                    return "<h5><span class='label label-danger'>Đang xử lý</span></h5>";
                    break;
                case '3':
                    return "<h5><span class='label label-warning'>Chờ đổi/trả hàng</span></h5>";
                    break;
                case '4':
                    return "<h5><span class='label label-warning label-pink'>Chờ hàng về</span></h5>";
                    break;
                case '5':
                    return "<h5><span class='label label-danger'>Chờ đặt thêm</span></h5>";
                    break;
                case '6':
                    return "<h5><span class='label label-success'>Đặt hàng</span></h5>";
                    break;
                case '98':
                    return "<h5><span class='label label-default'>Không mua hàng</span></h5>";
                    break;
                case '99':
                    return "<h5><span class='label label-success'>Hoàn tất</span></h5>";
                    break;
                default:
                    return "";
            }
        },

        renderOrderStatus = function (p) {
            switch (p) {
                case 1:
                    return "<h5><span class='label label-primary'>Chưa xử lý</span></h5>";
                case 2:
                    return "<h5><span class='label label-primary'>Chờ lấy hàng</span></h5>";
                case 3:
                    return "<h5><span class='label label-info'>Đã lấy hàng</span></h5>";
                case 4:
                    return "<h5><span class='label label-success'>Đã giao hàng</span></h5>";
                case 5:
                    return "<h5><span class='label label-success'>Hoàn tất</span></h5>";
                case 6:
                    return "<h5><span class='label label-info'>Đang giao hàng</span></h5>";
                case 7:
                    return "<h5><span class='label label-danger'>Giao không thành công</span></h5>";
                case 80:
                    return "<h5><span class='label label-default'>Đã trả hàng</span></h5>";
                case 81:
                    return "<h5><span class='label label-danger'>Đang trả hàng</span></h5>";
                case 90:
                    return "<h5><span class='label label-default'>Hủy đơn hàng</span></h5>";
                case 91:
                    return "<h5><span class='label label-default'>Hủy/Hoàn hàng</span></h5>";
                default:
                    return "";
            }
        },
        renderCarrier = function (p) {
            switch (p) {
                case "GHTK":
                    return "<h5><span class='label label-success'>GHTK</span></h5>";
                case "SUPERSHIP":
                    return "<h5><span class='label label-danger'>SuperShip</span></h5>";
                default:
                    return p;
            }
        },

        renderChannel = function (p) {
            var s = p.toString();
            switch (s) {
                case '1':
                    return "Facebook";
                    break;
                case '2':
                    return "sendo.vn";
                    break;
                case '3':
                    return "Google";
                    break;
                case '4':
                    return "Email";
                    break;
                case '5':
                    return "Zalo/Viber";
                    break;
                case '6':
                    return "batdongsanhanhphuc.vn";
                    break;
                case '7':
                    return "Gọi điện";
                    break;
                case '8':
                    return "CTV";
                    break;
                default:
                    return "";
            }
        },

        renderPhone = function (n) {
            var network = [];
            network.push({ num: '090', mobileNetwork: 'Mobi' });
            network.push({ num: '093', mobileNetwork: 'Mobi' });
            network.push({ num: '089', mobileNetwork: 'Mobi' });
            network.push({ num: '076', mobileNetwork: 'Mobi' });
            network.push({ num: '077', mobileNetwork: 'Mobi' });
            network.push({ num: '078', mobileNetwork: 'Mobi' });
            network.push({ num: '079', mobileNetwork: 'Mobi' });
            network.push({ num: '070', mobileNetwork: 'Mobi' });
            network.push({ num: '091', mobileNetwork: 'Vina' });
            network.push({ num: '094', mobileNetwork: 'Vina' });
            network.push({ num: '088', mobileNetwork: 'Vina' });
            network.push({ num: '085', mobileNetwork: 'Vina' });
            network.push({ num: '084', mobileNetwork: 'Vina' });
            network.push({ num: '083', mobileNetwork: 'Vina' });
            network.push({ num: '082', mobileNetwork: 'Vina' });
            network.push({ num: '081', mobileNetwork: 'Vina' });
            network.push({ num: '096', mobileNetwork: 'Viettel' });
            network.push({ num: '097', mobileNetwork: 'Viettel' });
            network.push({ num: '098', mobileNetwork: 'Viettel' });
            network.push({ num: '086', mobileNetwork: 'Viettel' });
            network.push({ num: '032', mobileNetwork: 'Viettel' });
            network.push({ num: '033', mobileNetwork: 'Viettel' });
            network.push({ num: '034', mobileNetwork: 'Viettel' });
            network.push({ num: '035', mobileNetwork: 'Viettel' });
            network.push({ num: '036', mobileNetwork: 'Viettel' });
            network.push({ num: '037', mobileNetwork: 'Viettel' });
            network.push({ num: '038', mobileNetwork: 'Viettel' });
            network.push({ num: '039', mobileNetwork: 'Viettel' });
            network.push({ num: '095', mobileNetwork: 'Sfone' });
            network.push({ num: '092', mobileNetwork: 'Vietnamobile' });
            network.push({ num: '056', mobileNetwork: 'Vietnamobile' });
            network.push({ num: '058', mobileNetwork: 'Vietnamobile' });
            network.push({ num: '099', mobileNetwork: 'GMobile' });
            network.push({ num: '059', mobileNetwork: 'GMobile' });

            var result = "";
            for (i = 0; i < network.length; i++) {
                if (n.lastIndexOf(network[i].num, 0) === 0) {
                    result = network[i].mobileNetwork;
                    break;
                }
            }

            return "<span class='label label-success'>" + result + "</span>";
        },

        renderDate = function (d, fd) {
            if (fd == null) return "";
            var oneDay = 1000 * 60 * 60 * 24;
            var date1 = d.getTime();

            var today = new Date();
            var date2 = today.getTime();

            var diff = date2 - date1;
            var days = Math.floor(diff / oneDay);

            if (days < 1) return fd;
            else if (days >= 1 && days < 3) return "<span style='color:blue'>" + fd + "</span>";
            else if (days >= 3) return "<span style='color:red'>" + fd + "</span>";
            else return fd;
        },
        renderAvatar = function (img) {
            var img_data = '/content/images/src/blank_avatar.jpg';
            if (img == null || img.length == 0) {
                img = img_data;
            }
            else img_data = img.replace('s200x200', '');
            return "<img data-magnify='gallery' data-src='" + img_data + "' src='" + img + "' style='width:50px;height:50px;' />"
        },
        renderCustomerShowPhone = function (isViewedMobileToday, customerId, FullName, Phone) {
            //var canViewMobile = parseInt($('#canViewMobile').val() || '1');
            //var canHideMobile = parseInt($('#canHideMobile').val() || '1');
            //if (canViewMobile == 1 || isViewedMobileToday == true) {
            //    if (isViewedMobileToday == false && canHideMobile == 0) return "<a class='btn btn-default btn-show-mobile-" + customerId + "' onclick='customerListing.showMobile(" + customerId + ")'>XEM SĐT</a><div style='display:none' class='info-" + customerId + "'><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
            //    else return "<div><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
            //} else return "";
            if (isViewedMobileToday == false) return "<a class='btn btn-default btn-show-mobile-" + customerId + "' onclick='customerListing.showMobile(" + customerId + ")'>XEM SĐT</a><div style='display:none' class='info-" + customerId + "'><span>" + FullName + "</span></br><span class='txt_phone'></span></div>";
            else return "<div><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
        },
        renderPropertyShowPhone = function (isViewedMobileToday, id, FullName, Phone) {
            //var canViewMobile = parseInt($('#canViewMobile').val() || '1');
            //var canHideMobile = parseInt($('#canHideMobile').val() || '1');
            //if (canViewMobile==1 || isViewedMobileToday == true) {
            //if (isViewedMobileToday == false && canHideMobile==0) return "<a class='btn btn-default btn-show-mobile-" + id + "' onclick='propertyListing.showMobile(" + id + ")'>XEM SĐT</a><div style='display:none' class='info-" + id + "'><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
            //    else return "<div><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
            //} else return "";

            if (isViewedMobileToday == false) return "<a class='btn btn-default btn-show-mobile-" + id + "' onclick='propertyListing.showMobile(" + id + ")'>XEM SĐT</a><div style='display:none' class='info-" + id + "'><span>" + FullName + "</span></br><span class='txt_phone'></span></div>";
            else return "<div><span>" + FullName + "</span></br><span>" + Phone + "</span></div>";
        },
        renderUserLogDetailUrl = function (id, type, text, textlink) {
            if (textlink != null && textlink.length > 0) textlink = textlink.replace(/(<br>|<\/br>|<br \/>)/mgi, ", ");
            if ((id || null) == null || id == '0') return text;
            else return "<span>" + text + "</span> <a target='_blank' href='/" + type + "/detail/" + id + "'>" + textlink + "</a>";
        },
        renderShowButton = function (canShow) {
            if (canShow) return 'btn btn-default';
            else return 'hidden';
        },
        renderCanDo = function (btnName) {
            var r = parseInt($('#' + btnName).val());
            return r == 0 ? 'hidden' : '';
        },
        renderCanEdit = function (btnName,createdBy) {
            var r = parseInt($('#' + btnName).val());
            var username = $('#userName').val();          
            return r == 0 ? (username != createdBy)? 'hidden':'' : '';
        };
    renderPropertyCode = function (address, street) {
        address = (address || '').replace(' ','_').trim();
        street = (street || '').trim();
        if (street.match(/^\d/)) {
            return (address +'Đ' + street).toUpperCase();
        } 
        if (street.indexOf(' ') < 0) return (address + street).toUpperCase();
        var letters = street.split(" ");
        var s = '';
        $.each(letters, function (key, value) {
            s += value.charAt(0);
        });
        return (address + s).toUpperCase();
        };
    return {
        renderPriority: renderPriority,
        renderDate: renderDate,
        renderStatus: renderStatus,
        renderPhone: renderPhone,
        renderChannel: renderChannel,
        renderOrderStatus: renderOrderStatus,
        renderCarrier: renderCarrier,
        renderCustomerShowPhone: renderCustomerShowPhone,
        renderPropertyShowPhone: renderPropertyShowPhone,
        renderUserLogDetailUrl: renderUserLogDetailUrl,
        renderShowButton: renderShowButton,
        renderCanDo: renderCanDo,
        renderCanEdit: renderCanEdit,
        renderPropertyCode: renderPropertyCode,
        renderAvatar: renderAvatar
    };
}())