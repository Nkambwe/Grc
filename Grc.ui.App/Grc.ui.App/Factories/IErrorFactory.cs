﻿using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IErrorFactory {
        Task<NotFoundModel> PrepareNotFoundModelAsync();
    }
}
