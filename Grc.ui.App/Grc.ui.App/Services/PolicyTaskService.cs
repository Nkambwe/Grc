using AutoMapper;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Linq.Dynamic.Core;

namespace Grc.ui.App.Services {

    public class PolicyTaskService : GrcBaseService, IPolicyTaskService {

        private IQueryable<PolicyTaskResponse> query = new List<PolicyTaskResponse>() {
            new PolicyTaskResponse {
                Id = 1,
                PolicyDocument = "Information Security Policy",
                TaskDescription = "Review and update security protocols",
                TaskStatus = "In Progress",
                AssigneeName = "John Smith",
                AssigneeEmail = "john.smith@company.com",
                AssigneeContact = "+1-555-0101",
                AssigneeDepartment = "IT Security",
                AssigneePosition = "Security Analyst",
                AssignedBy = "Sarah Johnson",
                AssignDate = new DateTime(2024, 1, 15),
                DueDate = new DateTime(2024, 2, 15),
                LastReminder = new DateTime(2024, 2, 1),
                ReminderIntervalDays = 7,
                NextReminder = new DateTime(2024, 2, 8),
                LastReminderSent = true,
                Comments = "Initial review completed, awaiting feedback"
            },
            new PolicyTaskResponse {
                Id = 2,
                PolicyDocument = "Remote Work Policy",
                TaskDescription = "Final approval for remote work guidelines",
                TaskStatus = "Pending",
                AssigneeName = "Maria Garcia",
                AssigneeEmail = "maria.garcia@company.com",
                AssigneeContact = "+1-555-0102",
                AssigneeDepartment = "HR",
                AssigneePosition = "HR Manager",
                AssignedBy = "David Chen",
                AssignDate = new DateTime(2024, 1, 20),
                DueDate = new DateTime(2024, 2, 10),
                LastReminder = null,
                ReminderIntervalDays = 5,
                NextReminder = new DateTime(2024, 1, 25),
                LastReminderSent = false,
                Comments = "Awaiting legal department review"
            },
            new PolicyTaskResponse {
                Id = 3,
                PolicyDocument = "Data Privacy Policy",
                TaskDescription = "Compliance check with GDPR regulations",
                TaskStatus = "Completed",
                AssigneeName = "Robert Kim",
                AssigneeEmail = "robert.kim@company.com",
                AssigneeContact = "+1-555-0103",
                AssigneeDepartment = "Legal",
                AssigneePosition = "Compliance Officer",
                AssignedBy = "Lisa Wong",
                AssignDate = new DateTime(2024, 1, 10),
                DueDate = new DateTime(2024, 1, 31),
                LastReminder = new DateTime(2024, 1, 25),
                ReminderIntervalDays = 3,
                NextReminder = null,
                LastReminderSent = true,
                Comments = "Policy approved and implemented"
            },
            new PolicyTaskResponse {
                Id = 4,
                PolicyDocument = "Social Media Policy",
                TaskDescription = "Update social media guidelines for 2024",
                TaskStatus = "In Progress",
                AssigneeName = "Emily Davis",
                AssigneeEmail = "emily.davis@company.com",
                AssigneeContact = "+1-555-0104",
                AssigneeDepartment = "Marketing",
                AssigneePosition = "Marketing Director",
                AssignedBy = "Michael Brown",
                AssignDate = new DateTime(2024, 1, 18),
                DueDate = new DateTime(2024, 2, 28),
                LastReminder = new DateTime(2024, 2, 5),
                ReminderIntervalDays = 10,
                NextReminder = new DateTime(2024, 2, 15),
                LastReminderSent = true,
                Comments = "Draft in progress, 60% complete"
            },
            new PolicyTaskResponse {
                Id = 5,
                PolicyDocument = "Code of Conduct",
                TaskDescription = "Annual review and employee training preparation",
                TaskStatus = "Overdue",
                AssigneeName = "James Wilson",
                AssigneeEmail = "james.wilson@company.com",
                AssigneeContact = "+1-555-0105",
                AssigneeDepartment = "HR",
                AssigneePosition = "Training Coordinator",
                AssignedBy = "Jennifer Lee",
                AssignDate = new DateTime(2023, 12, 1),
                DueDate = new DateTime(2024, 1, 15),
                LastReminder = new DateTime(2024, 1, 20),
                ReminderIntervalDays = 7,
                NextReminder = new DateTime(2024, 1, 27),
                LastReminderSent = true,
                Comments = "Escalated to department head"
            },
            new PolicyTaskResponse {
                Id = 6,
                PolicyDocument = "Disaster Recovery Plan",
                TaskDescription = "Test and validate recovery procedures",
                TaskStatus = "Not Started",
                AssigneeName = "Thomas Anderson",
                AssigneeEmail = "thomas.anderson@company.com",
                AssigneeContact = "+1-555-0106",
                AssigneeDepartment = "IT Operations",
                AssigneePosition = "Systems Administrator",
                AssignedBy = "Karen Martinez",
                AssignDate = new DateTime(2024, 1, 25),
                DueDate = new DateTime(2024, 3, 1),
                LastReminder = null,
                ReminderIntervalDays = 14,
                NextReminder = new DateTime(2024, 2, 8),
                LastReminderSent = false,
                Comments = "Scheduled for Q1 testing"
            },
            new PolicyTaskResponse {
                Id = 7,
                PolicyDocument = "Expense Reimbursement Policy",
                TaskDescription = "Update per diem rates and approval workflow",
                TaskStatus = "In Progress",
                AssigneeName = "Jessica Taylor",
                AssigneeEmail = "jessica.taylor@company.com",
                AssigneeContact = "+1-555-0107",
                AssigneeDepartment = "Finance",
                AssigneePosition = "Finance Manager",
                AssignedBy = "Kevin Rodriguez",
                AssignDate = new DateTime(2024, 1, 12),
                DueDate = new DateTime(2024, 2, 5),
                LastReminder = new DateTime(2024, 1, 29),
                ReminderIntervalDays = 5,
                NextReminder = new DateTime(2024, 2, 3),
                LastReminderSent = true,
                Comments = "Awaiting executive approval for new rates"
            },
            new PolicyTaskResponse {
                Id = 8,
                PolicyDocument = "Equipment Usage Policy",
                TaskDescription = "Create policy for company equipment usage",
                TaskStatus = "Completed",
                AssigneeName = "Daniel White",
                AssigneeEmail = "daniel.white@company.com",
                AssigneeContact = "+1-555-0108",
                AssigneeDepartment = "Operations",
                AssigneePosition = "Operations Manager",
                AssignedBy = "Amanda Harris",
                AssignDate = new DateTime(2024, 1, 5),
                DueDate = new DateTime(2024, 1, 26),
                LastReminder = new DateTime(2024, 1, 20),
                ReminderIntervalDays = 3,
                NextReminder = null,
                LastReminderSent = true,
                Comments = "Policy published and distributed to all employees"
            },
            new PolicyTaskResponse {
                Id = 9,
                PolicyDocument = "Vacation and Leave Policy",
                TaskDescription = "Review and align with new labor regulations",
                TaskStatus = "Pending",
                AssigneeName = "Michelle Clark",
                AssigneeEmail = "michelle.clark@company.com",
                AssigneeContact = "+1-555-0109",
                AssigneeDepartment = "HR",
                AssigneePosition = "Benefits Specialist",
                AssignedBy = "Brian Thompson",
                AssignDate = new DateTime(2024, 1, 22),
                DueDate = new DateTime(2024, 3, 15),
                LastReminder = null,
                ReminderIntervalDays = 10,
                NextReminder = new DateTime(2024, 2, 1),
                LastReminderSent = false,
                Comments = "Researching new state regulations"
            },
            new PolicyTaskResponse {
                Id = 10,
                PolicyDocument = "Health and Safety Protocol",
                TaskDescription = "Quarterly safety audit and documentation",
                TaskStatus = "In Progress",
                AssigneeName = "Christopher Lee",
                AssigneeEmail = "chris.lee@company.com",
                AssigneeContact = "+1-555-0110",
                AssigneeDepartment = "Facilities",
                AssigneePosition = "Safety Officer",
                AssignedBy = "Nancy Scott",
                AssignDate = new DateTime(2024, 1, 8),
                DueDate = new DateTime(2024, 1, 31),
                LastReminder = new DateTime(2024, 1, 24),
                ReminderIntervalDays = 7,
                NextReminder = new DateTime(2024, 1, 31),
                LastReminderSent = true,
                Comments = "Audit 80% complete, final report in progress"
            }
        }.AsQueryable();

        public PolicyTaskService(IApplicationLoggerFactory loggerFactory, 
                                IHttpHandler httpHandler, 
                                IEnvironmentProvider environment, 
                                IEndpointTypeProvider endpointType, 
                                IMapper mapper, 
                                IWebHelper webHelper, 
                                SessionManager sessionManager, 
                                IGrcErrorFactory errorFactory, 
                                IErrorService errorService) 
                                : base(loggerFactory, httpHandler, environment, 
                                      endpointType, mapper, webHelper, sessionManager, 
                                      errorFactory, errorService) {
        }

        public async Task<GrcResponse<PolicyTaskResponse>> GetPolicyTaskAsync(GrcIdRequest getRequest) {
            return await Task.FromResult(new GrcResponse<PolicyTaskResponse>(query.FirstOrDefault(q => q.Id == getRequest.RecordId)));
        }

        public async Task<GrcResponse<List<PolicyTaskResponse>>> GetAllAsync(GrcRequest request) {
            return await Task.FromResult(new GrcResponse<List<PolicyTaskResponse>>(query.ToList()));
        }

        public async Task<GrcResponse<PagedResponse<PolicyTaskResponse>>> GetAllPolicyTasks(TableListRequest request) {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.PolicyDocument != null && a.PolicyDocument.ToLower().Contains(lookUp)) ||
                    (a.AssigneeName != null && a.AssigneeName.ToLower().Contains(lookUp)) ||
                    (a.TaskStatus != null && a.TaskStatus.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            //..get paged data
            var page = new PagedResponse<PolicyTaskResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<PolicyTaskResponse>>(page));
        }

        public async Task<GrcResponse<PolicyTaskResponse>> CreatePolicyTaskAsync(PolicyTaskViewModel request) {
            var record = new PolicyTaskResponse
            {
                PolicyDocument = request.PolicyDocument,
                TaskDescription = request.TaskDescription,
                TaskStatus = "PROGRESS",
                AssigneeName = request.AssigneeName,
                AssigneeEmail = request.AssigneeEmail,
                AssigneeContact = request.AssigneeContact,
                AssigneeDepartment = request.AssigneeDepartment,
                AssigneePosition = request.AssigneePosition,
                AssignedBy = request.AssignedBy,
                AssignDate = request.AssignDate,
                DueDate = request.DueDate,
                LastReminder = request.LastReminder,
                ReminderIntervalDays = request.ReminderIntervalDays,
                NextReminder = request.NextReminder,
                LastReminderSent = request.LastReminderSent,
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<PolicyTaskResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeletePolicyTaskAsync(GrcIdRequest deleteRequest) {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Task not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Task deleted successfully"
            }));
        }

        public async Task<GrcResponse<PolicyTaskResponse>> UpdatePolicyTaskAsync(PolicyTaskViewModel request) {
            var record = query.FirstOrDefault(r => r.Id == request.Id);
            if (record == null) {
                return await Task.FromResult(new GrcResponse<PolicyTaskResponse>(record));
            }

            record.PolicyDocument = request.PolicyDocument;
            record.TaskDescription = request.TaskDescription;
            record.TaskStatus = "NEW";
            record.AssigneeName = request.AssigneeName;
            record.AssigneeEmail = request.AssigneeEmail;
            record.AssigneeContact = request.AssigneeContact;
            record.AssigneeDepartment = request.AssigneeDepartment;
            record.AssigneePosition = request.AssigneePosition;
            record.AssignedBy = request.AssignedBy;
            record.AssignDate = request.AssignDate;
            record.DueDate = request.DueDate;
            record.LastReminder = request.LastReminder;
            record.ReminderIntervalDays = request.ReminderIntervalDays;
            record.NextReminder = request.NextReminder;
            record.LastReminderSent = request.LastReminderSent;
            return await Task.FromResult(new GrcResponse<PolicyTaskResponse>(record));
        }
    }
}
