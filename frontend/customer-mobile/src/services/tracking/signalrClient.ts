import * as SignalR from '@microsoft/signalr';

export interface LocationUpdate {
  bookingId: string;
  latitude: number;
  longitude: number;
  speed?: number;
  heading?: number;
  timestamp: string;
}

export interface ETAUpdate {
  bookingId: string;
  estimatedArrival: string;
  remainingDistance: number;
  remainingTime: number;
}

export interface StatusUpdate {
  bookingId: string;
  status: string;
  timestamp: string;
}

class SignalRClient {
  private connection: SignalR.HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 5000;

  async connect(baseUrl: string, token: string): Promise<void> {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      return;
    }

    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(`${baseUrl}/hubs/tracking`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount >= this.maxReconnectAttempts) {
            return null;
          }
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
        },
      })
      .configureLogging(SignalR.LogLevel.Information)
      .build();

    this.connection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
    });

    this.connection.onreconnected(() => {
      console.log('SignalR reconnected');
      this.reconnectAttempts = 0;
    });

    this.connection.onclose(async (error) => {
      console.log('SignalR connection closed', error);
      if (this.reconnectAttempts < this.maxReconnectAttempts) {
        this.reconnectAttempts++;
        setTimeout(() => this.start(), this.reconnectDelay);
      }
    });

    await this.start();
  }

  private async start(): Promise<void> {
    if (!this.connection) return;

    try {
      await this.connection.start();
      console.log('SignalR connected');
    } catch (error) {
      console.error('SignalR connection error:', error);
      throw error;
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  async joinBookingTracking(bookingId: string): Promise<void> {
    if (!this.connection || this.connection.state !== SignalR.HubConnectionState.Connected) {
      throw new Error('SignalR not connected');
    }

    await this.connection.invoke('JoinBookingTracking', bookingId);
  }

  async leaveBookingTracking(bookingId: string): Promise<void> {
    if (!this.connection || this.connection.state !== SignalR.HubConnectionState.Connected) {
      return;
    }

    await this.connection.invoke('LeaveBookingTracking', bookingId);
  }

  onLocationUpdated(callback: (update: LocationUpdate) => void): void {
    if (!this.connection) return;
    this.connection.on('LocationUpdated', callback);
  }

  onETAUpdated(callback: (update: ETAUpdate) => void): void {
    if (!this.connection) return;
    this.connection.on('ETAUpdated', callback);
  }

  onStatusUpdated(callback: (update: StatusUpdate) => void): void {
    if (!this.connection) return;
    this.connection.on('StatusUpdated', callback);
  }

  offLocationUpdated(): void {
    if (!this.connection) return;
    this.connection.off('LocationUpdated');
  }

  offETAUpdated(): void {
    if (!this.connection) return;
    this.connection.off('ETAUpdated');
  }

  offStatusUpdated(): void {
    if (!this.connection) return;
    this.connection.off('StatusUpdated');
  }

  isConnected(): boolean {
    return this.connection?.state === SignalR.HubConnectionState.Connected;
  }
}

export const signalRClient = new SignalRClient();
