using Transport_Management.Models.Entity;

namespace Transport_Management.Models
{
    public class clsViewModel
    {
        
    }
    public class clsRentalViewModel
    {
        public List<mstVehicle> lstDrpVehicle;
        public List<mstCompany> lstDrpCompany;
        public List<mstRentalEntry> lstrentalEntry;
        public mstRentalEntry mstRentalEntry;


    }
    public class clsInvoiceViewModel
    {
        public List<mstVehicle> lstDrpVehicle;
        public List<mstCompany> lstDrpCompany;
        public List<mstRentalEntry> rentalList;
    }

    public class clstVehicleViewModel
    {
        public List<mstVehicle> inActiveVehicle;
        public List<mstVehicle> activeVehicle;

    }
    public class clsCompanyViewModel
    {
        public List<mstCompany> inActiveCompanyList; 
        public List<mstCompany> activetCompanyList;
    }
}
