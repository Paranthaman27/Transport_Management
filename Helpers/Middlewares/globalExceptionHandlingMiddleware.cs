
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace Transport_Management.Helpers.Middlewares
{
    public class globalExceptionHandlingMiddleware : IMiddleware
    {
        private IApiResponseRepository _apiResponseRepository;

        public globalExceptionHandlingMiddleware(IApiResponseRepository apiResponseRepository)
        {
            _apiResponseRepository = apiResponseRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var controllerDescription = context.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>();
                var controllerName = controllerDescription.ControllerName;
                var actionName = controllerDescription.ActionName;
                StreamWriter sw;

                string logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logFolderPath))
                {
                    Directory.CreateDirectory(logFolderPath);
                }
                var logFileName = Path.Combine(logFolderPath, $"{DateTime.Now.ToString("ddMMyyyy")}_ErrorLog.txt");

                if (!File.Exists(logFileName))
                {

                    sw = File.CreateText(logFileName);
                    sw.Flush();
                    sw.Close();
                }
                //Directory.GetFiles(logFileName);
                sw = new StreamWriter(logFileName, append: true);
                string Lineseprator = "=";

                sw.WriteLine(string.Concat(Enumerable.Repeat(Lineseprator, 150)) + "\n");
                sw.WriteLine("Date and Time    : " + DateTime.Now.ToString("F") + "\n");
                sw.WriteLine("Controller Name  : " + controllerName + "\n");
                sw.WriteLine("Action Name      : " + actionName + "\n");
                sw.WriteLine("Error            : " + ex + "\n");

                sw.Flush();
                sw.Close();

                Directory.GetFiles(logFolderPath).Select(f => new FileInfo(f)).Where(f => f.CreationTime < DateTime.Now.AddDays(-7)).ToList().ForEach(f => f.Delete());
                var problemDetails = _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Error has Occured", data = ex });
                await context.Response.WriteAsJsonAsync(problemDetails);
               // throw new Exception(problemDetails);

            }

        }

        private void SendErrorLogEmail(string fileRoute, Exception ex)
        {
            string smtpHost = "smtppro.zoho.com";
            int smtpPort = 587;
            string smtpUser = "paranthamans@rndsoftech.com";
            string smtpPass = "Rndsoft@1";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(smtpUser);
                mail.To.Add(smtpUser);
                mail.Subject = "Error Log Report";
                mail.Body = "Please find the attached error log report.";
                mail.Attachments.Add(new Attachment(fileRoute));

                using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Send(mail);
                }
            }
        }
    }
}
