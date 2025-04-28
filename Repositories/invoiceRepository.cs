using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Models.Entity;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;
using System.Xml.Linq;
using ClosedXML.Excel;
using System.Xml;
using DocumentFormat.OpenXml.Math;
using Transport_Management.Models;
using System.Diagnostics;
using DocumentFormat.OpenXml.Spreadsheet;
using  Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Extensions.Configuration;
using System.Data;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using GemBox.Spreadsheet;
using ClosedXML;

/*using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;*/

namespace Transport_Management.Repositories
{

    public class invoiceRepository : IinvoiceRepository
    {

        private readonly dbContext _dbContext;
        private IApiResponseRepository _apiResponseRepository;
        public readonly IConfiguration _configuration;
        public string LibreOfficePath { get; private set; }

        public invoiceRepository(dbContext dbContext, IApiResponseRepository apiResponseRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _apiResponseRepository = apiResponseRepository;
            LibreOfficePath = _configuration.GetSection("LibreOfficePath").Value;
        }
        public ApiResponseDTO checkInvoiceExistById(int mstInvoiceId)
        {
            var InvoiceDetails = _dbContext.mstInvoice.Where(a => a.mstInvoiceId == mstInvoiceId).FirstOrDefault();
            if (InvoiceDetails != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Invoice Details Exist", data = InvoiceDetails });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice not Found" });
        }
        public ApiResponseDTO getInvoiceList()
        {
            List<mstInvoice> InvoiceDetailsList = _dbContext.mstInvoice.Where(a => a.isActive == true).ToList();
            if (InvoiceDetailsList != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Data Fetched Successfully", data = InvoiceDetailsList });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice details not Found" });
        }
		public ApiResponseDTO getNotBilledInvoiceList()
		{
			List<mstRentalEntry> InvoiceDetailsList = _dbContext.mstRentalEntry.Where(a => a.isActive == true && a.isGST== true && a.isBilled == false).ToList();
			if (InvoiceDetailsList != null)
			{
				return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Data Fetched Successfully", data = InvoiceDetailsList });
			}
			return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice details not Found" });
		}
		public ApiResponseDTO addInvoicedetails(mstInvoice Invoice)
        {
            ApiResponseDTO resultInvoiceDetails = checkInvoiceExistById(Invoice.mstInvoiceId);
            mstInvoice InvoiceData = resultInvoiceDetails.data;
            if (resultInvoiceDetails.success != true)
            {
                _dbContext.mstInvoice.Add(Invoice);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Invoice Details Successfully Regisered" });
            }
            else if (resultInvoiceDetails.success == true && InvoiceData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice Details Already Exist. State is isActive False" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice Details already Exist" });
        }
        public ApiResponseDTO editInvoicedetails(mstInvoice Invoice)
        {
            ApiResponseDTO resultInvoiceDetails = checkInvoiceExistById(Invoice.mstInvoiceId);
            mstInvoice InvoiceData = resultInvoiceDetails.data;
            if (resultInvoiceDetails.success == true && InvoiceData.isActive == true)
            {
                InvoiceData = Invoice;
                _dbContext.mstInvoice.Update(InvoiceData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Invoice Details Updated Successfully " });
            }
            else if (resultInvoiceDetails.success == true && InvoiceData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice Details are Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }
        public ApiResponseDTO deleteInvoicedetails(mstInvoice Invoice)
        {
            ApiResponseDTO resultInvoiceDetails = checkInvoiceExistById(Invoice.mstInvoiceId);
            mstInvoice InvoiceData = resultInvoiceDetails.data;
            if (resultInvoiceDetails.success == true && InvoiceData.isActive == true)
            {
                InvoiceData.isActive = false;
                _dbContext.mstInvoice.Update(InvoiceData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Invoice Details Deleted Successfully " });
            }
            else if (resultInvoiceDetails.success == true && InvoiceData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invoice Details Already Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }

        public ApiResponseDTO generateInvoice(string rentalIds)
        {

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            List<int> rentalIdList = rentalIds.Split(',').Select(int.Parse).ToList();
            var rentalDetails = _dbContext.mstRentalEntry.Where(a => rentalIdList.Contains(a.mstRentalId)).ToList();
            int? companyIdList = rentalDetails.Select(a=> a.mstCompanyId).FirstOrDefault();
            var companyDetails = _dbContext.mstCompany.Where(a =>a.mstCompanyId == companyIdList).FirstOrDefault();
            var vehicledetails = _dbContext.mstVehicle.Where(a =>a.isActive == true).ToList();
            var InvoiceDetaills = ( from rnt in rentalDetails
                                    join vch in _dbContext.mstVehicle on rnt.mstVehicleId equals vch.mstVehicleId
                                    join cmp in _dbContext.mstCompany on rnt.mstCompanyId equals cmp.mstCompanyId
                                    where cmp.isActive == true  && rnt.isActive == true && rnt.isBilled == false && rnt.isGST == true
                                    select new
                                    {
                                        mstCompanyId= rnt.mstCompanyId,
                                        companyName= cmp.companyName,
                                        companyGst = cmp.companyGSTNo,
                                        companyAddress=cmp.addressLine1+" "+cmp.addressLine2 + " " + cmp.city + " " + cmp.pinCode + " " + cmp.state,
                                        mstRentalId=rnt.mstRentalId,
                                        rentalDate = rnt.rentalDate,
                                        vehicleId = vch.vehicleRegId,
                                        vehicleRegNo= vch.vehicleRegNo,
                                        rentalDescription = rnt.rentalDescription,
                                        rentalAmount=rnt.rentalAmount,
                                    }
                                    ).ToList();

            int contentfontSize = 14;
            int headerFontSize = 16;
            using (var workbook = new XLWorkbook())
            {
                var worksheet1 = workbook.Worksheets.Add("Bill");
                var topImgPath = "";
                int currentRow = 0;
                var newAprxRow = currentRow+1;



                //topImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "", "sample1.jpg");
                //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/sample1.jpg");

                string relativePath = Path.Combine("wwwroot", "assets", "images", "sample1.jpg");
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                fullPath= clsCommon.toConvertStringInTestServer(fullPath);
                //"C:\Users\paranthamans\Desktop\Project\Project\Transport_Management\Transport_Management\Transport_Management\wwwroot\assets\img\sample1.jpg"
               // string pichpath = "C:/Users/paranthamans/Desktop/Project/Project/Transport_Management/Transport_Management/wwwroot/assets/img/sample1.jpg";
                string pichpath = "C:\\Users\\paranthamans\\Desktop\\Project\\Project\\Transport_Management\\Transport_Management\\Transport_Management\\wwwroot\\assets\\img\\black.png";
                //string pichpath = "C:\\Users\\paranthamans\\Desktop\\Project\\Project\\Transport_Management\\Transport_Management\\Transport_Management\\wwwroot\\assets\\img\\sample1.jpg";
           


                worksheet1.AddPicture(pichpath).MoveTo(worksheet1.Cell("B" + (newAprxRow + 2))).WithSize(579, 98);

                currentRow = 7;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Merge();
                worksheet1.Cell(currentRow,2).Value = "Invoice";
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = headerFontSize;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet1.Range("B"+currentRow+":F"+currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;

                //Company Name Section
                currentRow = 9;
                worksheet1.Cell(currentRow, 2).Value = companyDetails.companyName;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = headerFontSize; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);//Alignment
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);//Alignment
                currentRow++;

                //Company Address Line 1 Section
                worksheet1.Cell(currentRow, 2).Value = companyDetails.addressLine1;
               // worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);//Alignment
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);//Alignment

                currentRow++;
                //Company Address Line 2 Section
                worksheet1.Cell(currentRow, 2).Value = companyDetails.addressLine2;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
               // worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);//Alignment

                //Invoice No Section
                worksheet1.Range("E" + currentRow + ":F" + currentRow).Merge();
                worksheet1.Cell(currentRow, 5).Value = "Invoice No: ";
                worksheet1.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
               // worksheet1.Range("F" + currentRow + ":G" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                currentRow++;
                // GST NO Section
                worksheet1.Cell(currentRow, 2).Value = "GSTIN No: "+companyDetails.companyGSTNo;
               // worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize; //FontSize
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);//Alignment

                worksheet1.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                //Date Section
                worksheet1.Range("E" + currentRow + ":F" + currentRow).Merge();
                worksheet1.Cell(currentRow, 5).Value = "Date : "+Convert.ToString(DateOnly.FromDateTime(DateTime.Now).ToString("dd-MM-yyyy"));
                worksheet1.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                currentRow++;
               
                currentRow = 14;

                worksheet1.Cell(currentRow, 2).Value = "Sno";
                worksheet1.Cell(currentRow, 3).Value = "Date";
                worksheet1.Cell(currentRow, 4).Value = "Vehicle No";
                worksheet1.Cell(currentRow, 5).Value = "Description";
                worksheet1.Cell(currentRow, 6).Value = "Amount";
                
                worksheet1.Range("A"+ currentRow+":I"+ currentRow).Style.Font.Bold = true; //FontSize
                worksheet1.Range("A" + currentRow + ":I" + currentRow).Row(currentRow).Style.Font.FontColor = XLColor.Black; //font color
                worksheet1.Range("A" + currentRow + ":I" + currentRow).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("A" + currentRow + ":I" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); //Alignment
                worksheet1.Range("A" + currentRow + ":I" + currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center); //Alignment
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.InsideBorderColor = XLColor.Black;

                // worksheet1.Range("A15:I15").Row(currentRow).Style.Fill.BackgroundColor = XLColor.;  // Background Color
                worksheet1.Range("A" + currentRow + ":E" + currentRow).Row(currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);//Alignment
                worksheet1.Range("A" + currentRow + ":E" + currentRow).Row(currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);//Alignment
                int SNo = 1;
                int subTotal = 0;
                int RentalEntrystart = currentRow+1;

                foreach (var rentalentry in InvoiceDetaills)
                {

                    currentRow++;
                    worksheet1.Cell(currentRow, 2).Value = SNo;
                    worksheet1.Cell(currentRow, 3).Value = rentalentry.rentalDate;
                    worksheet1.Cell(currentRow, 4).Value = rentalentry.vehicleRegNo;
                    worksheet1.Cell(currentRow, 5).Value = rentalentry.rentalDescription;
                    worksheet1.Cell(currentRow, 6).Value = rentalentry.rentalAmount;

                    //worksheet1.Row(currentRow).Style.Alignment.WrapText = true;
                    //worksheet1.Rows(currentRow, currentRow).AdjustToContents();
                    worksheet1.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                    subTotal = subTotal + rentalentry.rentalAmount;
                    SNo++;
                   
                }
                var workSheet1Range = worksheet1.Range("B"+ RentalEntrystart + ":F" + currentRow);
                int GST = Convert.ToInt32(subTotal * 0.06);
                int totalAmount = subTotal + (GST * 2);
                foreach (var cell in workSheet1Range.Cells())
                {
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    cell.Style.Font.FontSize = contentfontSize;
                }

                worksheet1.Range("F"+ RentalEntrystart+":F"+ currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet1.Range("E"+ RentalEntrystart+":E"+ currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet1.Range("E" + RentalEntrystart + ":E" + currentRow).Style.Alignment.WrapText = true;
                worksheet1.Range("E" + RentalEntrystart + ":E" + currentRow).Style.Alignment.Indent = 5;
                worksheet1.Rows(RentalEntrystart, currentRow).AdjustToContents();

                currentRow++;
                var rentalRowend = currentRow;
                worksheet1.Cell(currentRow, 5).Value = "Sub Total:";
                worksheet1.Cell(currentRow, 6).Value = subTotal;
                worksheet1.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet1.Range("B" + currentRow + ":E" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("F" + currentRow + ":F" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;
                currentRow++;
                worksheet1.Cell(currentRow, 5).Value = "SGST: 6%";
                worksheet1.Cell(currentRow, 6).Value = GST; 
                worksheet1.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet1.Range("B" + currentRow + ":E" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("F" + currentRow + ":F" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;
                currentRow++;
                worksheet1.Cell(currentRow, 5).Value = "CGST: 6%";
                worksheet1.Cell(currentRow, 6).Value = GST;
                worksheet1.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet1.Range("B" + currentRow + ":E" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("F" + currentRow + ":F" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;
                currentRow++;
                worksheet1.Cell(currentRow, 5).Value = "Total:";
                worksheet1.Cell(currentRow, 6).Value = totalAmount;
                worksheet1.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet1.Range("B" + currentRow + ":E" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("F" + currentRow + ":F" + currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("B" + currentRow + ":F" + currentRow).Style.Border.OutsideBorderColor = XLColor.Black;

                worksheet1.Range("E" + rentalRowend + ":E" + currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                currentRow++;

                worksheet1.Range("B" + currentRow + ":C" + (currentRow+1)).Merge();
                worksheet1.Cell(currentRow, 2).Value = "Rupees in Words";
                worksheet1.Range("B" + currentRow + ":C" + (currentRow + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("B" + currentRow + ":C" + (currentRow + 1)).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("B" + currentRow + ":C" + (currentRow + 1)).Style.Alignment.WrapText = true;
                worksheet1.Range("B" + currentRow + ":C" + (currentRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet1.Range("B" + currentRow + ":C" + (currentRow + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Merge();
                string rupeesInWords = clsCommon.CurrencyConversion.NumberToWords(Convert.ToInt32(totalAmount));    
                worksheet1.Cell(currentRow, 4).Value = rupeesInWords+" Rupees Only";
                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Style.Font.FontSize = contentfontSize;
                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Style.Alignment.WrapText = true;
                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet1.Range("D" + currentRow + ":F" + (currentRow + 1)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet1.Range("B" + currentRow + ":F" + (currentRow + 1)).Style.Border.OutsideBorderColor = XLColor.Black;

                currentRow+=2;
                worksheet1.Range("F"+currentRow+":F"+currentRow).Style.Border.OutsideBorderColor = XLColor.Black;
                worksheet1.Range("F"+currentRow+":F"+currentRow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("F"+currentRow+":F"+currentRow).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet1.Range("F"+currentRow+":F"+currentRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);

                worksheet1.Columns().AdjustToContents();
                worksheet1.Rows().AdjustToContents();
                int endRow = currentRow + 3;
                worksheet1.Range("A" + newAprxRow + ":G" + newAprxRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("A" + newAprxRow + ":G" + newAprxRow).Style.Border.TopBorderColor = XLColor.Black;
                worksheet1.Range("A" + newAprxRow + ":A"+ endRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("A" + newAprxRow + ":A" + endRow).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet1.Range("G" + newAprxRow + ":G" + endRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("G" + newAprxRow + ":G" + endRow).Style.Border.RightBorderColor = XLColor.Black;
                worksheet1.Range("A"+ endRow+ ":G" + endRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet1.Range("A"+ endRow+ ":G" + endRow).Style.Border.BottomBorderColor = XLColor.Black;
                worksheet1.Columns("A").Width = 3;
                worksheet1.Columns("B").Width = 6;
                worksheet1.Columns("C").Width = 14;
                worksheet1.Columns("D").Width = 15;
                worksheet1.Columns("E").Width = 33.29;
                worksheet1.Columns("F").Width = 11;
                worksheet1.Columns("G").Width = 3;
                worksheet1.Columns("H").Width = 3;
                //  worksheet1.Row(1).Height = 23.35;

                currentRow = 25;
                for (int i = 1; i < currentRow; i++)
                {
                   worksheet1.Row(i).Height = 23.35;
                }

                //  string lInvoiceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Invoice");
                string lInvoiceFolderPath = Path.Combine("C:\\Users\\paranthamans\\Desktop", "Invoice");
                 if (!Directory.Exists(lInvoiceFolderPath))
                {
                    Directory.CreateDirectory(lInvoiceFolderPath);
                }
                string initialExcelFilePath = Path.Combine(lInvoiceFolderPath, "Invoice.xlsx");
                //  var logFileName = Path.Combine(lInvoiceFolderPath, $"{DateTime.Now.ToString("ddMMyyyy")}_Invoice.xlsx");
                
                workbook.SaveAs(initialExcelFilePath);





                string pdfPath = Path.Combine(lInvoiceFolderPath, "Convert.pdf");
                ExcelFile workbook1 = ExcelFile.Load(initialExcelFilePath);
                var worksheet = workbook1.Worksheets[0];

                // Set narrower margins (e.g., 0.5 inches for top/bottom, 0.25 inches for left/right)
                worksheet.PrintOptions.LeftMargin =0.25;
                worksheet.PrintOptions.RightMargin = 0.25;
                worksheet.PrintOptions.TopMargin = 0.25;
                worksheet.PrintOptions.BottomMargin = 0.25;
                workbook1.Save(pdfPath, new PdfSaveOptions());

                string pdfFilePath = Path.Combine(lInvoiceFolderPath, "Invoice.pdf");
                pdfFilePath = clsCommon.toConvertStringInTestServer(pdfFilePath);

                //// Convert Excel to PDF using LibreOffice

                var libreOfficeProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = LibreOfficePath,
                        Arguments = $"--headless --convert-to pdf {initialExcelFilePath} --outdir {lInvoiceFolderPath}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                

                //if (File.Exists(tempPDFFilePath))
                //{
                //    File.Move(tempPDFFilePath, pdfFilePath);
                //}

                if (!Directory.Exists(pdfFilePath))
                {
                    libreOfficeProcess.Start();
                    libreOfficeProcess.WaitForExit();
                   // Directory.CreateDirectory(pdfFilePath);
                }
                else
                {
                    if (File.Exists(pdfFilePath))
                    {
                        File.Delete(pdfFilePath);
                    }
                    libreOfficeProcess.Start();
                    libreOfficeProcess.WaitForExit();
                }

            }
               return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }

        public ApiResponseDTO getFilteredInvoices(DateTime fromDate, DateTime toDate, int? companyId)
        {
            var filteredInvoices = _dbContext.mstInvoice.Where(i => i.createdDate >= fromDate && i.createdDate <= toDate && (companyId == null || i.mstCompanyId == companyId)).ToList();

            if (filteredInvoices.Any())
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Filtered invoices retrieved successfully.", data = filteredInvoices });
            }

            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "No invoices found matching the filter criteria." });
            
        }


    }
    
}
