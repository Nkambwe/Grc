     
class BugTrackingManager {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.init();
    }

    async init() {
        //..create connection using dynamic URL
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${backendApiUrl}/bughub`)
            .withAutomaticReconnect([0, 2000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        console.log('Connecting to SignalR at:', `${backendApiUrl}/bughub`);
                
        //..set up event handlers
        this.setupEventHandlers();

        //..start connection
        await this.startConnection();
    }

    // ... rest of your BugTrackingManager methods remain the same
    setupEventHandlers() {
        this.connection.on("NewErrorOccurred", (errorModel) => {
            this.handleNewError(errorModel);
        });

        this.connection.on("ErrorCountUpdated", (countUpdate) => {
            this.updateErrorCounts(countUpdate);
        });

        this.connection.onreconnecting((error) => {
            console.log("SignalR connection lost. Reconnecting...", error);
            this.showConnectionStatus("Reconnecting...", "warning");
        });

        this.connection.onreconnected((connectionId) => {
            console.log("SignalR reconnected:", connectionId);
            this.showConnectionStatus("Connected", "success");
            this.joinAdminGroup();
        });

        this.connection.onclose((error) => {
            console.log("SignalR connection closed:", error);
            this.showConnectionStatus("Disconnected", "error");
            this.isConnected = false;
        });
    }

    async startConnection() {
        try {
            await this.connection.start();
            console.log("SignalR connected successfully");
            this.isConnected = true;
            this.showConnectionStatus("Connected", "success");
            await this.joinAdminGroup();
        } catch (err) {
            console.error("SignalR connection failed:", err);
            this.showConnectionStatus("Failed to connect", "error");
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    // Add all your other methods here (handleNewError, updateErrorCounts, etc.)
    // ... (keeping them the same as before)
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.bugTracker = new BugTrackingManager();
});