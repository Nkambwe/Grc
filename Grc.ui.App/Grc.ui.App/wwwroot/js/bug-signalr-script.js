     
class BugTrackingManager {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.init();
    }

    async init() {
        try {
            //..fetch configuration from server
            await this.loadConfiguration();
            
            if (!this.config || !this.config.SignalRHubUrl) {
                console.error('Failed to load SignalR configuration');
                return;
            }

            //..create connection using dynamic URL
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(this.config.SignalRHubUrl)
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Information)
                .build();

            console.log('Connecting to SignalR at:', this.config.SignalRHubUrl);
            console.log('Environment:', this.config.Environment);
            
            //..set up event handlers
            this.setupEventHandlers();

            //..start connection
            await this.startConnection();
        } catch (error) {
            console.error('Failed to initialize BugTrackingManager:', error);
        }
    }
    async loadConfiguration() {
        try {
            const response = await fetch('/api/config/client-config');
            if (response.ok) {
                this.config = await response.json();
                console.log('Configuration loaded:', this.config);
            } else {
                throw new Error(`Failed to load configuration: ${response.status}`);
            }
        } catch (error) {
            console.error('Error loading configuration:', error);
            //..fallback to default configuration
            this.config = {
                SignalRHubUrl: 'https://localhost:7245/grc/bughub', 
                Environment: 'Development'
            };
        }
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

    setupEventHandlers() {
        //..handle new error notifications
        this.connection.on("NewErrorOccurred", (errorModel) => {
            this.handleNewError(errorModel);
        });

        //..handle error count updates
        this.connection.on("ErrorCountUpdated", (countUpdate) => {
            this.updateErrorCounts(countUpdate);
        });

        //..connection event handlers
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

    async joinAdminGroup() {
        if (this.isConnected) {
            try {
                await this.connection.invoke("JoinAdminGroup");
                console.log("Joined admin group successfully");
            } catch (err) {
                console.error("Failed to join admin group:", err);
            }
        }
    }

    handleNewError(errorModel) {
        console.log("New error received:", errorModel);
        
        // Update error list
        this.addErrorToList(errorModel);
        
        // Show notification
        this.showErrorNotification(errorModel);
        
    }

    updateErrorCounts(countUpdate) {
        console.log("Error counts updated:", countUpdate);
        
        // Update dashboard counters
        const totalCountElement = document.getElementById('total-error-count');
        const todayCountElement = document.getElementById('today-error-count');
        
        if (totalCountElement) {
            totalCountElement.textContent = countUpdate.TotalErrors;
            this.animateCounter(totalCountElement);
        }
        
        if (todayCountElement) {
            todayCountElement.textContent = countUpdate.TodayErrors;
            this.animateCounter(todayCountElement);
        }

        // Update last updated time
        const lastUpdatedElement = document.getElementById('last-updated');
        if (lastUpdatedElement) {
            lastUpdatedElement.textContent = new Date(countUpdate.LastUpdated).toLocaleTimeString();
        }
    }

    addErrorToList(errorModel) {
        const errorList = document.getElementById('error-list');
        if (!errorList) return;

        const errorItem = document.createElement('div');
        errorItem.className = `error-item error-${errorModel.Severity.toLowerCase()}`;
        errorItem.innerHTML = `
            <div class="error-header">
                <span class="error-time">${new Date(errorModel.OccurredAt).toLocaleString()}</span>
                <span class="error-severity severity-${errorModel.Severity.toLowerCase()}">${errorModel.Severity}</span>
            </div>
            <div class="error-source">${errorModel.Source}</div>
            <div class="error-message">${errorModel.Message}</div>
            <div class="error-ip">IP: ${errorModel.IpAddress}</div>
        `;

        //..add to top of list
        errorList.insertBefore(errorItem, errorList.firstChild);

        //..remove old items if list gets too long
        while (errorList.children.length > 50) {
            errorList.removeChild(errorList.lastChild);
        }

        //..highlight the new item
        errorItem.style.backgroundColor = '#fff3cd';
        setTimeout(() => {
            errorItem.style.backgroundColor = '';
        }, 3000);
    }

    showErrorNotification(errorModel) {
        //..create toast notification
        const notification = document.createElement('div');
        notification.className = `toast toast-${errorModel.Severity.toLowerCase()}`;
        notification.innerHTML = `
            <div class="toast-header">
                <strong>New ${errorModel.Severity}</strong>
                <button type="button" class="btn-close" onclick="this.parentElement.parentElement.remove()">&times;</button>
            </div>
            <div class="toast-body">
                <strong>Source:</strong> ${errorModel.Source}<br>
                <strong>Message:</strong> ${errorModel.Message.substring(0, 100)}${errorModel.Message.length > 100 ? '...' : ''}
            </div>
        `;

        document.body.appendChild(notification);

        //..auto remove after 10 seconds
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 10000);
    }

    showConnectionStatus(message, type) {
        const statusElement = document.getElementById('signalr-status');
        if (statusElement) {
            statusElement.textContent = message;
            statusElement.className = `connection-status status-${type}`;
        }
        console.log(`Connection Status: ${message} (${type})`);
    }

    animateCounter(element) {
        if (element) {
            element.style.transform = 'scale(1.2)';
            element.style.transition = 'transform 0.3s ease';
            setTimeout(() => {
                element.style.transform = 'scale(1)';
            }, 300);
        }
    }

}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.bugTracker = new BugTrackingManager();
});