using System.Collections.Generic;

namespace ChildFashion.Infracstructures
{
    public class MobileNumberHelper
    {
        public static MobileNetwork GetMobileName(string mobilePhone)
        {
            var network = new Dictionary<string, MobileNetwork>
            {
            {"090", MobileNetwork.Mobifone},
            {"093", MobileNetwork.Mobifone},
            {"089", MobileNetwork.Mobifone},
            {"076", MobileNetwork.Mobifone},
            {"077", MobileNetwork.Mobifone},
            {"078", MobileNetwork.Mobifone},
            {"079", MobileNetwork.Mobifone},
            {"070", MobileNetwork.Mobifone},
            {"091", MobileNetwork.Vinaphone},
            {"094", MobileNetwork.Vinaphone},
            {"088", MobileNetwork.Vinaphone},
            {"085", MobileNetwork.Vinaphone},
            {"084", MobileNetwork.Vinaphone},
            {"083", MobileNetwork.Vinaphone},
            {"082", MobileNetwork.Vinaphone},
            {"081", MobileNetwork.Vinaphone},
            {"096", MobileNetwork.Viettel},
            {"097", MobileNetwork.Viettel},
            {"098", MobileNetwork.Viettel},
            {"086", MobileNetwork.Viettel},
            {"032", MobileNetwork.Viettel},
            {"033", MobileNetwork.Viettel},
            {"034", MobileNetwork.Viettel},
            {"035", MobileNetwork.Viettel},
            {"036", MobileNetwork.Viettel},
            {"037", MobileNetwork.Viettel},
            {"038", MobileNetwork.Viettel},
            {"039", MobileNetwork.Viettel},
            {"095", MobileNetwork.Sfone},
            {"092", MobileNetwork.Vietnamobile},
            {"056", MobileNetwork.Vietnamobile},
            {"058", MobileNetwork.Vietnamobile},
            {"099", MobileNetwork.Gmobile},
            {"059", MobileNetwork.Gmobile}
            };

            var result = MobileNetwork.Telecom;
            foreach (var mobileNetwork in network)
            {
                if (mobilePhone.StartsWith(mobileNetwork.Key))
                {
                    result = mobileNetwork.Value;
                    break;
                }
            }

            return result;
        }
    }

    public enum MobileNetwork
    {
        Mobifone = 1,
        Vinaphone = 2,
        Viettel = 3,
        Telecom = 4,
        Sfone = 5,
        Vietnamobile = 6,
        Gmobile = 7
    }
}