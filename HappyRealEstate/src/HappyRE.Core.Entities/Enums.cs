using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class Permission
    {
        public const string ACCOUNT = "ACCOUNT";
        public const string ADMIN = "ADMIN";
        public const string NOTIFICATION_CREATE = "NOTIFICATION_CREATE";
        public const string NOTIFICATION_DELETE = "NOTIFICATION_DELETE";
        public const string COMMENT_DELETE = "COMMENT_DELETE";
        public const string IP_ACCESS = "IP_ACCESS";
        public const string CUSTOMER_VIEW = "CUSTOMER_VIEW";
        public const string CUSTOMER_CREATE = "CUSTOMER_CREATE";
        public const string CUSTOMER_DELETE = "CUSTOMER_DELETE";
        public const string CUSTOMER_EXPORT = "CUSTOMER_EXPORT";
        public const string CUSTOMER_INFO_VIEW = "CUSTOMER_INFO_VIEW";
        public const string CUSTOMER_MODIFY = "CUSTOMER_MODIFY";
        public const string CUSTOMER_MODIFY_HISTORY = "CUSTOMER_MODIFY_HISTORY";
        public const string CUSTOMER_STATUS = "CUSTOMER_STATUS";
        public const string CUSTOMER_VIP = "CUSTOMER_VIP";
        public const string DEPARTMENT = "DEPARTMENT";
        public const string DEPARTMENT_EXPORT = "DEPARTMENT_EXPORT";
        public const string PROPERTY_USER_CREATED = "PROPERTY_USER_CREATED";
        public const string SYS_ADMIN = "SYS_ADMIN";
        public const string PROPERTY_VIEW = "PROPERTY_VIEW";
        public const string PROPERTY_CREATE = "PROPERTY_CREATE";
        public const string PROPERTY_CUSTOMER_INFO_HIDE = "PROPERTY_CUSTOMER_INFO_HIDE";
        public const string PROPERTY_CUSTOMER_INFO_VIEW = "PROPERTY_CUSTOMER_INFO_VIEW";
        public const string PROPERTY_CUSTOMER_MANAGE = "PROPERTY_CUSTOMER_MANAGE";
        public const string PROPERTY_DELETE = "PROPERTY_DELETE";
        public const string PROPERTY_EXPORT = "PROPERTY_EXPORT";
        public const string PROPERTY_HOT = "PROPERTY_HOT";
        public const string PROPERTY_VERIFY = "PROPERTY_VERIFY";
        public const string PROPERTY_LEGAL_STATUS = "PROPERTY_LEGAL_STATUS";
        public const string PROPERTY_MODIFY = "PROPERTY_MODIFY";
        public const string PROPERTY_MODIFY_HISTORY = "PROPERTY_MODIFY_HISTORY";
        public const string PROPERTY_STATUS = "PROPERTY_STATUS";
        public const string SALEORDER_VIEW = "SALEORDER_VIEW";
        public const string SALEORDER_CREATE = "SALEORDER_CREATE";
        public const string SALEORDER_MODIFY = "SALEORDER_MODIFY";
        public const string SALEORDER_DELETE = "SALEORDER_DELETE";
        public const string SALEORDER_EXPORT = "SALEORDER_EXPORT";
    }

    public enum UserStatus
    {
        NotEmail=0,
        Done=1
    }

    public enum FileType
    {
        Property = 1,
        Blog=2,
        Avatar=3,
        Customer = 4
    }

    public enum TemplateCateType
    {
        Section = 20,
        PopUp = 30
    }

    public enum IntegrationType
    {
        WebHook = 1,
        GetResponse = 4,
        MailChimp= 3,
        ActiveCampain = 5,
        Gmail = 8,
        GoogleSheet = 9,
        Wordpress = 10,
        Shopify =11,
        Haravan = 12,
        Sapo = 13,
        Ftp = 14,
        Sms=15,
        InfusionSoft=16,
        Autopilot = 17,
        OmiCall=18,
        SendGrid = 19,
        Twilio =20
    }

    public enum TemplateStatus
    {
        New =0,
        Submited = 1,
        Approved = 2,
        Cancelled = 3
    }

    public enum CommissionPercent
    {
        Level1 = 30,
        Level2 = 40,
        Level3 = 50
    }
    public enum AffilateAgentLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    public enum AffilateMemberStatus
    {
        NotApproved = 0,
        Approved = 1
    }

    public enum LeadStatus
    {
        New = 0,
        Hot = 1,
        Warm = 2,
        Cold = 3
    }

    public enum ProfileLevel
    {
        Trial = 0,
        Economy = 1,
        Business = 2,
        VIP = 3
    }

    public enum InvoiceStatus
    {
        WaitToPay = 0,
        Paid = 1,
        Cancel = 2,
        PayError = 3
    }

    public enum PaymentType
    {
        ATM = 1,
        CreditCard = 2,
        QRCode = 3
    }

    public enum NotificationType
    {
        Subcrible = 1,
        SystemAlert = 2,
        Other = 3
    }

    public enum LeadSendStatus
    {
        None = 0,
        Success = 1,
        Failed = 2
    }

    public enum EmailFunction
    {
        ResetPassword = 1,
        VerifyEmail = 2
    }

    public enum EmailSendType
    {
        System_NoReply = 1,
        Lead_AutoReply = 2,
        Marketing = 3,
        Alert=4
    }

    public enum TrafficSource
    {
        Direct = 0,
        Search = 2,
        Social = 3,
        Other = 4
    }

    public enum TaskQueueType
    {
        /// <summary>
        /// Thay đổi page content để xóa cache
        /// </summary>
        UpdatePageContent = 1
    }

    //public enum PublishVendor
    //{
    //    /// <summary>
    //    /// Xuất bản ra các ứng dụng khác
    //    /// </summary>
    //    WordPress = 10
    //}

    public enum PublishType
    {
        /// <summary>
        /// Loại xuất bản
        /// </summary>
        Dns = 1,
        WP=2,
        Shopify=3,
        Haravan=4,
        Sapo = 5,
        Ftp=6
    }

    public enum SMSType
    {
        /// <summary>
        /// Loại gửi SMS
        /// </summary>
        New = 0,
        CS1 = 1,
        CS2 = 2,
        CS3 = 3
    }

    public enum EmailStatus
    {
        NotVerify=0,
        Verified=1,
        Block=2
    }

    public enum MobileStatus
    {
        NotVerify = 0,
        Verified = 1,
        Block = 2
    }

    public enum ResponderType
    {
        Email = 1,
        Sms = 2
    }

    public enum AffiliatePaymentStatus
    {
        WaitingForApproved = 0,
        Approved = 1,
        Tranfered = 2,
        Refunded= 3
    }

    public enum TicketPriority
    {
        Normal = 0,
        Low = 1,
        High = 2
    }

    public enum TicketStatus
    {
        New = 0,
        Process = 1,
        Close = 2,
        ReOpen =3
    }

    public enum TicketChannel
    {
        Page = 0,
        Phone = 1,
        Email = 2,
        Facebook = 3,
        Zalo = 4
    }

}
