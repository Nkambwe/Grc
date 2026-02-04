using Grc.Middleware.Api.Utils;
using System.Net;
using System.Net.Mail;

namespace Grc.Middleware.Api.Helpers {

    public class MailHandler {

        public static (bool, string, string) SendReviewMail(IServiceLogger logger, string from,string sendTo, string sendToName, string cc, string processName, int port, string pwd) {
			try {
                //..mail body
                var body = MailBody(sendToName, "Request for process review", processName);
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
                logger.LogActivity("MAIL SENT SUCCESSFULLY", "MSG");
                return (true, subject, body);
            } catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }
			
        }

        public static (bool, string, string) SendSubmissionMail(IServiceLogger logger, string from, string sendTo, string sendToName, string cc, string submissionType, string title, int port, string pwd) {
            try {
                //..mail body
                var body = MailSubmissionBody(sendToName,title, submissionType);
                var subject = submissionType.Equals("RETURN")? "Compliance Return submission notice": "Compliance circular submission notice";
                MailMessage mail = new() {
                    From = new MailAddress(from, "GRC SUITE")
                };
                mail.To.Add(sendTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.CC.Add(cc);
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new("pearlbank-com.mail.eo.outlook.com") {
                    Port = port,
                    Credentials = new NetworkCredential(from, pwd)
                };
                smtpClient.Send(mail);
                logger.LogActivity("MAIL SENT SUCCESSFULLY", "MSG");
                return (true, subject, body);
            } catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }

        }

		public static (bool, string, string) SendNewAccountMail(IServiceLogger logger, string from, string sendTo, 
            string sendToName, string cc, string title, int port, string pwd, string accountUsername, string accountPassword) {
            try {
                //..mail body
                var body = AccountMailBody(sendToName,title,accountUsername,accountPassword);
                var subject = "Compliance GRC Suite User account";
                MailMessage mail = new() {From = new MailAddress(from, "GRC SUITE")};
                mail.To.Add(sendTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.CC.Add(cc);
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new("pearlbank-com.mail.eo.outlook.com") {
                    Port = port,
                    Credentials = new NetworkCredential(from, pwd)
                };
                smtpClient.Send(mail);
                logger.LogActivity("MAIL SENT SUCCESSFULLY", "MSG");
                return (true, subject, body);
            } catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }

        }

        public static (bool, string, string) SendPasswordResetMail(IServiceLogger logger, string from, string sendTo, 
            string sendToName, string cc, string title, int port, string pwd, string accountPassword) {
            try {
                //..mail body
                var body = PasswordResetMailBody(sendToName,title,accountPassword);
                var subject = "Compliance GRC Suite Password reset";
                MailMessage mail = new() {From = new MailAddress(from, "GRC SUITE")};
                mail.To.Add(sendTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.CC.Add(cc);
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new("pearlbank-com.mail.eo.outlook.com") {
                    Port = port,
                    Credentials = new NetworkCredential(from, pwd)
                };
                smtpClient.Send(mail);
                logger.LogActivity("MAIL SENT SUCCESSFULLY", "MSG");
                return (true, subject, body);
            } catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }

        }

        public static (bool, string, string) SendPolicyNotificationMail(IServiceLogger logger,string from,string sendTo,string sendToName,string cc, string policyTitle,DateTime? nextReviewDate,int port,string pwd) {

            try {
                var reviewDateText = nextReviewDate.HasValue ? nextReviewDate.Value.ToString("dd MMM yyyy") : "Not Set";

                var body = PolicyNotificationMailBody(sendToName, policyTitle, reviewDateText);
                var subject = "Policy Review Notification - Action Required";

                MailMessage mail = new() {
                    From = new MailAddress(from, "GRC SUITE")
                };

                AddMailAddresses(mail.To, sendTo);
                AddMailAddresses(mail.CC, cc);

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new("pearlbank-com.mail.eo.outlook.com") {
                    Port = port,
                    Credentials = new NetworkCredential(from, pwd)
                };

                smtpClient.Send(mail);
                return (true, subject, body);

            } catch (Exception ex) {
                logger.LogActivity("MAIL Exception", "ERROR");
                logger.LogActivity(ex.Message, "ERROR-MSG");
                logger.LogActivity(ex.StackTrace, "STACKTRACE");
                return (false, string.Empty, string.Empty);
            }
        }

        private static string MailBody(string sendToName, string title,  string processName) {
            return $@"
				<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f9f9f9;'>
				<div style='max-width: 600px; margin: auto; background: white; border: 1px solid #ddd; border-radius: 5px; overflow: hidden;'>
				<div style='background-color: #0073e6; padding: 20px; color: white; text-align: center;'>
				<h2 style='margin: 0;'>{title}</h2>
				</div>
				<div style='padding: 20px;'>
				<p>Dear {sendToName},</p>
				<p>A process document '{processName}' requires your attention for approval or review</p>
				<p>Login to GRC Suite and attend to this document. Please note that this is tracked for TAT based on your response time</p>
				<p>Thanks for your attention to this matter.</p>
				<br />
				<p style='font-size: 12px; color: #777;'>This is an automated email — please do not reply directly to this message.</p>
				</div>
				<div style='background-color: #f0f0f0; padding: 10px; text-align: center; font-size: 12px; color: #999;'>
					  Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.
				</div>
				</div>
				</div>";
        }

        private static string AccountMailBody(string sendToName, string title, string username, string password) {
            return $@"
				<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f9f9f9;'>
				<div style='max-width: 600px; margin: auto; background: white; border: 1px solid #ddd; border-radius: 5px; overflow: hidden;'>
				<div style='background-color: #0073e6; padding: 20px; color: white; text-align: center;'>
				<h2 style='margin: 0;'>{title}</h2>
				</div>
				<div style='padding: 20px;'>
				<p>Dear {sendToName},</p>
				<p>User account has been setup for your access to the Compliance GRC Suite Application. Username is '{username}' and
                user password '{password}'. This is meant to be a single use password, please loging and do a password change</p>
				<p>Thanks for your attention to this matter.</p>
				<br />
				<p style='font-size: 12px; color: #777;'>This is an automated email — please do not reply directly to this message.</p>
				</div>
				<div style='background-color: #f0f0f0; padding: 10px; text-align: center; font-size: 12px; color: #999;'>
					  Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.
				</div>
				</div>
				</div>";
        }

        private static string PasswordResetMailBody(string sendToName, string title, string password) {
            return $@"
				<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f9f9f9;'>
				    <div style='max-width: 600px; margin: auto; background: white; border: 1px solid #ddd; border-radius: 5px; overflow: hidden;'>
				        <div style='background-color: #0073e6; padding: 20px; color: white; text-align: center;'>
				            <h2 style='margin: 0;'>{title}</h2>
				            </div>
				            <div style='padding: 20px;'>

				            <p>Dear {sendToName},</p>

				            <p>Your user account password for Compliance GRC Suite Application has been reset. Use this password '{password}' to access the applicatopn.</p>
                            <p>This is meant to be a single use password, please loging and do a password change</p>

				        </div>
				        <div class='footer'>
                            <p>This is an automated message from the GRC Suite. Please do not reply to this email.</p>
                            <p> Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.</p>
                        </div>
				    </div>
				</div>";
        }

        private static string MailSubmissionBody(string sendToName, string title, string submissionType) {
            var submission = submissionType.Equals("RETURN") ? $"Report/return '{title}'" : $"Circular '{title}'";
            var emailTitle = submissionType.Equals("RETURN") ? "Compliance Return submission notice" : "Compliance circular submission notice";
            return $@"
				<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f9f9f9;'>
				    <div style='max-width: 600px; margin: auto; background: white; border: 1px solid #ddd; border-radius: 5px; overflow: hidden;'>
				        <div style='background-color: #0073e6; padding: 20px; color: white; text-align: center;'>
				            <h2 style='margin: 0;'>{emailTitle}</h2>
				        </div>
				        <div style='padding: 20px;'>

				            <p>Dear {sendToName},</p>

				            <p>A '{submission}' has been submitted and an update made to the GRC Suite</p>

				            <p>Checkout the attached document for the details in case of need for review or reporting.</p>
				            
                            <p>If you have any questions or need assistance, please contact the Compliance department.</p>
                    
                            <p>Best regards,<br>
                            <strong>GRC Compliance System</strong></p>
				        </div>
				         <div class='footer'>
                            <p>This is an automated message from the GRC Suite. Please do not reply to this email.</p>
                            <p> Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.</p>
                        </div>
				    </div>
				</div>";
        }

        private static string PolicyNotificationMailBody(string recipientName, string policyTitle, string reviewDate) {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #0066cc; color: white; padding: 20px; text-align: center; }}
                    .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
                    .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                    .highlight {{ background-color: #fff3cd; padding: 10px; border-left: 4px solid #ffc107; margin: 20px 0; }}
                    .button {{ display: inline-block; padding: 10px 20px; background-color: #0066cc; color: white; text-decoration: none; border-radius: 5px; margin-top: 15px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Policy Review Notification</h2>
                    </div>
                    <div class='content'>
                        <p>Dear {recipientName},</p>
                    
                        <p>This is a notification regarding the following policy that requires your attention:</p>
                    
                        <div class='highlight'>
                            <strong>Policy:</strong> {policyTitle}<br>
                            <strong>Next Review Date:</strong> {reviewDate}
                        </div>
                    
                        <p>The policy is currently pending action. Please review and take the necessary steps to ensure compliance.</p>
                    
                        <p>If you have any questions or need assistance, please contact the Compliance department.</p>
                    
                        <p>Best regards,<br>
                        <strong>GRC Compliance System</strong></p>
                    </div>
                    <div class='footer'>
                        <p>This is an automated message from the GRC Suite. Please do not reply to this email.</p>
                        <p> Pearl Bank Uganda. GRC SUITE © {DateTime.Now.Year} All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private static void AddMailAddresses(MailAddressCollection collection,string addresses) {
            if (string.IsNullOrWhiteSpace(addresses))
                return;

            var normalized = addresses
                .Replace(";", ",")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim());

            foreach (var email in normalized) {
                collection.Add(new MailAddress(email));
            }
        }


    }
}
