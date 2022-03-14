using HappyRE.Core.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Model
{
    public class HistoryLog: BaseEntity
    {
        public int Id { get; set; }

        public string TableName { get; set; }

        public int? TableKeyId { get; set; }

        public string Action { get; set; }

        public string Contents { get; set; }
        public string IpAddress { get; set; }

        public bool Deleted { get; set; }

        public int Type { get; set; }

        [NotMapped]
        public string DateChangedFriendly
        {
            get { return CreatedDate.ToFriendlyDate(); }
        }
        [NotMapped]
        public string DateChangedDisplay
        {
            get
            {
                return CreatedDate.ToString("dd-MM-yyyy HH:mm:ss");
            }
        }

        [NotMapped]
        public string ContentDisplay
        {
            get
            {
                //var n = this.TableName == "Property" ? "bất động sản" : this.TableName == "Customer" ? "khách hàng" : this.TableName == "CustomerInfo" ? "chăm sóc khách hàng" : this.TableName == "SaleOrder" ? "giao dịch BĐS" : "";
                //var a = this.Action == "Search" ? "Xem danh sách" : this.Action == "Detail" ? "Xem chi tiết thông tin" : this.Action == "IU" ? "Sửa thông tin": this.Action == "Delete" ? "Xóa": this.Action == "Export" ? "Xuất dữ liệu":"";

                var tableName = Conts.GetTableName(this.TableName);
                var action = Conts.GetLogAction(this.Action);
                return $"{action} {tableName} {this.Quantity} lần";
            }
        }

        [NotMapped]
        public string ContentDisplayDetail
        {
            get
            {
                //var n = this.TableName == "Property" ? "bất động sản" : this.TableName == "Customer" ? "khách hàng" : this.TableName == "CustomerInfo" ? "chăm sóc khách hàng" : this.TableName == "SaleOrder" ? "giao dịch BĐS" : "";
                //var a = this.Action == "Search" ? "Xem danh sách" : this.Action == "Detail" ? "Xem chi tiết thông tin" : this.Action == "IU" ? "Sửa thông tin" : this.Action == "Delete" ? "Xóa" : this.Action == "Export" ? "Xuất dữ liệu" : "";
                var tableName = Conts.GetTableName(this.TableName);
                var action = Conts.GetLogAction(this.Action);
                return $"{action} {tableName}";
            }
        }

        [NotMapped]
        public int Quantity { get; set; }
        [NotMapped]
        public string FullName { get; set; }

        [NotMapped]
        public string CustomerName { get; set; }
        [NotMapped]
        public string PropertyName { get; set; }
        [NotMapped]
        public string CustomerInfoName { get; set; }
        [NotMapped]
        public string SaleOrderNumber { get; set; }

        [NotMapped]
        public string NotificationName { get; set; }

        [NotMapped]
        public string ReferName => this.TableName == "Property" ? this.PropertyName : this.TableName == "Customer" ? this.CustomerName: this.TableName == "SaleOrder" ? this.SaleOrderNumber: this.TableName == "CustomerInfo" ? this.CustomerInfoName: this.TableName == "Notification" ? this.NotificationName : "";
    }
}
