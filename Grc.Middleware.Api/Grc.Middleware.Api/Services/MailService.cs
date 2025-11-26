using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services
{
    public class MailService : BaseService, IMailService {

        public MailService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) { }

        public async Task<MailSettings> GetMailSettingsAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieving mail settings", "INFO");

            try {
                var settings = await uow.MailSettingsRepository.GetAllAsync(s => !s.IsDeleted);
                var activeSettings = settings.FirstOrDefault();
                if (activeSettings == null)
                    return null;

                return activeSettings;
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> InsertMailAsync(MailRecord mail) {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Save mail record >>>>");

            try {

                //..log the mail record being saved
                var setJson = JsonSerializer.Serialize(mail, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Mail data: {setJson}", "DEBUG");

                var added = await uow.MailRecordRepository.InsertAsync(mail);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(mail).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }


        #region private methods

        private async Task LogErrorAsync(IUnitOfWork uow, Exception ex)
        {
            Logger.LogActivity($"Failed to retrieve permission set: {ex.Message}", "ERROR");

            var currentEx = ex.InnerException;
            while (currentEx != null)
            {
                Logger.LogActivity($"Service Inner Exception: {currentEx.Message}", "ERROR");
                currentEx = currentEx.InnerException;
            }

            // Get company ID efficiently
            var company = await uow.CompanyRepository.GetAllAsync(c => true, false);
            long companyId = company.FirstOrDefault()?.Id ?? 1L;

            // Get innermost exception
            var innermostException = ex;
            while (innermostException.InnerException != null)
                innermostException = innermostException.InnerException;

            var errorObj = new SystemError
            {
                ErrorMessage = innermostException.Message,
                ErrorSource = "SYSTEM-ACCESS-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            uow.SystemErrorRespository.Insert(errorObj);
            await uow.SaveChangesAsync();
        }

        #endregion

    }
}
