using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class CustomerInfoSummary
    {
        public string Phone { get; set; }
        [DisplayName("Giá trị giao dịch")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Số lượng giao dịch")]
        public int Total { get; set; }
        [DisplayName("Tổng điêm thưởng")]
        public decimal RewardPoint { get; set; }
        [DisplayName("Tổng điêm thưởng")]
        public decimal TotalAmountB => this.TotalAmount / 1000000000;
    }

    public class CustomerInfoTransaction
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string TranTo { get; set; }
        public string PropertyNumber { get; set; }
        public string PropertyAddress { get; set; }
        public string Type { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal RewardPoint => Math.Round(this.TotalAmount / 50000000);
        public string TotalAmountB => (Math.Round(this.TotalAmount / 1000000000,2)).ToString() + " tỷ";
    }

    public class CustomerInfoDetail
    {
        public CustomerInfoSummary Summary { get; set; }
        public IEnumerable<CustomerInfoTransaction> Transactions { get; set; }
        public Model.CustomerInfo Personal { get; set; }
    }
}
