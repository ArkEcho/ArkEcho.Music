import { WebsocketService }from './websocketservice';
import { HttpServerService } from './httpserverservice';

let wsservice : WebsocketService = new WebsocketService(8080);

let httpservice : HttpServerService = new HttpServerService(3000);