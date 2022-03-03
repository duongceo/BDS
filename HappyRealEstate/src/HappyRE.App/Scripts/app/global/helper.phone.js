var phoneHelper = (function() {
    function renderPhone(n) {
        var network = [];
        network.push({ num: '090', mobileNetwork: 'Mobi' });
        network.push({ num: '093', mobileNetwork: 'Mobi' });
        network.push({ num: '0122', mobileNetwork: 'Mobi' });
        network.push({ num: '0126', mobileNetwork: 'Mobi' });
        network.push({ num: '0121', mobileNetwork: 'Mobi' });
        network.push({ num: '0128', mobileNetwork: 'Mobi' });
        network.push({ num: '0120', mobileNetwork: 'Mobi' });
        network.push({ num: '091', mobileNetwork: 'Vina' });
        network.push({ num: '094', mobileNetwork: 'Vina' });
        network.push({ num: '0123', mobileNetwork: 'Vina' });
        network.push({ num: '0125', mobileNetwork: 'Vina' });
        network.push({ num: '0127', mobileNetwork: 'Vina' });
        network.push({ num: '0129', mobileNetwork: 'Vina' });
        network.push({ num: '097', mobileNetwork: 'Viettel' });
        network.push({ num: '098', mobileNetwork: 'Viettel' });
        network.push({ num: '0168', mobileNetwork: 'Viettel' });
        network.push({ num: '0169', mobileNetwork: 'Viettel' });
        network.push({ num: '0166', mobileNetwork: 'Viettel' });
        network.push({ num: '0167', mobileNetwork: 'Viettel' });
        network.push({ num: '0165', mobileNetwork: 'Viettel' });
        network.push({ num: '0164', mobileNetwork: 'Viettel' });
        network.push({ num: '0163', mobileNetwork: 'Viettel' });
        network.push({ num: '0162', mobileNetwork: 'Viettel' });
        network.push({ num: '096', mobileNetwork: 'Telecom' });
        network.push({ num: '099', mobileNetwork: 'Telecom' });
        network.push({ num: '095', mobileNetwork: 'Sfone' });
        network.push({ num: '092', mobileNetwork: 'Vietnamobile' });
        network.push({ num: '0188', mobileNetwork: 'Vietnamobile' });
        network.push({ num: '0199', mobileNetwork: 'Beeline' });

        var result = "";
        for (var i = 0; i < network.length; i++) {
            if (n.lastIndexOf(network[i].num, 0) === 0) {
                result = network[i].mobileNetwork;
                break;
            }
        }

        return "<span class='label label-success'>" + result + "</span>";
    }

    return {
        renderPhone: renderPhone
    };
}())