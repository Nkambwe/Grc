using Grc.ui.App.Utils;

public interface IApplicationLoggerFactory {
    IApplicationLogger CreateLogger();
    IApplicationLogger CreateLogger(string name);
}
