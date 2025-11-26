using Grc.Middleware.Api.Utils;
using System.Net;
using System.Net.Mail;

namespace Grc.Middleware.Api.Helpers {

    public class MailHandler {

        public static (bool, string, string) GenerateMail(IServiceLogger logger, string from,string sendTo, string sendToName, string cc, string processName, int port, string pwd) {
			try
			{
                //..mail body
                var body = MailBody(sendToName, processName);
                var subject = "Operations and service process approval/review";
                MailMessage mail = new() {
                    From = new MailAddress(from,"GRC SUITE")
                };
                mail.To.Add(sendTo);
                mail.Subject = subject;
                mail.Body =body;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new("pearlbank-com.mail.eo.outlook.com") {
                    Port = port,
                    Credentials = new NetworkCredential(from, pwd)
                };
                smtpClient.Send(mail);

                return (true, subject, body);
            }
			catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }
			
        }

        private static string MailBody(string sendToName, string processName) {
            return $@"
				<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f9f9f9;'>
				<div style='max-width: 600px; margin: auto; background: white; border: 1px solid #ddd; border-radius: 5px; overflow: hidden;'>
				<div style='background-color: #0073e6; padding: 20px; color: white; text-align: center;'>
				<h2 style='margin: 0;'>Service Request Acknowledgement</h2>
				</div>
				<div style='padding: 20px;'>
				<p>Dear {sendToName},</p>
				<p>A process document '{processName}' requires your attention for approval or review</p>
				<p>Login to GRC Suite and attend to this document. Please note that this is tracked for TAT based on your response time</p>
				<p>Thanks for your attenstion to this matter.</p>
				<br />
				<p style='font-size: 12px; color: #777;'>This is an automated email — please do not reply directly to this message.</p>
				</div>
				<div style='background-color: #f0f0f0; padding: 10px; text-align: center; font-size: 12px; color: #999;'>
					  Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.
				</div>
				</div>
				</div>";
        }

    }
}
