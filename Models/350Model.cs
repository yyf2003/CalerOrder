using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Order350Model
    {
        public int ShopId { get; set; }
        public int SubjectId { get; set; }
        public string ShopNo { get; set; }
        public string ShopName { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string CityTier { get; set; }
        public string Channel { get; set; }
        public string Format { get; set; }
        public string NewFormat { get; set; }
        public string POPAddress { get; set; }
        public string Contacts { get; set; }
        public string Tels { get; set; }
        public string ChooseImg { get; set; }
        public string SubjectName { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string Sheet { get; set; }
        public string GraphicNo { get; set; }
        public double Quantity { get; set; }
        public string GraphicMaterial { get; set; }
        public double GraphicWidth { get; set; }
        public double GraphicLength { get; set; }
        public double Area { get; set; }
        public string OtherRemark { get; set; }
        public string PositionDescription { get; set; }
        public int? IsPOPMaterial { get; set; }
        public string CornerType { get; set; }
        public string ShopLevel { get; set; }
        public string POSScale { get; set; }
        public string MaterialSupport { get; set; }
        public string MachineFrame { get; set; }
        public string IsInstall { get; set; }
        public string SupplimentSubjectName { get; set; }
        public string OrderType { get; set; }
        public double UnitPrice { get; set; }
        public string QuoteGraphicMaterial { get; set; }
        public string GuidanceName { get; set; }
        public double ReceivePrice { get; set; }
        public double PayPrice { get; set; }
        public DateTime? AddDate { get; set; }
    }
}
