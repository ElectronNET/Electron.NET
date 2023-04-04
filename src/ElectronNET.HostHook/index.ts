import { App } from "electron";
import { Socket } from "socket.io";

export class HookService {
  constructor(private socket: Socket, public app: App) {}

  private on(key: string, cb: (...args: Array<any>) => void): void {
    this.socket.on(key, (...args: Array<any>) => {
      const id: string = args.pop();

      try {
        cb(...args, (data) => {
          if (data) {
            this.socket.emit(`${key}Complete${id}`, data);
          }
        });
      } catch (error) {
        this.socket.emit(`${key}Error${id}`, `Host Hook Exception`, error);
      }
    });
  }

  onHostReady(): void {
    // execute your own JavaScript Host logic here
  }
}
